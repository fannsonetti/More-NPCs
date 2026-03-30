using MelonLoader;
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

        public static void Register()
        {
            CategoryManager = MelonPreferences.CreateCategory(
                "MoreNPCs_Manager",
                "Manager");
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
            Registered = true;
        }
    }
}
