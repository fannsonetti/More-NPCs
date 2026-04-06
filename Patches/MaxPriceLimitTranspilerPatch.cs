using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ScheduleOne.UI.Handover;
using ScheduleOne.UI.Phone;

namespace MoreNPCs.Patches
{
    /// <summary>
    /// The game's UI clamps prices with an inlined <c>9999f</c> constant; this transpiler replaces those loads with <c>9999999f</c>
    /// in handover and counteroffer flows.
    /// </summary>
    [HarmonyPatch]
    internal static class MaxPriceLimitTranspilerPatch
    {
        private const float OldLimit = 9999f;
        private const float NewLimit = 9999999f;

        static bool Prepare()
        {
            return AccessTools.Method(typeof(HandoverScreenPriceSelector), nameof(HandoverScreenPriceSelector.SetPrice)) != null
                && AccessTools.Method(typeof(CounterofferInterface), nameof(CounterofferInterface.Send)) != null
                && AccessTools.Method(typeof(CounterofferInterface), nameof(CounterofferInterface.ChangePrice)) != null
                && AccessTools.Method(typeof(CounterofferInterface), nameof(CounterofferInterface.PriceSubmitted)) != null;
        }

        static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(HandoverScreenPriceSelector), nameof(HandoverScreenPriceSelector.SetPrice))!;
            yield return AccessTools.Method(typeof(CounterofferInterface), nameof(CounterofferInterface.Send))!;
            yield return AccessTools.Method(typeof(CounterofferInterface), nameof(CounterofferInterface.ChangePrice))!;
            yield return AccessTools.Method(typeof(CounterofferInterface), nameof(CounterofferInterface.PriceSubmitted))!;
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldc_R4
                    && instruction.operand is float f
                    && f == OldLimit)
                {
                    instruction.operand = NewLimit;
                }

                yield return instruction;
            }
        }
    }
}
