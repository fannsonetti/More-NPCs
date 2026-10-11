using System.Linq;

namespace MoreNPCs.Distributor
{
    internal static class DistributorText
    {
        public static string UnlockSms() =>
            "Wholesale line's open. I'll text when I need product. Pay hits your card after drop-off.";

        public static string[] DealOfferMessages()
        {
            DistributorWeeklyOrder.EnsureCurrentWeekOrder();
            int days = DistributorConfig.DealDeadlineDays;
            var drug = DistributorWeeklyOrder.GetDrugDisplayName();
            var qty = DistributorWeeklyOrder.TargetQuantity;
            var payout = DistributorPayoutEstimate.FormatOrderPayout();
            bool premium = DistributorWeeklyOrder.OrderKind == DistributorOrderKind.PremiumProperties;

            string order = premium
                ? BuildPremiumOrderLine(qty, drug)
                : $"Need {qty} {drug}.";

            string terms = BuildPaymentTermsLine(days, payout, premium);
            string part1 = $"{order} {terms}".Trim();
            string part2 = "You can see my boat offshore. Leave it in the pier locker.";

            return new[] { part1, part2 };
        }

        private static string BuildPremiumOrderLine(int qty, string drug)
        {
            var effects = FormatPremiumEffectsLine();
            if (string.IsNullOrEmpty(effects))
                return $"Need {qty} {drug}. Custom mix, something I haven't seen before.";
            return $"Need {qty} {drug}. {effects}";
        }

        private static string BuildPaymentTermsLine(int days, string payout, bool premium)
        {
            string payment = string.IsNullOrEmpty(payout)
                ? "Payment on delivery."
                : premium
                    ? $"Payment is {payout} if the batch matches."
                    : $"Payment is {payout}.";

            return $"You've got {days} day{(days == 1 ? "" : "s")} to get it to me. {payment}";
        }

        private static string FormatPremiumEffectsLine()
        {
            var required = DistributorWeeklyOrder.RequiredPropertyNames
                .Select(DistributorPropertyCatalog.FormatName)
                .ToList();
            var forbidden = DistributorWeeklyOrder.ForbiddenPropertyName;

            if (required.Count == 0 && string.IsNullOrEmpty(forbidden))
                return string.Empty;

            var parts = new System.Collections.Generic.List<string>();
            if (required.Count > 0)
                parts.Add(string.Join(" and ", required));
            if (!string.IsNullOrEmpty(forbidden))
                parts.Add("no " + DistributorPropertyCatalog.FormatName(forbidden));

            return string.Join(". ", parts) + ".";
        }

        public static string BulkCompletedSms(float amount, string extraLine) =>
            $"Got it. ${amount:N0} on your card." +
            (string.IsNullOrEmpty(extraLine) ? "" : $" {extraLine}");

        public static string PremiumCompletedSms(float amount, string extraLine) =>
            $"Batch received. ${amount:N0} on your card." +
            (string.IsNullOrEmpty(extraLine) ? "" : $" {extraLine}");

        public static string ExtraUnitsLine(int overflowUnits) =>
            $"{overflowUnits} extra units at {(int)(DistributorConfig.Tier2Multiplier * 100f)}%.";

        public static string DealExpiredSms() =>
            "Order's closed. I'll text when I need something else.";

        public static string DealFailedSms() =>
            "Couldn't verify the drop. Order's cancelled.";
    }
}
