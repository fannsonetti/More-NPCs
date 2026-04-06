using System;
using HarmonyLib;
using MelonLoader;
using ScheduleOne.Product;
using UnityEngine;

namespace MoreNPCs.Patches
{
    /// <summary>
    /// <see cref="HandoverScreen.Open"/> (free sample path) calls <see cref="DrugTypeMethods.GetColor"/>; some customer/mod
    /// combinations can NRE inside game code. Suppress and return white so <see cref="ScheduleOne.Economy.Customer.SampleAccepted"/> can finish.
    /// </summary>
    [HarmonyPatch(typeof(DrugTypeMethods), nameof(DrugTypeMethods.GetColor), new[] { typeof(EDrugType) })]
    internal static class DrugTypeMethodsGetColorPatch
    {
        [HarmonyFinalizer]
        private static Exception Finalizer(Exception? __exception, ref Color __result)
        {
            if (__exception == null) return null;

            MelonLogger.Warning(
                $"[MoreNPCs] DrugTypeMethods.GetColor suppressed: {__exception.GetType().Name}: {__exception.Message}");
            __result = Color.white;
            return null;
        }
    }
}
