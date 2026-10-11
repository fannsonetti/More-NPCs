using System;
using System.Collections;
using HarmonyLib;
using MelonLoader;
using MoreNPCs.Utils;
using ScheduleOne.Vehicles;

namespace MoreNPCs.Patches
{
    [HarmonyPatch(typeof(LandVehicle), nameof(LandVehicle.Load))]
    internal static class PoliceShitboxTrunkLoadPatch
    {
        [HarmonyPostfix]
        private static void Postfix(LandVehicle __instance)
        {
            PoliceShitboxTrunkPatch.ScheduleTrunkFix(__instance);
        }
    }

    [HarmonyPatch(typeof(VehicleManager), nameof(VehicleManager.SpawnAndReturnVehicle))]
    internal static class PoliceShitboxTrunkSpawnPatch
    {
        [HarmonyPostfix]
        private static void Postfix(LandVehicle __result)
        {
            PoliceShitboxTrunkPatch.ScheduleTrunkFix(__result);
        }
    }

    internal static class PoliceShitboxTrunkPatch
    {
        internal static void ScheduleTrunkFix(LandVehicle? vehicle)
        {
            if (vehicle == null) return;
            MelonCoroutines.Start(DeferredTrunkFix(vehicle));
        }

        private static IEnumerator DeferredTrunkFix(LandVehicle vehicle)
        {
            yield return null;
            try
            {
                PoliceVehicleHelper.TryFixTrunkOnVehicle(vehicle);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"PoliceShitboxTrunkPatch: {ex.Message}");
            }
        }
    }
}
