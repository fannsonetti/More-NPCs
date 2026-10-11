using System.Collections.Generic;

namespace MoreNPCs.Utils
{
    /// <summary>
    /// Artificial businesses that have no buildable interior in the base game. Gated by
    /// <see cref="MoreNPCsPreferences.ExperimentalProperties"/> (off = excluded from manager / unlocks / economy).
    /// </summary>
    public static class ExperimentalArtificialProperties
    {
        private static readonly HashSet<string> NoInteriorBusinessNames =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Chinese Restaurant",
                "Thompson Construction",
                "Koyama Chemical",
                "Hyland Bank"
            };

        /// <summary>
        /// When artificial businesses are disabled or experimental properties are off (default), these listings are unavailable.
        /// </summary>
        public static bool IsArtificialPropertyDisabled(string businessName)
        {
            if (!ArtificialBusinessFeature.IsEnabled) return true;
            if (string.IsNullOrWhiteSpace(businessName)) return false;
            if (!NoInteriorBusinessNames.Contains(businessName.Trim())) return false;
            return !MoreNPCsPreferences.ExperimentalProperties.Value;
        }
    }
}
