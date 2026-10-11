using System;
using System.Reflection;
using ScheduleOne.DevUtilities;
using ScheduleOne.Product;

namespace MoreNPCs.Utils
{
    /// <summary>Drug product unit weights: baggie 1, jar 5, brick 20 (per stack quantity).</summary>
    internal static class ProductPackagingUnits
    {
        public const float UnitsPerBaggie = 1f;
        public const float UnitsPerJar = 5f;
        public const float UnitsPerBrick = 20f;

        public static float GetUnitsPerItem(object productItemInstance)
        {
            if (TryGetInstanceAmount(productItemInstance, out float amount))
                return amount;

            var applied = TryGetAppliedPackaging(productItemInstance);
            if (applied != null)
            {
                var id = GetPackagingDefinitionId(applied);
                return GetUnitsPerPackagingId(id) ?? UnitsPerBaggie;
            }

            var packagingId = TryGetPackagingId(productItemInstance);
            return GetUnitsPerPackagingId(packagingId) ?? UnitsPerBaggie;
        }

        /// <summary>Per-unit street value from live product pricing (effects-aware).</summary>
        public static float GetUnitMarketValue(object productItemInstance, object? definition = null)
        {
            if (TryGetPackagedValueFromInstance(productItemInstance, out float packaged))
            {
                float units = GetUnitsPerItem(productItemInstance);
                if (units > 0f) return packaged / units;
            }

            definition ??= productItemInstance.GetType().GetProperty("Definition")?.GetValue(productItemInstance);
            return GetRuntimeUnitPrice(definition);
        }

        /// <summary>Street value of one packaged item (one baggie/jar/brick), not the whole stack.</summary>
        public static float GetPackagedItemValue(object productItemInstance, object? definition = null)
        {
            if (TryGetPackagedValueFromInstance(productItemInstance, out float packaged))
                return packaged;

            definition ??= productItemInstance.GetType().GetProperty("Definition")?.GetValue(productItemInstance);
            return GetRuntimeUnitPrice(definition) * GetUnitsPerItem(productItemInstance);
        }

        public static float GetStackUnits(object productItemInstance, int stackQuantity) =>
            Math.Max(0, stackQuantity) * GetUnitsPerItem(productItemInstance);

        private static bool TryGetPackagedValueFromInstance(object productItemInstance, out float packagedValue)
        {
            packagedValue = 0f;
            try
            {
                var method = productItemInstance.GetType().GetMethod(
                    "GetMonetaryValue",
                    BindingFlags.Public | BindingFlags.Instance,
                    null,
                    Type.EmptyTypes,
                    null);
                if (method == null) return false;

                float stackValue = Convert.ToSingle(method.Invoke(productItemInstance, null));
                if (stackValue <= 0f) return false;

                int stackQty = GetInstanceStackQuantity(productItemInstance);
                if (stackQty <= 0) stackQty = 1;

                packagedValue = stackValue / stackQty;
                return packagedValue > 0f;
            }
            catch { }

            return false;
        }

        private static int GetInstanceStackQuantity(object productItemInstance)
        {
            foreach (var name in new[] { "Quantity", "quantity" })
            {
                var prop = productItemInstance.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
                if (prop?.GetValue(productItemInstance) is int i && i > 0) return i;
            }

            return 1;
        }

        private static float GetRuntimeUnitPrice(object? definition)
        {
            if (definition == null) return 15f;

            if (definition is ProductDefinition productDef
                && NetworkSingleton<ProductManager>.InstanceExists)
            {
                try
                {
                    float price = NetworkSingleton<ProductManager>.Instance.GetPrice(productDef);
                    if (price > 0f) return price;
                }
                catch { }
            }

            return GetDefinitionMarketValue(definition);
        }

        private static bool TryGetInstanceAmount(object item, out float amount)
        {
            amount = 0f;
            try
            {
                var prop = item.GetType().GetProperty("Amount", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (prop?.GetValue(item) is int i && i > 0)
                {
                    amount = i;
                    return true;
                }

                if (prop?.GetValue(item) is float f && f > 0f)
                {
                    amount = f;
                    return true;
                }
            }
            catch { }

            return false;
        }

        private static string? TryGetPackagingId(object item)
        {
            try
            {
                foreach (var name in new[] { "PackagingID", "packagingID", "PackagingId" })
                {
                    var prop = item.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (prop?.GetValue(item) is string id && !string.IsNullOrWhiteSpace(id))
                        return id;

                    var field = item.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (field?.GetValue(item) is string fieldId && !string.IsNullOrWhiteSpace(fieldId))
                        return fieldId;
                }
            }
            catch { }

            return null;
        }

        private static float GetDefinitionMarketValue(object? definition)
        {
            if (definition == null) return 15f;
            foreach (var name in new[] { "MarketValue", "Price", "Value" })
            {
                var prop = definition.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
                if (prop?.GetValue(definition) is { } v)
                {
                    try
                    {
                        var value = Convert.ToSingle(v);
                        if (value > 0f) return value;
                    }
                    catch { }
                }
            }

            return 15f;
        }

        private static object? TryGetAppliedPackaging(object item)
        {
            for (var t = item.GetType(); t != null; t = t.BaseType)
            {
                var p = t.GetProperty("AppliedPackaging", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (p == null) continue;
                var v = p.GetValue(item);
                if (v != null) return v;
            }

            return null;
        }

        private static string? GetPackagingDefinitionId(object packagingDef)
        {
            var t = packagingDef.GetType();
            var id = t.GetProperty("ID", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(packagingDef) as string
                     ?? t.GetField("ID", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(packagingDef) as string;
            if (!string.IsNullOrEmpty(id)) return id;

            return t.GetProperty("Name", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(packagingDef)?.ToString();
        }

        private static float? GetUnitsPerPackagingId(string? id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            id = id.Trim().ToLowerInvariant();
            if (id == "brick" || id.IndexOf("brick", StringComparison.OrdinalIgnoreCase) >= 0) return UnitsPerBrick;
            if (id == "jar" || id.IndexOf("jar", StringComparison.OrdinalIgnoreCase) >= 0) return UnitsPerJar;
            if (id == "baggie" || id.IndexOf("baggie", StringComparison.OrdinalIgnoreCase) >= 0) return UnitsPerBaggie;
            return null;
        }
    }
}
