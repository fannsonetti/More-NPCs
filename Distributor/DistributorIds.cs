namespace MoreNPCs.Distributor
{
    public static class DistributorIds
    {
        public const string Grant = "grant_ellison";
        public const string DisplayName = "Richard Roe";
        public const string ContactFirstName = "Richard";
        public const string ContactLastName = "Roe";

        /// <summary>Old prefab id — still recognized for saves / unlock tracking.</summary>
        public const string LegacyVinny = "vinny_harbor";

        public static bool MatchesNpcId(string? id) =>
            !string.IsNullOrEmpty(id)
            && (id.Equals(Grant, System.StringComparison.OrdinalIgnoreCase)
                || id.Equals(LegacyVinny, System.StringComparison.OrdinalIgnoreCase));
    }
}
