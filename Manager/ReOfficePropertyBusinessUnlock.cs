using System;
using MelonLoader;
using MoreNPCs.Utils;
using S1API.Entities;
using S1API.Property;

namespace MoreNPCs.Manager
{
    /// <summary>
    /// Unlocks artificial manager businesses when the player owns the matching RE Office whiteboard property
    /// (Laundromat / Post Office / Car Wash / Taco Ticklers — PropertyListing cards on the Downtown RE interior whiteboard).
    /// Tiers align with <see cref="ArtificialBusinessMapping.LaunderTier"/> (≈4k–50k in-game property range).
    /// </summary>
    public static class ReOfficePropertyBusinessUnlock
    {
        private static float _nextCheckAt;

        private static float CheckIntervalSeconds =>
            !MoreNPCsPreferences.Registered ? 12f : MoreNPCsPreferences.ReOffice_CheckIntervalSeconds.Value;

        public static void Update()
        {
            if (UnityEngine.Time.time < _nextCheckAt) return;
            _nextCheckAt = UnityEngine.Time.time + CheckIntervalSeconds;
            try
            {
                ReOfficeWhiteboardDisplay.SyncFromGameState();
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"ReOfficePropertyBusinessUnlock: {ex.Message}");
            }
        }

        /// <summary>Whether the player owns the RE whiteboard property that unlocks listings for this tier slot.</summary>
        public static bool PlayerOwnsReTierSlot(ArtificialBusinessMapping.LaunderTier tier)
        {
            var owned = BusinessManager.GetOwnedBusinesses();
            if (owned == null || owned.Count == 0) return false;
            foreach (var b in owned)
            {
                var n = b?.PropertyName?.Trim();
                if (string.IsNullOrEmpty(n)) continue;
                var k = n.ToLowerInvariant();
                switch (tier)
                {
                    case ArtificialBusinessMapping.LaunderTier.Laundromat:
                        if (k.Contains("laundromat")) return true;
                        break;
                    case ArtificialBusinessMapping.LaunderTier.PostOffice:
                        if (k.Contains("postal") || k.Contains("post office") || k.Contains("postoffice") ||
                            (k.Contains("post") && k.Contains("office")))
                            return true;
                        break;
                    case ArtificialBusinessMapping.LaunderTier.CarWash:
                        if ((k.Contains("car") && k.Contains("wash")) || k.Contains("carwash")) return true;
                        break;
                    case ArtificialBusinessMapping.LaunderTier.TacoTicklers:
                        if ((k.Contains("taco") && k.Contains("tickler")) || k.Contains("tacotickler")) return true;
                        break;
                    default:
                        return false;
                }
            }
            return false;
        }

    }
}
