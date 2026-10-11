namespace MoreNPCs.Utils
{
    /// <summary>
    /// Master gate for the manager artificial-business pipeline (RE whiteboard listings, laundering, passive income).
    /// </summary>
    public static class ArtificialBusinessFeature
    {
        /// <summary>When false (default), artificial businesses are unavailable everywhere in the mod.</summary>
        public static bool IsEnabled =>
            MoreNPCsPreferences.Registered && MoreNPCsPreferences.Manager_ArtificialBusinessesEnabled.Value;

        public static bool IsBusinessUnavailable(string businessName) =>
            !IsEnabled || ExperimentalArtificialProperties.IsArtificialPropertyDisabled(businessName);
    }
}
