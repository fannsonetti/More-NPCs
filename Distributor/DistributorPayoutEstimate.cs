using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ScheduleOne.DevUtilities;
using ScheduleOne.Money;
using S1API.Products;

namespace MoreNPCs.Distributor
{
    internal static class DistributorPayoutEstimate
    {
        public static string FormatOrderPayout()
        {
            if (TryGetOrderPayout(out float payout))
                return MoneyManager.FormatAmount(payout, false, false);
            return string.Empty;
        }

        public static bool TryGetOrderPayout(out float payout)
        {
            payout = 0f;
            var pmType = FindType("ScheduleOne.Product.ProductManager");
            if (pmType == null || !NetworkSingletonExists(pmType))
                return false;

            object? pm = GetNetworkSingleton(pmType);
            if (pm == null)
                return false;

            object? baseDef = GetDefaultProductDefinition(pm, DistributorWeeklyOrder.OrderedDrug);
            if (baseDef == null)
                return false;

            int qty = DistributorWeeklyOrder.TargetQuantity;
            if (qty <= 0)
                return false;

            float unitPrice = GetEstimatedUnitPrice(pm, pmType, baseDef);
            if (unitPrice <= 0f)
                return false;

            if (DistributorWeeklyOrder.OrderKind == DistributorOrderKind.PremiumProperties)
                payout = unitPrice * qty * DistributorConfig.FullPriceMultiplier;
            else
                payout = unitPrice * qty * DistributorConfig.Tier1Multiplier;

            return payout > 0f;
        }

        private static float GetEstimatedUnitPrice(object pm, Type pmType, object baseDef)
        {
            if (DistributorWeeklyOrder.OrderKind == DistributorOrderKind.PremiumProperties)
            {
                var ids = DistributorWeeklyOrder.RequiredPropertyNames.ToList();
                if (ids.Count > 0)
                {
                    float fromEffects = TryCalculateFromRequiredEffects(baseDef, ids);
                    if (fromEffects > 0f)
                        return fromEffects;
                }
            }

            return InvokeGetPrice(pm, pmType, baseDef);
        }

        private static float TryCalculateFromRequiredEffects(object baseDef, List<string> ids)
        {
            try
            {
                var puType = FindType("ScheduleOne.Product.PropertyUtility");
                if (puType == null) return 0f;

                var singletonType = FindType("ScheduleOne.DevUtilities.Singleton`1");
                if (singletonType == null) return 0f;

                var closed = singletonType.MakeGenericType(puType);
                if (closed.GetProperty("InstanceExists")?.GetValue(null) is not true)
                    return 0f;

                var pu = closed.GetProperty("Instance")?.GetValue(null);
                if (pu == null) return 0f;

                var getProps = puType.GetMethod("GetProperties", new[] { typeof(List<string>) });
                if (getProps?.Invoke(pu, new object[] { ids }) is not IList effects || effects.Count == 0)
                    return 0f;

                float basePrice = ReadFloat(baseDef, "BasePrice");
                if (basePrice <= 0f) basePrice = ReadFloat(baseDef, "MarketValue");
                if (basePrice <= 0f) return 0f;

                var pmType = FindType("ScheduleOne.Product.ProductManager");
                var calc = pmType?.GetMethod(
                    "CalculateProductValue",
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    new[] { typeof(float), typeof(IList) },
                    null);
                if (calc == null) return 0f;

                return Convert.ToSingle(calc.Invoke(null, new object[] { basePrice, effects }));
            }
            catch
            {
                return 0f;
            }
        }

        private static float InvokeGetPrice(object pm, Type pmType, object baseDef)
        {
            try
            {
                var getPrice = pmType.GetMethod("GetPrice", new[] { baseDef.GetType().BaseType ?? baseDef.GetType() })
                    ?? pmType.GetMethod("GetPrice", BindingFlags.Public | BindingFlags.Instance);
                if (getPrice == null) return ReadFloat(baseDef, "MarketValue");

                var value = getPrice.Invoke(pm, new[] { baseDef });
                return value != null ? Convert.ToSingle(value) : 0f;
            }
            catch
            {
                return ReadFloat(baseDef, "MarketValue");
            }
        }

        private static object? GetDefaultProductDefinition(object pm, DrugType drug)
        {
            string fieldName = drug switch
            {
                DrugType.Marijuana => "DefaultWeed",
                DrugType.Methamphetamine => "DefaultMeth",
                DrugType.Shrooms => "DefaultShroom",
                DrugType.Cocaine => "DefaultCocaine",
                _ => string.Empty
            };

            if (string.IsNullOrEmpty(fieldName)) return null;
            return pm.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.Instance)?.GetValue(pm);
        }

        private static float ReadFloat(object target, string name)
        {
            var prop = target.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            if (prop?.GetValue(target) is { } v)
            {
                try { return Convert.ToSingle(v); }
                catch { }
            }

            return 0f;
        }

        private static bool NetworkSingletonExists(Type managerType)
        {
            var nsType = FindType("ScheduleOne.DevUtilities.NetworkSingleton`1");
            if (nsType == null) return false;
            var closed = nsType.MakeGenericType(managerType);
            return closed.GetProperty("InstanceExists")?.GetValue(null) is true;
        }

        private static object? GetNetworkSingleton(Type managerType)
        {
            var nsType = FindType("ScheduleOne.DevUtilities.NetworkSingleton`1");
            if (nsType == null) return null;
            return nsType.MakeGenericType(managerType).GetProperty("Instance")?.GetValue(null);
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
