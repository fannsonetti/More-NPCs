using MelonLoader;
using MoreNPCs.Distributor;
using MoreNPCs.Manager;
namespace MoreNPCs.Utils
{
    /// <summary>
    /// MelonLoader preferences grouped by feature area. Edit values in UserData/MelonPreferences.cfg (or the in-game preferences UI when available).
    /// </summary>
    public static class MoreNPCsPreferences
    {
        /// <summary>True after <see cref="Register"/> completes; before that, economy tiers fall back to built-in defaults.</summary>
        public static bool Registered { get; private set; }

        public static MelonPreferences_Category CategoryManager { get; private set; } = null!;
        public static MelonPreferences_Category CategorySupervisor { get; private set; } = null!;
        public static MelonPreferences_Category CategoryEconomyTiers { get; private set; } = null!;
        public static MelonPreferences_Category CategorySupervisorAutomation { get; private set; } = null!;
        public static MelonPreferences_Category CategoryReOffice { get; private set; } = null!;
        public static MelonPreferences_Category CategoryBusinessEconomy { get; private set; } = null!;
        public static MelonPreferences_Category CategoryExperimental { get; private set; } = null!;
        public static MelonPreferences_Category CategoryRegionChanges { get; private set; } = null!;
        public static MelonPreferences_Category CategoryPhotoSession { get; private set; } = null!;
        public static MelonPreferences_Category CategoryDebug { get; private set; } = null!;
        public static MelonPreferences_Category CategoryDistributor { get; private set; } = null!;

        public static MelonPreferences_Entry<bool> RegionChanges_ReducedCartelInfluenceOnNewCustomers { get; private set; } = null!;
        public static MelonPreferences_Entry<bool> RegionChanges_RequireRankAndCartelForNextRegion { get; private set; } = null!;

        public static MelonPreferences_Entry<bool> Manager_ArtificialBusinessesEnabled { get; private set; } = null!;
        public static MelonPreferences_Entry<int> Manager_MaxAssignedBusinesses { get; private set; } = null!;
        public static MelonPreferences_Entry<float> Manager_LaunderCutPercent { get; private set; } = null!;

        public static MelonPreferences_Entry<int> Supervisor_MaxAssignedDealers { get; private set; } = null!;
        public static MelonPreferences_Entry<float> Supervisor_MinCashToCollect { get; private set; } = null!;
        public static MelonPreferences_Entry<int> Supervisor_LowStockDrugThreshold { get; private set; } = null!;
        public static MelonPreferences_Entry<float> Supervisor_WalkTimeoutSeconds { get; private set; } = null!;
        public static MelonPreferences_Entry<float> Supervisor_ArrivalRadius { get; private set; } = null!;
        public static MelonPreferences_Entry<float> Supervisor_CollectCutPercent { get; private set; } = null!;

        public static MelonPreferences_Entry<float> Tier_Laundromat_DailyPassive { get; private set; } = null!;
        public static MelonPreferences_Entry<float> Tier_PostOffice_DailyPassive { get; private set; } = null!;
        public static MelonPreferences_Entry<float> Tier_CarWash_DailyPassive { get; private set; } = null!;
        public static MelonPreferences_Entry<float> Tier_TacoTicklers_DailyPassive { get; private set; } = null!;

        public static MelonPreferences_Entry<float> SupervisorChain_IdleCheckInterval { get; private set; } = null!;
        public static MelonPreferences_Entry<float> SupervisorChain_ChainDelaySeconds { get; private set; } = null!;
        public static MelonPreferences_Entry<float> SupervisorChain_InitialStartDelay { get; private set; } = null!;
        public static MelonPreferences_Entry<float> SupervisorIdle_ArriveThreshold { get; private set; } = null!;

        public static MelonPreferences_Entry<float> ReOffice_CheckIntervalSeconds { get; private set; } = null!;
        public static MelonPreferences_Entry<bool> ExperimentalProperties { get; private set; } = null!;

        public static MelonPreferences_Entry<float> PhotoSession_OriginX { get; private set; } = null!;
        public static MelonPreferences_Entry<float> PhotoSession_OriginY { get; private set; } = null!;
        public static MelonPreferences_Entry<float> PhotoSession_OriginZ { get; private set; } = null!;
        public static MelonPreferences_Entry<int> PhotoSession_Columns { get; private set; } = null!;
        public static MelonPreferences_Entry<float> PhotoSession_Spacing { get; private set; } = null!;
        public static MelonPreferences_Entry<float> PhotoSession_RowSpacing { get; private set; } = null!;
        public static MelonPreferences_Entry<float> PhotoSession_FaceYaw { get; private set; } = null!;
        public static MelonPreferences_Entry<string> PhotoSession_RegionFilter { get; private set; } = null!;

        public static MelonPreferences_Entry<bool> DebugMenu_Enabled { get; private set; } = null!;

        public static MelonPreferences_Entry<float> Distributor_VisitChancePerSleep { get; private set; } = null!;
        public static MelonPreferences_Entry<int> Distributor_PityDaysWithoutEvent { get; private set; } = null!;
        public static MelonPreferences_Entry<int> Distributor_WindowMinutes { get; private set; } = null!;
        public static MelonPreferences_Entry<int> Distributor_BulkCocaineMinUnits { get; private set; } = null!;
        public static MelonPreferences_Entry<int> Distributor_BulkCocaineMaxUnits { get; private set; } = null!;
        public static MelonPreferences_Entry<int> Distributor_PremiumMinUnits { get; private set; } = null!;
        public static MelonPreferences_Entry<int> Distributor_PremiumMaxUnits { get; private set; } = null!;
        public static MelonPreferences_Entry<float> Distributor_PremiumOrderChance { get; private set; } = null!;
        public static MelonPreferences_Entry<float> Distributor_Tier1Multiplier { get; private set; } = null!;
        public static MelonPreferences_Entry<float> Distributor_Tier2Multiplier { get; private set; } = null!;
        public static MelonPreferences_Entry<int> Distributor_DealDeadlineDays { get; private set; } = null!;
        public static MelonPreferences_Entry<float> Distributor_FullPriceMultiplier { get; private set; } = null!;

        public static void Register()
        {
            CategoryManager = MelonPreferences.CreateCategory(
                "MoreNPCs_Manager",
                "Manager");
            Manager_ArtificialBusinessesEnabled = CategoryManager.CreateEntry(
                "ArtificialBusinessesEnabled",
                false,
                "Enable Thomas Ashford's artificial business listings, RE whiteboard cards, laundering, and passive income.");
            Manager_MaxAssignedBusinesses = CategoryManager.CreateEntry(
                "MaxAssignedBusinesses",
                4,
                "Max businesses one manager can run.");
            Manager_LaunderCutPercent = CategoryManager.CreateEntry(
                "LaunderCutPercent",
                0.10f,
                "Manager laundering fee (0.10 = 10%).");

            CategorySupervisor = MelonPreferences.CreateCategory(
                "MoreNPCs_Supervisor",
                "Supervisors");
            Supervisor_MaxAssignedDealers = CategorySupervisor.CreateEntry(
                "MaxAssignedDealersPerSupervisor",
                6,
                "Max dealers per supervisor.");
            Supervisor_MinCashToCollect = CategorySupervisor.CreateEntry(
                "MinCashToCollect",
                2000f,
                "Min cash at a dealer before collection.");
            Supervisor_LowStockDrugThreshold = CategorySupervisor.CreateEntry(
                "LowStockDrugThreshold",
                20,
                "Total drug count below this = low stock.");
            Supervisor_WalkTimeoutSeconds = CategorySupervisor.CreateEntry(
                "WalkTimeoutSeconds",
                120f,
                "Walk timeout when visiting dealers.");
            Supervisor_ArrivalRadius = CategorySupervisor.CreateEntry(
                "ArrivalRadius",
                2.5f,
                "Arrival distance at dealers (m).");
            Supervisor_CollectCutPercent = CategorySupervisor.CreateEntry(
                "CollectCutPercent",
                0.10f,
                "Supervisor cut on collected cash (0.10 = 10%).");

            CategoryEconomyTiers = MelonPreferences.CreateCategory(
                "MoreNPCs_EconomyTiers",
                "Daily income (T1–T4)");
            Tier_Laundromat_DailyPassive = CategoryEconomyTiers.CreateEntry("Tier_Laundromat_DailyPassive", 200f, "T1 ($/day, once per tier among assignments).");
            Tier_PostOffice_DailyPassive = CategoryEconomyTiers.CreateEntry("Tier_PostOffice_DailyPassive", 400f, "T2 ($/day, once per tier among assignments).");
            Tier_CarWash_DailyPassive = CategoryEconomyTiers.CreateEntry("Tier_CarWash_DailyPassive", 600f, "T3 ($/day, once per tier among assignments).");
            Tier_TacoTicklers_DailyPassive = CategoryEconomyTiers.CreateEntry("Tier_TacoTicklers_DailyPassive", 800f, "T4 ($/day, once per tier among assignments).");

            CategorySupervisorAutomation = MelonPreferences.CreateCategory(
                "MoreNPCs_SupervisorAutomation",
                "Supervisor chains");
            SupervisorChain_IdleCheckInterval = CategorySupervisorAutomation.CreateEntry("IdleCheckInterval", 45f, "Idle check interval (s).");
            SupervisorChain_ChainDelaySeconds = CategorySupervisorAutomation.CreateEntry("ChainDelaySeconds", 5f, "Delay after dialogue before chain.");
            SupervisorChain_InitialStartDelay = CategorySupervisorAutomation.CreateEntry("InitialStartDelay", 10f, "Delay after NPC init.");
            SupervisorIdle_ArriveThreshold = CategorySupervisorAutomation.CreateEntry("IdleArriveThreshold", 2f, "Home arrival distance (m).");

            CategoryReOffice = MelonPreferences.CreateCategory(
                "MoreNPCs_REOffice",
                "RE office");
            ReOffice_CheckIntervalSeconds = CategoryReOffice.CreateEntry(
                "PropertyUnlockCheckInterval",
                12f,
                "Whiteboard / unlock poll (s).");

            CategoryRegionChanges = MelonPreferences.CreateCategory(
                "MoreNPCs_RegionChanges",
                "Region Changes");
            RegionChanges_ReducedCartelInfluenceOnNewCustomers = CategoryRegionChanges.CreateEntry(
                "ReducedCartelInfluenceOnNewCustomers",
                true,
                "Cartel influence on new customers: use 0.025 penalty (instead of vanilla 0.075) when the cartel is hostile.");
            RegionChanges_RequireRankAndCartelForNextRegion = CategoryRegionChanges.CreateEntry(
                "RequireRankAndCartelForNextRegion",
                true,
                "Region unlocks: require both enough rank and low enough cartel influence in the previous region (300/1000 scale).");

            CategoryExperimental = MelonPreferences.CreateCategory(
                "MoreNPCs_Experimental",
                "Experimental");
            ExperimentalProperties = CategoryExperimental.CreateEntry(
                "ExperimentalProperties",
                false,
                "Extra no-interior listings (Chinese Restaurant, Thompson Construction, Koyama Chemical, Hyland Bank).");

            CategoryBusinessEconomy = MelonPreferences.CreateCategory(
                "MoreNPCs_BusinessEconomy",
                "Business Prices");
            ArtificialBusinessCatalog.RegisterBusinessEconomyEntries(CategoryBusinessEconomy);

            CategoryPhotoSession = MelonPreferences.CreateCategory(
                "MoreNPCs_PhotoSession",
                "Photo Session (debug tab)");
            PhotoSession_OriginX = CategoryPhotoSession.CreateEntry("OriginX", -50f, "Grid origin X (world). Set in the F6 menu or here.");
            PhotoSession_OriginY = CategoryPhotoSession.CreateEntry("OriginY", -2.935f, "Grid origin Y (world).");
            PhotoSession_OriginZ = CategoryPhotoSession.CreateEntry("OriginZ", 170f, "Grid origin Z (world).");
            PhotoSession_Columns = CategoryPhotoSession.CreateEntry("Columns", 6, "NPCs per row.");
            PhotoSession_Spacing = CategoryPhotoSession.CreateEntry("ColumnSpacing", 1.5f, "Metres between columns.");
            PhotoSession_RowSpacing = CategoryPhotoSession.CreateEntry("RowSpacing", 1.5f, "Metres between rows (rows extend -Z).");
            PhotoSession_FaceYaw = CategoryPhotoSession.CreateEntry("FaceYaw", 180f, "World Y rotation all NPCs face after teleport.");
            PhotoSession_RegionFilter = CategoryPhotoSession.CreateEntry(
                "RegionFilter",
                "All",
                "All | Northtown | Westville | Downtown | Docks | Suburbia | Uptown — only mod NPCs in that region.");

            CategoryDebug = MelonPreferences.CreateCategory(
                "MoreNPCs_Debug",
                "Debug Menu");
            DebugMenu_Enabled = CategoryDebug.CreateEntry(
                "Enabled",
                false,
                "Enable the F6 MoreNPCs debug menu (photo tools, distributor cheats, etc.).");

            CategoryDistributor = MelonPreferences.CreateCategory(
                "MoreNPCs_Distributor",
                "Distributor (Richard Roe)");
            Distributor_VisitChancePerSleep = CategoryDistributor.CreateEntry(
                "VisitChancePerSleep",
                DistributorConfig.DefaultVisitChancePerSleep,
                "Chance (0–1) each sleep that a wholesale deal text is sent.");
            Distributor_PityDaysWithoutEvent = CategoryDistributor.CreateEntry(
                "PityDaysWithoutEvent",
                DistributorConfig.DefaultPityDaysWithoutEvent,
                "Guarantee a deal after this many sleeps without one.");
            Distributor_DealDeadlineDays = CategoryDistributor.CreateEntry(
                "DealDeadlineDays",
                DistributorConfig.DefaultDealDeadlineDays,
                "In-game days to complete a drop deal after the text arrives.");
            Distributor_WindowMinutes = CategoryDistributor.CreateEntry(
                "WindowMinutes",
                DistributorConfig.DefaultWindowMinutes,
                "Legacy setting (unused by drop-deal flow).");
            Distributor_BulkCocaineMinUnits = CategoryDistributor.CreateEntry(
                "BulkCocaineMinUnits",
                DistributorConfig.DefaultBulkCocaineMinUnits,
                "Bulk order minimum (cocaine baseline; other drugs scale up).");
            Distributor_BulkCocaineMaxUnits = CategoryDistributor.CreateEntry(
                "BulkCocaineMaxUnits",
                DistributorConfig.DefaultBulkCocaineMaxUnits,
                "Bulk order maximum (cocaine baseline; weed 1.5×, meth 1.2×, shrooms 1.3×).");
            Distributor_PremiumMinUnits = CategoryDistributor.CreateEntry(
                "PremiumMinUnits",
                DistributorConfig.DefaultPremiumMinUnits,
                "Premium property-order minimum units.");
            Distributor_PremiumMaxUnits = CategoryDistributor.CreateEntry(
                "PremiumMaxUnits",
                DistributorConfig.DefaultPremiumMaxUnits,
                "Premium property-order maximum units.");
            Distributor_PremiumOrderChance = CategoryDistributor.CreateEntry(
                "PremiumOrderChance",
                DistributorConfig.DefaultPremiumOrderChance,
                "Chance (0–1) each week that Grant requests a premium property order instead of bulk.");
            Distributor_Tier1Multiplier = CategoryDistributor.CreateEntry(
                "Tier1Multiplier",
                DistributorConfig.DefaultTier1Multiplier,
                "Bulk payout fraction for the first X units of the weekly drug (0.50 = 50%).");
            Distributor_Tier2Multiplier = CategoryDistributor.CreateEntry(
                "Tier2Multiplier",
                DistributorConfig.DefaultTier2Multiplier,
                "Payout fraction for extra units beyond the order (0.20 = 20%).");
            Distributor_FullPriceMultiplier = CategoryDistributor.CreateEntry(
                "FullPriceMultiplier",
                DistributorConfig.DefaultFullPriceMultiplier,
                "Premium property-order payout fraction (1.0 = full market price).");

            Registered = true;
        }
    }
}
