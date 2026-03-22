using System;
using MelonLoader;
using S1API.GameTime;

namespace MoreNPCs.Manager
{
    /// <summary>Adds daily passive income from manager's assigned businesses. Laundromat 200, Post office 400, Carwash 600, Taco Ticklers 800.</summary>
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
                float total = 0f;
                foreach (var name in assigned)
                {
                    total += GetDailyEarningsFor(name);
                }
                if (total > 0) ManagerFundsSave.AddEarningsStatic(total);
            }
            catch (Exception ex) { MelonLogger.Warning($"ManagerEarningsCollector OnDayPass: {ex.Message}"); }
        }

        private static float GetDailyEarningsFor(string businessName)
        {
            if (string.IsNullOrWhiteSpace(businessName)) return 0f;
            var key = businessName.Trim().ToLowerInvariant();
            if (key.Contains("laundromat")) return 200f;
            if (key.Contains("post office") || key.Contains("postoffice")) return 400f;
            if (key.Contains("carwash") || key.Contains("car wash")) return 600f;
            if (key.Contains("taco") || key.Contains("tickler")) return 800f;
            return 0f;
        }
    }
}
