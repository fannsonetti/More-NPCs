using System;
using System.Collections.Generic;
using System.Linq;
using MoreNPCs.Persistence;
using MoreNPCs.Utils;

namespace MoreNPCs.Distributor
{
    /// <summary>
    /// Weekly purchase order: bulk wholesale (large qty, tiered pricing) or premium property order (small qty, full price).
    /// </summary>
    public static class DistributorWeeklyOrder
    {
        private static int _runtimeWeek = -1;
        private static int _runtimeKind;
        private static int _runtimeDrug = (int)S1API.Products.DrugType.Cocaine;
        private static int _runtimeTargetQty;
        private static List<string> _runtimeRequiredProps = new List<string>();
        private static string _runtimeForbiddenProp = string.Empty;
        private static int _debugRollNonce;

        public static int OrderWeek => _runtimeWeek;

        public static DistributorOrderKind OrderKind => (DistributorOrderKind)_runtimeKind;

        public static S1API.Products.DrugType OrderedDrug => (S1API.Products.DrugType)_runtimeDrug;

        public static int TargetQuantity => _runtimeTargetQty;

        public static bool HasCurrentOrder() => TargetQuantity > 0 && IsCurrentOrderValid();

        public static IReadOnlyList<string> RequiredPropertyNames => ReadRequiredPropertyNames();

        public static string? ForbiddenPropertyName =>
            string.IsNullOrWhiteSpace(_runtimeForbiddenProp) ? null : _runtimeForbiddenProp;

        public static int CurrentWeekIndex() => Math.Max(0, GameTimeHelper.ElapsedDays / 7);

        public static void EnsureCurrentWeekOrder()
        {
            var week = CurrentWeekIndex();
            if (OrderWeek == week && TargetQuantity > 0 && IsCurrentOrderValid())
                return;
            RollNewOrder(week, GameTimeHelper.ElapsedDays);
        }

        /// <summary>Fresh order roll when a new deal is offered (varies by day, not only by week).</summary>
        public static void RollOrderForDeal()
        {
            int week = CurrentWeekIndex();
            int seedDay = GameTimeHelper.ElapsedDays;
            RollNewOrder(week, seedDay);
        }

        /// <summary>Debug-only reroll — not tied to elapsed days; new random order every press.</summary>
        public static void RollRandomOrderForDebug()
        {
            var rng = new Random(unchecked(
                Environment.TickCount
                ^ Guid.NewGuid().GetHashCode()
                ^ ++_debugRollNonce * 15485863));
            RollNewOrderCore(CurrentWeekIndex(), rng);
        }

        public static int RoundOrderQuantity(int quantity) =>
            Math.Max(DistributorConfig.OrderQuantityStep, ((quantity + DistributorConfig.OrderQuantityStep - 1) / DistributorConfig.OrderQuantityStep) * DistributorConfig.OrderQuantityStep);

        private static bool IsCurrentOrderValid()
        {
            if (TargetQuantity % DistributorConfig.OrderQuantityStep != 0)
                return false;

            var drug = OrderedDrug;

            if (OrderKind == DistributorOrderKind.PremiumProperties)
            {
                if (TargetQuantity < DistributorConfig.PremiumMinUnits
                    || TargetQuantity > DistributorConfig.PremiumMaxUnits)
                    return false;
                return RequiredPropertyNames.Count >= 2;
            }

            var range = GetBulkQuantityRange(drug);
            return TargetQuantity >= range.min && TargetQuantity <= range.max;
        }

        public static float GetDrugAmountMultiplier(S1API.Products.DrugType drug) =>
            drug switch
            {
                S1API.Products.DrugType.Marijuana => 1.5f,
                S1API.Products.DrugType.Methamphetamine => 1.2f,
                S1API.Products.DrugType.Shrooms => 1.3f,
                S1API.Products.DrugType.Cocaine => 1.0f,
                _ => 1.0f
            };

        public static (int min, int max) GetBulkQuantityRange(S1API.Products.DrugType drug)
        {
            float mult = GetDrugAmountMultiplier(drug);
            int min = RoundOrderQuantity(Math.Max(DistributorConfig.OrderQuantityStep, (int)Math.Round(DistributorConfig.BulkCocaineMinUnits * mult)));
            int max = RoundOrderQuantity(Math.Max(min, (int)Math.Round(DistributorConfig.BulkCocaineMaxUnits * mult)));
            return (min, max);
        }

        public static string GetDrugDisplayName() => GetDrugDisplayName(OrderedDrug);

        public static string GetDrugDisplayName(S1API.Products.DrugType drug) =>
            drug switch
            {
                S1API.Products.DrugType.Marijuana => "marijuana",
                S1API.Products.DrugType.Methamphetamine => "meth",
                S1API.Products.DrugType.Shrooms => "shrooms",
                S1API.Products.DrugType.Cocaine => "cocaine",
                _ => drug.ToString().ToLowerInvariant()
            };

        public static string GetOrderSummaryLine()
        {
            if (TargetQuantity <= 0)
                return "No order rolled";

            var drugName = GetDrugDisplayName();

            if (OrderKind == DistributorOrderKind.PremiumProperties)
            {
                return $"{TargetQuantity} units of custom {drugName}";
            }

            return $"{TargetQuantity} units of {drugName}";
        }

        public static string GetPricingSummaryLine()
        {
            if (OrderKind == DistributorOrderKind.PremiumProperties)
                return "Full price if it's right. Missing effects cost you. Wrong effect costs more.";

            return $"First {TargetQuantity} units at {DistributorConfig.Tier1Multiplier * 100f:0}%. After that {DistributorConfig.Tier2Multiplier * 100f:0}%.";
        }

        private static IReadOnlyList<string> ReadRequiredPropertyNames()
        {
            if (_runtimeRequiredProps == null || _runtimeRequiredProps.Count == 0)
                return Array.Empty<string>();
            return _runtimeRequiredProps.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
        }

        private static void RollNewOrder(int week, int seedSalt)
        {
            var rng = new Random(unchecked(week * 7919 + seedSalt * 9973 + 104729));
            RollNewOrderCore(week, rng);
        }

        private static void RollNewOrderCore(int week, Random rng)
        {
            var drugs = new[]
            {
                S1API.Products.DrugType.Marijuana,
                S1API.Products.DrugType.Methamphetamine,
                S1API.Products.DrugType.Shrooms,
                S1API.Products.DrugType.Cocaine
            };
            var drug = drugs[rng.Next(drugs.Length)];

            bool premium = rng.NextDouble() < DistributorConfig.PremiumOrderChance;
            int qty;
            var required = new List<string>();
            string forbidden = string.Empty;

            if (premium)
            {
                qty = RoundOrderQuantity(rng.Next(DistributorConfig.PremiumMinUnits, DistributorConfig.PremiumMaxUnits + 1));
                RollPropertyRequirements(rng, required, out forbidden);
            }
            else
            {
                var range = GetBulkQuantityRange(drug);
                int span = Math.Max(1, range.max - range.min + 1);
                int steps = span / DistributorConfig.OrderQuantityStep;
                int stepIndex = rng.Next(0, steps + 1);
                qty = RoundOrderQuantity(range.min + stepIndex * DistributorConfig.OrderQuantityStep);
                qty = Math.Min(qty, range.max);
            }

            ApplyOrder(week, premium, drug, qty, required, forbidden);
        }

        private static void ApplyOrder(
            int week,
            bool premium,
            S1API.Products.DrugType drug,
            int qty,
            List<string> required,
            string forbidden)
        {
            _runtimeWeek = week;
            _runtimeKind = premium ? (int)DistributorOrderKind.PremiumProperties : (int)DistributorOrderKind.BulkWholesale;
            _runtimeDrug = (int)drug;
            _runtimeTargetQty = qty;
            _runtimeRequiredProps = required;
            _runtimeForbiddenProp = forbidden ?? string.Empty;
            PersistRuntime();
        }

        private static void RollPropertyRequirements(Random rng, List<string> requiredOut, out string forbiddenOut)
        {
            var pool = DistributorPropertyCatalog.AllNames.ToList();
            if (pool.Count < 3)
            {
                forbiddenOut = string.Empty;
                return;
            }

            Shuffle(pool, rng);
            requiredOut.Add(pool[0]);
            requiredOut.Add(pool[1]);
            forbiddenOut = pool[2];
        }

        private static void Shuffle(IList<string> list, Random rng)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        internal static void SyncFromLoaded(DistributorSave save)
        {
            _runtimeWeek = save.OrderWeek;
            _runtimeKind = save.OrderKind;
            _runtimeDrug = save.OrderDrug;
            _runtimeTargetQty = save.OrderQuantity > 0
                ? RoundOrderQuantity(save.OrderQuantity)
                : save.OrderQuantity;
            _runtimeRequiredProps = save.OrderRequiredProps != null
                ? new List<string>(save.OrderRequiredProps)
                : new List<string>();
            _runtimeForbiddenProp = save.OrderForbiddenProp ?? string.Empty;
        }

        internal static void SyncFromLoaded(MoreNPCsModSave save)
        {
            _runtimeWeek = save._distributorOrderWeek;
            _runtimeKind = save._distributorOrderKind;
            _runtimeDrug = save._distributorOrderDrug;
            _runtimeTargetQty = save._distributorOrderQuantity;
            _runtimeRequiredProps = save._distributorOrderRequiredProps != null
                ? new List<string>(save._distributorOrderRequiredProps)
                : new List<string>();
            _runtimeForbiddenProp = save._distributorOrderForbiddenProp ?? string.Empty;
        }

        private static void PersistRuntime()
        {
            if (DistributorSave.Instance != null)
            {
                DistributorSave.Instance.OrderWeek = _runtimeWeek;
                DistributorSave.Instance.OrderKind = _runtimeKind;
                DistributorSave.Instance.OrderDrug = _runtimeDrug;
                DistributorSave.Instance.OrderQuantity = _runtimeTargetQty;
                DistributorSave.Instance.OrderRequiredProps = new List<string>(_runtimeRequiredProps);
                DistributorSave.Instance.OrderForbiddenProp = _runtimeForbiddenProp;
                return;
            }

            if (MoreNPCsModSave.Instance == null) return;
            MoreNPCsModSave.Instance._distributorOrderWeek = _runtimeWeek;
            MoreNPCsModSave.Instance._distributorOrderKind = _runtimeKind;
            MoreNPCsModSave.Instance._distributorOrderDrug = _runtimeDrug;
            MoreNPCsModSave.Instance._distributorOrderQuantity = _runtimeTargetQty;
            MoreNPCsModSave.Instance._distributorOrderRequiredProps = new List<string>(_runtimeRequiredProps);
            MoreNPCsModSave.Instance._distributorOrderForbiddenProp = _runtimeForbiddenProp;
        }
    }
}
