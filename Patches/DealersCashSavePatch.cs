using System.Reflection;
using HarmonyLib;
using MelonLoader;
using MoreNPCs.Persistence;

namespace MoreNPCs.Patches
{
    [HarmonyPatch]
    internal static class DealersCashSavePatch
    {
        [HarmonyTargetMethod]
        private static MethodBase? TargetMethod()
        {
            return AccessTools.Method("ScheduleOne.Persistence.SaveManager:Save")
                   ?? AccessTools.Method("ScheduleOne.Persistence.SaveManager:SaveGame");
        }

        [HarmonyPrefix]
        private static void Prefix()
        {
            try
            {
                DealersCashSave.CaptureFromWorld();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Warning($"DealersCashSavePatch: {ex.Message}");
            }
        }
    }
}
