using System.Reflection;
using HarmonyLib;
using MoreNPCs.Utils;
using ScheduleOne.PlayerScripts;

namespace MoreNPCs.Patches
{
    [HarmonyPatch]
    internal static class PoliceVehicleCurfewUpdatePatch
    {
        private static MethodInfo? _removeCurfewFlag;
        private static FieldInfo? _playerField;

        [HarmonyTargetMethod]
        private static MethodBase TargetMethod() =>
            AccessTools.Method("ScheduleOne.Vision.PlayerVisibility:Update");

        [HarmonyPostfix]
        private static void Postfix(object __instance)
        {
            if (__instance == null || !PoliceVehicleHelper.IsLocalPlayerInPoliceVehicle())
                return;

            if (!IsLocalPlayerVisibility(__instance))
                return;

            _removeCurfewFlag ??= __instance.GetType().GetMethod(
                "RemoveFlag_DisobeyingCurfew",
                BindingFlags.Instance | BindingFlags.NonPublic);
            _removeCurfewFlag?.Invoke(__instance, null);
        }

        private static bool IsLocalPlayerVisibility(object visibility)
        {
            var local = Player.Local;
            if (local == null)
                return false;

            _playerField ??= visibility.GetType().GetField(
                "player",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            var owner = _playerField?.GetValue(visibility) as Player;
            return owner != null && owner == local;
        }
    }

    [HarmonyPatch]
    internal static class PoliceVehicleCurfewAddPatch
    {
        private static FieldInfo? _playerField;

        [HarmonyTargetMethod]
        private static MethodBase TargetMethod() =>
            AccessTools.Method("ScheduleOne.Vision.PlayerVisibility:AddFlag_DisobeyingCurfew");

        [HarmonyPrefix]
        private static bool Prefix(object __instance)
        {
            if (__instance == null)
                return true;

            if (!IsLocalPlayerVisibility(__instance))
                return true;

            return !PoliceVehicleHelper.IsLocalPlayerInPoliceVehicle();
        }

        private static bool IsLocalPlayerVisibility(object visibility)
        {
            var local = Player.Local;
            if (local == null)
                return false;

            _playerField ??= visibility.GetType().GetField(
                "player",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            var owner = _playerField?.GetValue(visibility) as Player;
            return owner != null && owner == local;
        }
    }
}
