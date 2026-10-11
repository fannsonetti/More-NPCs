using HarmonyLib;
using MoreNPCs.Utils;
using ScheduleOne.UI;
using ScheduleOne.Vehicles;

namespace MoreNPCs.Patches
{
    [HarmonyPatch(typeof(VehicleCanvas), "VehicleEntered")]
    internal static class PoliceSirenPromptVehiclePatch
    {
        [HarmonyPostfix]
        private static void Postfix(LandVehicle veh)
        {
            if (veh == null || !veh.LocalPlayerIsDriver)
                return;

            if (!PoliceVehicleHelper.IsPoliceVehicleCode(veh.VehicleCode))
                return;

            PoliceVehicleSirenPrompt.EnsureForVehicle(veh);
        }
    }

    [HarmonyPatch(typeof(VehicleCanvas), "VehicleExited")]
    internal static class PoliceSirenPromptVehicleExitPatch
    {
        [HarmonyPostfix]
        private static void Postfix()
        {
            PoliceVehicleSirenPrompt.Hide();
        }
    }
}
