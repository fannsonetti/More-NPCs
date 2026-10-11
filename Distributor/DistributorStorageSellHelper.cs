using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using MelonLoader;
using MoreNPCs.Distributor;
using MoreNPCs.Utils;
using S1API.Products;

namespace MoreNPCs.Distributor
{
    /// <summary>Sell/consume distributor order drugs from a WorldStorageEntity with tiered payout.</summary>
    internal static class DistributorStorageSellHelper
    {
        public sealed class StorageSaleResult
        {
            public float TotalPayout { get; set; }
            public int UnitsSold { get; set; }
            public int OrderUnitsSold { get; set; }
            public int OverflowUnitsSold { get; set; }
        }

        public static int CountDrugUnits(object storage, DrugType drug)
        {
            if (storage == null) return 0;
            int total = 0;
            foreach (var slot in EnumerateSlots(storage))
            {
                if (!TryGetProductInstance(slot, out var inst, out var qty) || qty <= 0) continue;
                if (!TryGetDrugType(inst, out var slotDrug) || slotDrug != drug) continue;
                total += (int)Math.Round(ProductPackagingUnits.GetStackUnits(inst, qty));
            }

            return total;
        }

        public static bool IsPlayerAccessing(object storage)
        {
            try
            {
                var accessor = storage.GetType().GetProperty("CurrentPlayerAccessor", BindingFlags.Public | BindingFlags.Instance)
                    ?.GetValue(storage);
                return accessor != null;
            }
            catch { return false; }
        }

        public static StorageSaleResult TrySellFromStorage(object storage, DrugType drug, int targetQty, DistributorOrderKind orderKind)
        {
            var result = new StorageSaleResult();
            if (storage == null || targetQty <= 0) return result;

            if (orderKind == DistributorOrderKind.PremiumProperties)
                return TrySellPremiumFromStorage(storage, drug, targetQty, result);

            return TrySellBulkFromStorage(storage, drug, targetQty, result);
        }

        private static StorageSaleResult TrySellBulkFromStorage(object storage, DrugType drug, int targetQty, StorageSaleResult result)
        {
            float tier1Remaining = targetQty;
            float tier1Mult = DistributorConfig.Tier1Multiplier;
            float tier2Mult = DistributorConfig.Tier2Multiplier;

            foreach (var slot in EnumerateSlots(storage))
            {
                if (!TryGetProductInstance(slot, out var inst, out var qty) || qty <= 0) continue;
                if (!TryGetDrugType(inst, out var slotDrug) || slotDrug != drug) continue;

                var def = inst.GetType().GetProperty("Definition")?.GetValue(inst);
                if (def == null) continue;

                float unitsPerItem = ProductPackagingUnits.GetUnitsPerItem(inst);
                float lineValue = ProductPackagingUnits.GetPackagedItemValue(inst, def);
                if (lineValue <= 0f) lineValue = 10f * unitsPerItem;

                for (int item = 0; item < qty; item++)
                {
                    float payout;
                    if (tier1Remaining >= unitsPerItem)
                    {
                        payout = lineValue * tier1Mult;
                        tier1Remaining -= unitsPerItem;
                        result.OrderUnitsSold += (int)Math.Round(unitsPerItem);
                    }
                    else if (tier1Remaining > 0f)
                    {
                        float tier1Share = tier1Remaining / unitsPerItem;
                        payout = lineValue * (tier1Share * tier1Mult + (1f - tier1Share) * tier2Mult);
                        result.OrderUnitsSold += (int)Math.Round(tier1Remaining);
                        tier1Remaining = 0f;
                    }
                    else
                    {
                        payout = lineValue * tier2Mult;
                        result.OverflowUnitsSold += (int)Math.Round(unitsPerItem);
                    }

                    result.TotalPayout += payout;
                    result.UnitsSold += (int)Math.Round(unitsPerItem);
                }

                slot.GetType().GetMethod("ChangeQuantity", new[] { typeof(int), typeof(bool) })
                    ?.Invoke(slot, new object[] { -qty, true });
            }

            return result;
        }

        private static StorageSaleResult TrySellPremiumFromStorage(
            object storage,
            DrugType drug,
            int targetQty,
            StorageSaleResult result)
        {
            float orderRemaining = targetQty;
            var required = DistributorWeeklyOrder.RequiredPropertyNames;
            var forbidden = DistributorWeeklyOrder.ForbiddenPropertyName;
            float fullMult = DistributorConfig.FullPriceMultiplier;
            float tier2Mult = DistributorConfig.Tier2Multiplier;

            var slotData = new List<(object slot, int qty, float lineValue, float unitsPerItem, float propertyMult)>();
            foreach (var slot in EnumerateSlots(storage))
            {
                if (!TryGetProductInstance(slot, out var inst, out var qty) || qty <= 0) continue;
                if (!TryGetDrugType(inst, out var slotDrug) || slotDrug != drug) continue;
                var def = inst.GetType().GetProperty("Definition")?.GetValue(inst);
                if (def == null) continue;
                float unitsPerItem = ProductPackagingUnits.GetUnitsPerItem(inst);
                float lineValue = ProductPackagingUnits.GetPackagedItemValue(inst, def);
                if (lineValue <= 0f) lineValue = 10f * unitsPerItem;
                var props = GetProductPropertyNames(inst);
                float propertyMult = ComputePremiumPropertyMultiplier(
                    props, required, forbidden,
                    DistributorConfig.MissingPropertyMultiplier,
                    DistributorConfig.ForbiddenPropertyMultiplier);
                slotData.Add((slot, qty, lineValue, unitsPerItem, propertyMult));
            }

            slotData.Sort((a, b) => b.propertyMult.CompareTo(a.propertyMult));

            foreach (var entry in slotData)
            {
                for (int item = 0; item < entry.qty; item++)
                {
                    float units = entry.unitsPerItem;
                    float payout;
                    if (orderRemaining > 0f)
                    {
                        payout = entry.lineValue * fullMult * entry.propertyMult;
                        float applied = Math.Min(units, orderRemaining);
                        result.OrderUnitsSold += (int)Math.Round(applied);
                        orderRemaining -= units;
                    }
                    else
                    {
                        payout = entry.lineValue * tier2Mult;
                        result.OverflowUnitsSold += (int)Math.Round(units);
                    }

                    result.TotalPayout += payout;
                    result.UnitsSold += (int)Math.Round(units);
                }

                entry.slot.GetType().GetMethod("ChangeQuantity", new[] { typeof(int), typeof(bool) })
                    ?.Invoke(entry.slot, new object[] { -entry.qty, true });
            }

            return result;
        }

        private static IEnumerable<object> EnumerateSlots(object storage)
        {
            var slots = storage.GetType().GetProperty("ItemSlots")?.GetValue(storage) as IEnumerable;
            if (slots == null) yield break;
            foreach (var slot in slots)
            {
                if (slot != null) yield return slot;
            }
        }

        private static bool TryGetProductInstance(object slot, out object inst, out int qty)
        {
            inst = slot.GetType().GetProperty("ItemInstance")?.GetValue(slot)!;
            qty = 0;
            if (inst == null) return false;
            var productType = FindType("ScheduleOne.Product.ProductItemInstance");
            if (productType == null || !productType.IsAssignableFrom(inst.GetType())) return false;
            var qtyProp = slot.GetType().GetProperty("Quantity");
            qty = qtyProp != null ? Convert.ToInt32(qtyProp.GetValue(slot)) : 0;
            return qty > 0;
        }

        private static bool TryGetDrugType(object instance, out DrugType drug)
        {
            drug = default;
            try
            {
                var def = instance.GetType().GetProperty("Definition")?.GetValue(instance);
                if (def == null) return false;
                foreach (var name in new[] { "DrugType", "drugType", "Type" })
                {
                    var prop = def.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
                    if (prop == null) continue;
                    var val = prop.GetValue(def);
                    if (val == null) continue;
                    if (val is DrugType dt) { drug = dt; return true; }
                    if (val is int i && Enum.IsDefined(typeof(DrugType), i)) { drug = (DrugType)i; return true; }
                    if (Enum.TryParse(val.ToString(), true, out DrugType parsed)) { drug = parsed; return true; }
                }
            }
            catch { }

            return false;
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
                    if (!props.Contains(req))
                        mult *= missingPropertyMultiplier;
                }
            }

            if (!string.IsNullOrWhiteSpace(forbidden) && props.Contains(forbidden))
                mult *= forbiddenPropertyMultiplier;

            return mult;
        }

        private static HashSet<string> GetProductPropertyNames(object instance)
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                var type = instance.GetType();
                foreach (var name in new[] { "Properties", "AppliedProperties", "ActiveProperties", "ProductProperties" })
                {
                    var prop = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (prop?.GetValue(instance) is not IEnumerable enumerable || enumerable is string) continue;
                    foreach (var item in enumerable)
                        AddPropertyName(set, item);
                }
            }
            catch { }

            return set;
        }

        private static void AddPropertyName(HashSet<string> set, object? value)
        {
            if (value == null) return;
            if (value is string s) { set.Add(s); return; }
            var t = value.GetType();
            foreach (var propName in new[] { "Name", "ID", "Id", "PropertyName" })
            {
                if (t.GetProperty(propName)?.GetValue(value) is string str && !string.IsNullOrWhiteSpace(str))
                {
                    set.Add(str);
                    return;
                }
            }
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
