using System.Reflection;
using FishNet.Object;
using HarmonyLib;
using MoreNPCs.Utils;
using ScheduleOne.Vehicles;

namespace MoreNPCs.Patches
{
    [HarmonyPatch]
    internal static class PoliceVehicleCheckpointSearchPatch
    {
        [HarmonyTargetMethod]
        private static MethodBase TargetMethod() =>
            AccessTools.Method("ScheduleOne.NPCs.Behaviour.CheckpointBehaviour:StartSearch");

        [HarmonyPrefix]
        private static bool Prefix(NetworkObject targetVehicle)
        {
            if (targetVehicle == null)
                return true;

            var vehicle = targetVehicle.GetComponent<LandVehicle>();
            if (vehicle == null)
                return true;

            return !PoliceVehicleHelper.IsPoliceVehicleCode(vehicle.VehicleCode);
        }
    }
}
