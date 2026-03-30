using System;
using MelonLoader;
using S1API.GameTime;

namespace MoreNPCs.Manager
{
    /// <summary>Adds daily passive income: one payout per tier slot (T1–T4) that has at least one assigned business, not per listing.</summary>
    public static class ManagerEarningsCollector
    {
        private static bool _subscribed;

        public static void EnsureSubscribed()
        {
            if (_subscribed) return;
            try
            {
                TimeManager.OnDayPass += OnDayPassed;
                _subscribed = true;
            }
            catch (Exception ex) { MelonLogger.Warning($"ManagerEarningsCollector subscribe: {ex.Message}"); }
        }

        private static void OnDayPassed()
        {
            try
            {
                var assigned = ManagerBusinessSave.GetAssignedStatic();
                if (assigned == null || assigned.Count == 0) return;
                var total = ArtificialBusinessMapping.GetDailyPassiveEarningsTotalForAssignments(assigned);
                if (total > 0) ManagerBusinessSave.AddEarningsStatic(total);
            }
            catch (Exception ex) { MelonLogger.Warning($"ManagerEarningsCollector OnDayPass: {ex.Message}"); }
        }

    }
}
