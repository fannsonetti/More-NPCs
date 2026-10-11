using System.Reflection;
using HarmonyLib;
using MoreNPCs.Utils;
using ScheduleOne.Police;
using ScheduleOne.Vehicles;
using UnityEngine;

namespace MoreNPCs.Patches
{
    /// <summary>Opens checkpoint gates for local player in mod police vehicles after vanilla gate logic runs.</summary>
    [HarmonyPatch(typeof(RoadCheckpoint), "Update")]
    internal static class PoliceVehicleCheckpointGatePatch
    {
        private static FieldInfo? _searchArea1;
        private static FieldInfo? _searchArea2;
        private static FieldInfo? _immediateDetector;
        private static FieldInfo? _vehiclesField;

        [HarmonyPostfix]
        private static void Postfix(RoadCheckpoint __instance)
        {
            if (__instance == null || __instance.ActivationState == RoadCheckpoint.ECheckpointState.Disabled)
                return;

            if (!PoliceVehicleHelper.TryGetLocalPoliceVehicle(out var vehicle) || vehicle == null)
                return;

            if (!IsVehicleAtCheckpoint(__instance, vehicle))
                return;

            __instance.SetGate1Open(true);
            __instance.SetGate2Open(true);
        }

        private static bool IsVehicleAtCheckpoint(RoadCheckpoint checkpoint, LandVehicle vehicle)
        {
            if (ContainsVehicle(GetDetector(checkpoint, ref _immediateDetector, "ImmediateVehicleDetector"), vehicle))
                return true;

            if (ContainsVehicle(GetDetector(checkpoint, ref _searchArea1, "SearchArea1"), vehicle))
                return true;

            return ContainsVehicle(GetDetector(checkpoint, ref _searchArea2, "SearchArea2"), vehicle);
        }

        private static object? GetDetector(RoadCheckpoint checkpoint, ref FieldInfo? cache, string name)
        {
            cache ??= typeof(RoadCheckpoint).GetField(name, BindingFlags.Public | BindingFlags.Instance);
            return cache?.GetValue(checkpoint);
        }

        private static bool ContainsVehicle(object? detector, LandVehicle vehicle)
        {
            if (detector == null)
                return false;

            _vehiclesField ??= detector.GetType().GetField("vehicles", BindingFlags.Public | BindingFlags.Instance);
            if (_vehiclesField?.GetValue(detector) is not System.Collections.IList vehicles)
                return false;

            foreach (var entry in vehicles)
            {
                if (ReferenceEquals(entry, vehicle))
                    return true;
            }

            return false;
        }
    }
}
