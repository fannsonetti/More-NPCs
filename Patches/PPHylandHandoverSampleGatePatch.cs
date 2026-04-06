using System;
using System.Reflection;
using HarmonyLib;
using MelonLoader;
using MoreNPCs.Utils;
using ScheduleOne.Economy;
using ScheduleOne.UI.Handover;

namespace MoreNPCs.Patches
{
    /// <summary>
    /// When the handover screen would allow Done with no other error, block P.P. Hyland free samples
    /// until meth unit total ≥ <see cref="PpHylandSampleDifficulty.RequiredMethUnits"/> (same rules as <see cref="PpHylandSampleDifficulty.MeetsHylandMethQuantityRequirement"/>).
    /// </summary>
    [HarmonyPatch]
    internal static class PPHylandHandoverGetErrorPatch
    {
        private const string QuantityMessage =
            "P.P. expects 20 meth units for a sample (baggie 1, jar 5, brick 20 per item).";

        private static MethodBase? _getError;

        private static bool Prepare()
        {
            for (var ty = typeof(HandoverScreen); ty != null; ty = ty.BaseType)
            {
                foreach (var m in ty.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                {
                    if (m.Name != "GetError") continue;
                    var ps = m.GetParameters();
                    if (ps.Length != 1 || !ps[0].ParameterType.IsByRef || ps[0].ParameterType.GetElementType() != typeof(string))
                        continue;
                    _getError = m;
                    return true;
                }
            }

            MelonLogger.Warning("[MoreNPCs] PPHyland: HandoverScreen.GetError(ref string) not found — handover gate disabled.");
            return false;
        }

        private static MethodBase TargetMethod() => _getError!;

        private static void Postfix(HandoverScreen __instance, ref string err, ref bool __result)
        {
            try
            {
                if (__result) return;

                PPHylandHandoverScreenHelper.EnsureCustomerCache(__instance);

                if (!PPHylandHandoverScreenHelper.IsSampleMode(__instance)) return;

                var customer = PPHylandHandoverScreenHelper.TryGetCustomer(__instance);
                if (customer == null || !PpHylandSampleDifficulty.IsPpHylandCustomer(customer)) return;

                var items = PPHylandHandoverScreenHelper.GetDistinctHandoverItems(__instance);
                if (items.Count == 0) return;

                if (!PpHylandSampleDifficulty.SampleContainsOnlyMethamphetamine(items)) return;
                if (PpHylandSampleDifficulty.MeetsHylandMethQuantityRequirement(items)) return;

                __result = true;
                err = QuantityMessage;
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[MoreNPCs] PPHyland HandoverScreen.GetError postfix: {ex.Message}");
            }
            finally
            {
                PPHylandHandoverWarning.SetActiveScreen(__instance);
                PPHylandHandoverWarning.SyncWarning(__instance);
            }
        }
    }
}
