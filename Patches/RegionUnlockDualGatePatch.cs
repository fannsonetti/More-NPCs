using System;
using System.Diagnostics;
using HarmonyLib;
using MoreNPCs.Utils;
using ScheduleOne.Cartel;
using ScheduleOne.DevUtilities;
using ScheduleOne.Levelling;
using ScheduleOne.Map;
using ScheduleOne.UI;

namespace MoreNPCs.Patches
{
    /// <summary>
    /// Shared bypass so <see cref="MapRegionData.SetUnlocked"/> checks do not block intentional unlocks from our rank-up replacement.
    /// </summary>
    internal static class RegionUnlockDualGateBypass
    {
        [ThreadStatic]
        private static int _depth;

        internal static bool IsActive => _depth > 0;

        internal static void Enter()
        {
            _depth++;
        }

        internal static void Leave()
        {
            if (_depth > 0)
                _depth--;
        }
    }

    /// <summary>
    /// Replaces vanilla Westville-only rank unlock with a scan of all regions: unlock each locked region
    /// when rank meets its <see cref="MapRegionData.RankRequirement"/> and prior-region cartel influence is at or below 0.3.
    /// </summary>
    [HarmonyPatch(typeof(Map), "OnRankUp", new[] { typeof(FullRank), typeof(FullRank) })]
    internal static class MapOnRankUpDualGatePatch
    {
        [HarmonyPrefix]
        private static bool Prefix(Map __instance, FullRank old, FullRank newRank)
        {
            if (!MoreNPCsPreferences.RegionChanges_RequireRankAndCartelForNextRegion.Value)
                return true;

            try
            {
                RegionUnlockDualGateBypass.Enter();
                foreach (EMapRegion region in Enum.GetValues(typeof(EMapRegion)))
                {
                    var data = __instance.GetRegionData(region);
                    if (data.IsUnlocked)
                        continue;
                    if (newRank < data.RankRequirement)
                        continue;
                    if (!IsPreviousRegionInfluenceAtOrBelowThreshold(region))
                        continue;

                    data.SetUnlocked();
                    if (Singleton<RegionUnlockedCanvas>.Instance != null)
                        Singleton<RegionUnlockedCanvas>.Instance.QueueUnlocked(region);
                }
            }
            finally
            {
                RegionUnlockDualGateBypass.Leave();
            }

            return false;
        }

        private static bool IsPreviousRegionInfluenceAtOrBelowThreshold(EMapRegion regionToUnlock)
        {
            if (regionToUnlock == EMapRegion.Northtown)
                return true;
            var prev = regionToUnlock - 1;
            if (!NetworkSingleton<Cartel>.InstanceExists)
                return true;
            var inf = NetworkSingleton<Cartel>.Instance.Influence.GetInfluence(prev);
            return inf <= CartelInfluence.INFLUENCE_TO_UNLOCK_NEXT_REGION + 1e-4f;
        }
    }

    /// <summary>
    /// Cartel influence drop unlocks the next region only if rank meets that region's requirement.
    /// Save sync, map init, and console commands bypass this.
    /// </summary>
    [HarmonyPatch(typeof(MapRegionData), nameof(MapRegionData.SetUnlocked))]
    internal static class MapRegionDataSetUnlockedDualGatePatch
    {
        [HarmonyPrefix]
        private static bool Prefix(MapRegionData __instance)
        {
            if (!MoreNPCsPreferences.RegionChanges_RequireRankAndCartelForNextRegion.Value)
                return true;
            if (RegionUnlockDualGateBypass.IsActive)
                return true;
            if (__instance.IsUnlocked)
                return true;

            var source = ClassifyCaller();
            if (source == CallerKind.Bypass)
                return true;

            if (!NetworkSingleton<LevelManager>.InstanceExists)
                return true;

            var fullRank = NetworkSingleton<LevelManager>.Instance.GetFullRank();
            if (fullRank < __instance.RankRequirement)
                return false;

            if (source == CallerKind.CartelInfluenceChange)
                return true;

            return true;
        }

        private enum CallerKind
        {
            Unknown,
            Bypass,
            CartelInfluenceChange,
        }

        private static CallerKind ClassifyCaller()
        {
            var trace = new StackTrace(skipFrames: 2);
            for (var i = 0; i < Math.Min(trace.FrameCount, 24); i++)
            {
                var m = trace.GetFrame(i)?.GetMethod();
                if (m == null)
                    continue;
                var name = m.Name ?? "";
                var decl = m.DeclaringType?.FullName ?? "";

                if (decl.Contains("LevelManager", StringComparison.Ordinal) &&
                    (name.Contains("SetUnlockedRegions", StringComparison.Ordinal) || name.Contains("RpcLogic___SetUnlockedRegions", StringComparison.Ordinal)))
                    return CallerKind.Bypass;

                if (decl.Contains("Console", StringComparison.Ordinal) && name.Contains("SetUnlocked", StringComparison.Ordinal))
                    return CallerKind.Bypass;

                if (decl.Contains("ScheduleOne.Map.Map", StringComparison.Ordinal) && (name == "Awake" || name == "Start"))
                    return CallerKind.Bypass;

                if (decl.Contains("CartelInfluence", StringComparison.Ordinal) && name.Contains("RpcLogic___ChangeInfluence", StringComparison.Ordinal))
                    return CallerKind.CartelInfluenceChange;
            }

            return CallerKind.Unknown;
        }
    }
}
