using MoreNPCs.Utils;
using UnityEngine;

namespace MoreNPCs.Distributor
{
    public static class DistributorConfig
    {
        public const int UnlockDealerThreshold = 15;

        /// <summary>Default chance (0–1) each sleep to schedule a wholesale meet after unlock.</summary>
        public const float DefaultVisitChancePerSleep = 0.30f;
        public const int DefaultPityDaysWithoutEvent = 5;
        public const int DefaultWindowMinutes = 300;
        public const int DefaultBulkCocaineMinUnits = 300;
        public const int DefaultBulkCocaineMaxUnits = 600;
        public const int DefaultPremiumMinUnits = 20;
        public const int DefaultPremiumMaxUnits = 100;
        public const int OrderQuantityStep = 20;
        public const float DefaultPremiumOrderChance = 0.20f;
        public const float DefaultTier1Multiplier = 0.50f;
        public const float DefaultTier2Multiplier = 0.20f;
        public const int DefaultDealDeadlineDays = 3;
        public const float DefaultFullPriceMultiplier = 1.0f;
        /// <summary>Per missing required premium effect, payout is multiplied by this (30% off).</summary>
        public const float DefaultMissingPropertyMultiplier = 0.70f;
        /// <summary>If a forbidden premium effect is present, payout is multiplied by this (50% penalty).</summary>
        public const float DefaultForbiddenPropertyMultiplier = 0.50f;

        public static int BulkCocaineMinUnits =>
            MoreNPCsPreferences.Registered
                ? MoreNPCsPreferences.Distributor_BulkCocaineMinUnits.Value
                : DefaultBulkCocaineMinUnits;

        public static int BulkCocaineMaxUnits =>
            MoreNPCsPreferences.Registered
                ? MoreNPCsPreferences.Distributor_BulkCocaineMaxUnits.Value
                : DefaultBulkCocaineMaxUnits;

        public static int PremiumMinUnits =>
            MoreNPCsPreferences.Registered
                ? MoreNPCsPreferences.Distributor_PremiumMinUnits.Value
                : DefaultPremiumMinUnits;

        public static int PremiumMaxUnits =>
            MoreNPCsPreferences.Registered
                ? MoreNPCsPreferences.Distributor_PremiumMaxUnits.Value
                : DefaultPremiumMaxUnits;

        public static float PremiumOrderChance =>
            MoreNPCsPreferences.Registered
                ? MoreNPCsPreferences.Distributor_PremiumOrderChance.Value
                : DefaultPremiumOrderChance;

        public static float FullPriceMultiplier =>
            MoreNPCsPreferences.Registered
                ? MoreNPCsPreferences.Distributor_FullPriceMultiplier.Value
                : DefaultFullPriceMultiplier;

        public static float Tier1Multiplier =>
            MoreNPCsPreferences.Registered
                ? MoreNPCsPreferences.Distributor_Tier1Multiplier.Value
                : DefaultTier1Multiplier;

        public static float Tier2Multiplier =>
            MoreNPCsPreferences.Registered
                ? MoreNPCsPreferences.Distributor_Tier2Multiplier.Value
                : DefaultTier2Multiplier;

        /// <summary>Chance each sleep that Grant texts with a meet location (MelonPreferences override).</summary>
        public static float VisitChancePerSleep =>
            MoreNPCsPreferences.Registered
                ? MoreNPCsPreferences.Distributor_VisitChancePerSleep.Value
                : DefaultVisitChancePerSleep;

        public static int PityDaysWithoutEvent =>
            MoreNPCsPreferences.Registered
                ? MoreNPCsPreferences.Distributor_PityDaysWithoutEvent.Value
                : DefaultPityDaysWithoutEvent;

        public static int WindowMinutes =>
            MoreNPCsPreferences.Registered
                ? MoreNPCsPreferences.Distributor_WindowMinutes.Value
                : DefaultWindowMinutes;

        public static int DealDeadlineDays =>
            MoreNPCsPreferences.Registered
                ? MoreNPCsPreferences.Distributor_DealDeadlineDays.Value
                : DefaultDealDeadlineDays;

        /// <summary>Off-map hide spot when no active run.</summary>
        public static readonly Vector3 HiddenPosition = new Vector3(0f, -500f, 0f);

        public static string DropLocationLabel => DistributorTrawlerDrop.DisplayName;
        public static Vector3 DropPosition => DistributorTrawlerDrop.Waypoint;

        /// <summary>Docks waterfront spawn for the distributor trawler (S1MAPI GLB).</summary>
        public static readonly Vector3 TrawlerPosition = new Vector3(-150.4941f, -8f, -38.1404f);
        /// <summary>Local euler on the trawler root (GLB loads at 0,0,0).</summary>
        public static readonly Vector3 TrawlerEuler = new Vector3(0f, 60f, 0f);
        public const float TrawlerUniformScale = 60f;
        /// <summary>On-land pier locker — reachable while the trawler stays offshore.</summary>
        public static readonly Vector3 PierDropPosition = new Vector3(-101.2265f, -1.485f, -42.492f);
        public static readonly Vector3 PierDropEuler = Vector3.zero;
        public static readonly Vector3 PierLadderPosition = new Vector3(-101.4105f, -8.089f, -38.9659f);
        public static readonly Vector3 PierLadderEuler = new Vector3(0f, 195f, 0f);

        /// <summary>Legacy meet position alias.</summary>
        public static readonly Vector3 MeetPosition = new Vector3(-39.30f, -0.975f, 167.35f);
        public static readonly Vector3 DepartPosition = new Vector3(-35f, -0.975f, 175f);
        public const float MeetFaceYaw = 180f;

        public static float MissingPropertyMultiplier => DefaultMissingPropertyMultiplier;
        public static float ForbiddenPropertyMultiplier => DefaultForbiddenPropertyMultiplier;
    }
}
