using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using MelonLoader;
using S1API.Entities;
using S1API.Products;

namespace MoreNPCs.Utils
{
    /// <summary>Reflection helpers for scanning/selling product items from the local player inventory.</summary>
    internal static class PlayerInventoryHelper
    {
        public sealed class WholesaleSaleResult
        {
            public float TotalPayout { get; set; }
            public int StacksSold { get; set; }
            public int UnitsSold { get; set; }
        }

        public static WholesaleSaleResult TrySellAllProducts(float wholesaleMultiplier)
        {
            var result = new WholesaleSaleResult();
            try
            {
                var inv = GetPlayerInventory();
                if (inv == null) return result;

                var slots = inv.GetType().GetProperty("ItemSlots")?.GetValue(inv) as IEnumerable;
                if (slots == null) return result;

                var productInstanceType = FindType("ScheduleOne.Product.ProductItemInstance");
                var productDefType = FindType("ScheduleOne.Product.ProductDefinition");
                if (productInstanceType == null || productDefType == null) return result;

                foreach (var slot in slots)
                {
                    if (slot == null) continue;
                    var inst = slot.GetType().GetProperty("ItemInstance")?.GetValue(slot);
                    if (inst == null || !productInstanceType.IsAssignableFrom(inst.GetType())) continue;

                    var qtyProp = slot.GetType().GetProperty("Quantity");
                    int qty = qtyProp != null ? Convert.ToInt32(qtyProp.GetValue(slot)) : 0;
                    if (qty <= 0) continue;

                    var def = inst.GetType().GetProperty("Definition")?.GetValue(inst);
                    if (def == null || !productDefType.IsAssignableFrom(def.GetType())) continue;

                    float unitValue = GetMarketValue(def, inst);
                    if (unitValue <= 0f) unitValue = 10f;

                    float payout = unitValue * qty * wholesaleMultiplier;
                    if (payout <= 0f) continue;

                    slot.GetType().GetMethod("ChangeQuantity", new[] { typeof(int), typeof(bool) })
                        ?.Invoke(slot, new object[] { -qty, true });

                    result.TotalPayout += payout;
                    result.StacksSold++;
                    result.UnitsSold += qty;
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"PlayerInventoryHelper.TrySellAllProducts: {ex.Message}");
            }

            return result;
        }

        public static bool HasSellableProducts()
        {
            try
            {
                var inv = GetPlayerInventory();
                if (inv == null) return false;
                var slots = inv.GetType().GetProperty("ItemSlots")?.GetValue(inv) as IEnumerable;
                if (slots == null) return false;
                var productInstanceType = FindType("ScheduleOne.Product.ProductItemInstance");
                if (productInstanceType == null) return false;

                foreach (var slot in slots)
                {
                    if (slot == null) continue;
                    var inst = slot.GetType().GetProperty("ItemInstance")?.GetValue(slot);
                    if (inst == null || !productInstanceType.IsAssignableFrom(inst.GetType())) continue;
                    var qtyProp = slot.GetType().GetProperty("Quantity");
                    int qty = qtyProp != null ? Convert.ToInt32(qtyProp.GetValue(slot)) : 0;
                    if (qty > 0) return true;
                }
            }
            catch { }
            return false;
        }

        public static bool HasSellableDrug(DrugType drug) => CountDrugUnits(drug) > 0;

        public static int CountDrugUnits(DrugType drug)
        {
            try
            {
                var inv = GetPlayerInventory();
                if (inv == null) return 0;
                var slots = inv.GetType().GetProperty("ItemSlots")?.GetValue(inv) as IEnumerable;
                if (slots == null) return 0;
                var productInstanceType = FindType("ScheduleOne.Product.ProductItemInstance");
                if (productInstanceType == null) return 0;

                float total = 0f;
                foreach (var slot in slots)
                {
                    if (slot == null) continue;
                    var inst = slot.GetType().GetProperty("ItemInstance")?.GetValue(slot);
                    if (inst == null || !productInstanceType.IsAssignableFrom(inst.GetType())) continue;
                    var def = inst.GetType().GetProperty("Definition")?.GetValue(inst);
                    if (def == null || !TryGetDrugType(def, out var slotDrug) || slotDrug != drug) continue;
                    var qtyProp = slot.GetType().GetProperty("Quantity");
                    int qty = qtyProp != null ? Convert.ToInt32(qtyProp.GetValue(slot)) : 0;
                    if (qty > 0) total += ProductPackagingUnits.GetStackUnits(inst, qty);
                }

                return (int)Math.Round(total);
            }
            catch { return 0; }
        }

        public static WholesaleSaleResult TrySellWeeklyOrderDrug(
            DrugType drug,
            int targetQty,
            float tier1Multiplier,
            float tier2Multiplier)
        {
            var result = new WholesaleSaleResult();
            try
            {
                var inv = GetPlayerInventory();
                if (inv == null) return result;

                var slots = inv.GetType().GetProperty("ItemSlots")?.GetValue(inv) as IEnumerable;
                if (slots == null) return result;

                var productInstanceType = FindType("ScheduleOne.Product.ProductItemInstance");
                var productDefType = FindType("ScheduleOne.Product.ProductDefinition");
                if (productInstanceType == null || productDefType == null) return result;

                float tier1Remaining = Math.Max(0f, targetQty);

                foreach (var slot in slots)
                {
                    if (slot == null) continue;
                    var inst = slot.GetType().GetProperty("ItemInstance")?.GetValue(slot);
                    if (inst == null || !productInstanceType.IsAssignableFrom(inst.GetType())) continue;

                    var qtyProp = slot.GetType().GetProperty("Quantity");
                    int qty = qtyProp != null ? Convert.ToInt32(qtyProp.GetValue(slot)) : 0;
                    if (qty <= 0) continue;

                    var def = inst.GetType().GetProperty("Definition")?.GetValue(inst);
                    if (def == null || !productDefType.IsAssignableFrom(def.GetType())) continue;
                    if (!TryGetDrugType(def, out var slotDrug) || slotDrug != drug) continue;

                    float unitValue = GetMarketValue(def, inst);
                    if (unitValue <= 0f) unitValue = 10f;

                    float unitsPerItem = ProductPackagingUnits.GetUnitsPerItem(inst);
                    float lineValue = ProductPackagingUnits.GetPackagedItemValue(inst, def);
                    if (lineValue <= 0f) lineValue = unitValue * unitsPerItem;
                    float payout = 0f;
                    float unitsSoldFromStack = 0f;

                    for (int item = 0; item < qty; item++)
                    {
                        if (tier1Remaining >= unitsPerItem)
                        {
                            payout += lineValue * tier1Multiplier;
                            tier1Remaining -= unitsPerItem;
                            unitsSoldFromStack += unitsPerItem;
                        }
                        else if (tier1Remaining > 0f)
                        {
                            float tier1Share = tier1Remaining / unitsPerItem;
                            payout += lineValue * (tier1Share * tier1Multiplier + (1f - tier1Share) * tier2Multiplier);
                            unitsSoldFromStack += unitsPerItem;
                            tier1Remaining = 0f;
                        }
                        else
                        {
                            payout += lineValue * tier2Multiplier;
                            unitsSoldFromStack += unitsPerItem;
                        }
                    }

                    if (payout <= 0f) continue;

                    slot.GetType().GetMethod("ChangeQuantity", new[] { typeof(int), typeof(bool) })
                        ?.Invoke(slot, new object[] { -qty, true });

                    result.TotalPayout += payout;
                    result.StacksSold++;
                    result.UnitsSold += (int)Math.Round(unitsSoldFromStack);
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"PlayerInventoryHelper.TrySellWeeklyOrderDrug: {ex.Message}");
            }

            return result;
        }

        public static bool HasMatchingPremiumOrder(
            DrugType drug,
            IReadOnlyList<string> required,
            string? forbidden) =>
            CountMatchingPremiumUnits(drug, required, forbidden) > 0;

        public static int CountMatchingPremiumUnits(
            DrugType drug,
            IReadOnlyList<string> required,
            string? forbidden)
        {
            try
            {
                var inv = GetPlayerInventory();
                if (inv == null) return 0;
                var slots = inv.GetType().GetProperty("ItemSlots")?.GetValue(inv) as IEnumerable;
                if (slots == null) return 0;
                var productInstanceType = FindType("ScheduleOne.Product.ProductItemInstance");
                if (productInstanceType == null) return 0;

                int total = 0;
                foreach (var slot in slots)
                {
                    if (slot == null) continue;
                    var inst = slot.GetType().GetProperty("ItemInstance")?.GetValue(slot);
                    if (inst == null || !productInstanceType.IsAssignableFrom(inst.GetType())) continue;
                    var def = inst.GetType().GetProperty("Definition")?.GetValue(inst);
                    if (def == null || !TryGetDrugType(def, out var slotDrug) || slotDrug != drug) continue;
                    if (!ProductMatchesPropertyOrder(inst, required, forbidden)) continue;
                    var qtyProp = slot.GetType().GetProperty("Quantity");
                    int qty = qtyProp != null ? Convert.ToInt32(qtyProp.GetValue(slot)) : 0;
                    if (qty > 0) total += qty;
                }

                return total;
            }
            catch { return 0; }
        }

        /// <summary>Sell premium-order drug up to <paramref name="targetQty"/>; payout reduced per missing/forbidden effects.</summary>
        public static WholesaleSaleResult TrySellPremiumOrderDrug(
            DrugType drug,
            int targetQty,
            IReadOnlyList<string> required,
            string? forbidden,
            float fullPriceMultiplier,
            float missingPropertyMultiplier,
            float forbiddenPropertyMultiplier)
        {
            var result = new WholesaleSaleResult();
            try
            {
                var inv = GetPlayerInventory();
                if (inv == null) return result;

                var slots = inv.GetType().GetProperty("ItemSlots")?.GetValue(inv) as IEnumerable;
                if (slots == null) return result;

                var productInstanceType = FindType("ScheduleOne.Product.ProductItemInstance");
                var productDefType = FindType("ScheduleOne.Product.ProductDefinition");
                if (productInstanceType == null || productDefType == null) return result;

                float remainingUnits = Math.Max(0f, targetQty);

                foreach (var slot in slots)
                {
                    if (remainingUnits <= 0f) break;
                    if (slot == null) continue;
                    var inst = slot.GetType().GetProperty("ItemInstance")?.GetValue(slot);
                    if (inst == null || !productInstanceType.IsAssignableFrom(inst.GetType())) continue;

                    var qtyProp = slot.GetType().GetProperty("Quantity");
                    int qty = qtyProp != null ? Convert.ToInt32(qtyProp.GetValue(slot)) : 0;
                    if (qty <= 0) continue;

                    var def = inst.GetType().GetProperty("Definition")?.GetValue(inst);
                    if (def == null || !productDefType.IsAssignableFrom(def.GetType())) continue;
                    if (!TryGetDrugType(def, out var slotDrug) || slotDrug != drug) continue;

                    var props = GetProductPropertyNames(inst);
                    float propertyMult = ComputePremiumPropertyMultiplier(
                        props, required, forbidden, missingPropertyMultiplier, forbiddenPropertyMultiplier);
                    if (propertyMult <= 0f) continue;

                    float unitsPerItem = ProductPackagingUnits.GetUnitsPerItem(inst);
                    int sellItems = Math.Min(qty, Math.Max(1, (int)Math.Ceiling(remainingUnits / unitsPerItem)));
                    sellItems = Math.Min(sellItems, qty);

                    float unitValue = GetMarketValue(def, inst);
                    if (unitValue <= 0f) unitValue = 10f;
                    float lineValue = ProductPackagingUnits.GetPackagedItemValue(inst, def);
                    if (lineValue <= 0f) lineValue = unitValue * unitsPerItem;

                    float payout = sellItems * lineValue * fullPriceMultiplier * propertyMult;
                    if (payout <= 0f) continue;

                    slot.GetType().GetMethod("ChangeQuantity", new[] { typeof(int), typeof(bool) })
                        ?.Invoke(slot, new object[] { -sellItems, true });

                    float unitsSold = sellItems * unitsPerItem;
                    result.TotalPayout += payout;
                    result.StacksSold++;
                    result.UnitsSold += (int)Math.Round(unitsSold);
                    remainingUnits -= unitsSold;
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"PlayerInventoryHelper.TrySellPremiumOrderDrug: {ex.Message}");
            }

            return result;
        }

        private static float ComputePremiumPropertyMultiplier(
            HashSet<string> props,
            IReadOnlyList<string> required,
            string? forbidden,
            float missingPropertyMultiplier,
            float forbiddenPropertyMultiplier)
        {
            float mult = 1f;

            if (required != null)
            {
                foreach (var req in required)
                {
                    if (string.IsNullOrWhiteSpace(req)) continue;
                    if (!props.Contains(req, StringComparer.OrdinalIgnoreCase))
                        mult *= missingPropertyMultiplier;
                }
            }

            if (!string.IsNullOrWhiteSpace(forbidden)
                && props.Contains(forbidden, StringComparer.OrdinalIgnoreCase))
                mult *= forbiddenPropertyMultiplier;

            return mult;
        }

        private static bool ProductMatchesPropertyOrder(
            object instance,
            IReadOnlyList<string> required,
            string? forbidden)
        {
            var props = GetProductPropertyNames(instance);

            if (!string.IsNullOrWhiteSpace(forbidden)
                && props.Contains(forbidden, StringComparer.OrdinalIgnoreCase))
                return false;

            if (required == null || required.Count == 0)
                return true;

            foreach (var req in required)
            {
                if (string.IsNullOrWhiteSpace(req)) continue;
                if (!props.Contains(req, StringComparer.OrdinalIgnoreCase))
                    return false;
            }

            return true;
        }

        private static HashSet<string> GetProductPropertyNames(object instance)
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                var type = instance.GetType();
                foreach (var name in new[]
                         {
                             "Properties", "AppliedProperties", "ActiveProperties", "ProductProperties",
                             "AppliedProperty", "Property"
                         })
                {
                    var member = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                        ?? (MemberInfo?)type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (member == null) continue;

                    object? val = member is PropertyInfo pi
                        ? pi.GetValue(instance)
                        : ((FieldInfo)member).GetValue(instance);
                    if (val == null) continue;

                    AddPropertyName(set, val);

                    if (val is IEnumerable enumerable && val is not string)
                    {
                        foreach (var item in enumerable)
                            AddPropertyName(set, item);
                    }
                }
            }
            catch { }

            return set;
        }

        private static void AddPropertyName(HashSet<string> set, object? value)
        {
            if (value == null) return;

            var name = ExtractPropertyName(value);
            if (!string.IsNullOrWhiteSpace(name))
                set.Add(name);
        }

        private static string? ExtractPropertyName(object value)
        {
            if (value is string s) return s;

            var t = value.GetType();
            foreach (var propName in new[] { "Name", "ID", "Id", "PropertyName" })
            {
                var prop = t.GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (prop?.GetValue(value) is string str && !string.IsNullOrWhiteSpace(str))
                    return str;
            }

            var text = value.ToString();
            if (string.IsNullOrWhiteSpace(text)) return null;
            if (text.Contains("."))
                text = text.Substring(text.LastIndexOf('.') + 1);
            return text;
        }

        private static bool TryGetDrugType(object definition, out DrugType drug)
        {
            drug = default;
            try
            {
                foreach (var name in new[] { "DrugType", "drugType", "Type" })
                {
                    var prop = definition.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
                    if (prop == null) continue;
                    var val = prop.GetValue(definition);
                    if (val == null) continue;
                    if (val is DrugType dt) { drug = dt; return true; }
                    if (val is int i && Enum.IsDefined(typeof(DrugType), i)) { drug = (DrugType)i; return true; }
                    if (Enum.TryParse(val.ToString(), true, out DrugType parsed)) { drug = parsed; return true; }
                }
            }
            catch { }

            return false;
        }

        private static float GetMarketValue(object definition, object instance)
        {
            foreach (var name in new[] { "MarketValue", "Price", "Value", "GetMarketValue" })
            {
                var prop = definition.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
                if (prop != null)
                {
                    try
                    {
                        var v = prop.GetValue(definition);
                        if (v != null) return Convert.ToSingle(v);
                    }
                    catch { }
                }

                var method = definition.GetType().GetMethod(name, BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
                if (method != null)
                {
                    try
                    {
                        var v = method.Invoke(definition, null);
                        if (v != null) return Convert.ToSingle(v);
                    }
                    catch { }
                }
            }

            var qualityProp = instance.GetType().GetProperty("Quality");
            if (qualityProp != null)
            {
                try
                {
                    var q = Convert.ToSingle(qualityProp.GetValue(instance));
                    return 20f + q * 5f;
                }
                catch { }
            }

            return 15f;
        }

        private static object? GetPlayerInventory()
        {
            var player = Player.Local;
            if (player?.Transform == null) return null;

            var invType = FindType("ScheduleOne.PlayerScripts.PlayerInventory")
                ?? FindType("ScheduleOne.PlayerScripts.PlayerInventoryLoader");
            if (invType == null) return null;

            foreach (var name in new[] { "Instance", "Local", "PlayerInventory" })
            {
                var prop = invType.GetProperty(name, BindingFlags.Public | BindingFlags.Static);
                if (prop?.GetValue(null) is { } inst) return inst;
            }

            var go = player.Transform.gameObject;
            return go.GetComponent(invType) ?? go.GetComponentInChildren(invType, true);
        }

        private static Type? FindType(string fullName)
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                var t = asm.GetType(fullName);
                if (t != null) return t;
            }
            return null;
        }
    }
}
