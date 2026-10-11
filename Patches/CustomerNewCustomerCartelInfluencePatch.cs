using HarmonyLib;
using MoreNPCs.Utils;
using ScheduleOne.Cartel;
using ScheduleOne.Map;
using UnityEngine;

namespace MoreNPCs.Patches
{
    /// <summary>
    /// Scales down the cartel influence hit when a new customer unlocks (vanilla: -0.075).
    /// Transpiling <c>Customer.OnCustomerUnlocked</c> does not work on Il2Cpp (native invoke stub);
    /// -0.075 is only used for that unlock path in game code.
    /// </summary>
    [HarmonyPatch(typeof(CartelInfluence), nameof(CartelInfluence.ChangeInfluence), new[] { typeof(EMapRegion), typeof(float) })]
    internal static class CustomerNewCustomerCartelInfluencePatch
    {
        private const float VanillaNewCustomerDelta = -0.075f;
        private const float ReducedNewCustomerDelta = -0.025f;

        [HarmonyPrefix]
        private static void Prefix(ref float amount)
        {
            if (!MoreNPCsPreferences.RegionChanges_ReducedCartelInfluenceOnNewCustomers.Value)
                return;
            if (Mathf.Approximately(amount, VanillaNewCustomerDelta))
                amount = ReducedNewCustomerDelta;
        }
    }
}
