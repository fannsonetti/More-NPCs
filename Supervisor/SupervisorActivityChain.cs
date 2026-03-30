using System.Collections;
using MelonLoader;
using MoreNPCs.Utils;
using S1API.Entities;
using UnityEngine;

namespace MoreNPCs.Supervisor
{
    /// <summary>Chains supervisor activities: distribute → collect → idle. Supports Silas and Dominic.</summary>
    public static class SupervisorActivityChain
    {
        private static float IdleCheckInterval =>
            !MoreNPCsPreferences.Registered ? 45f : MoreNPCsPreferences.SupervisorChain_IdleCheckInterval.Value;
        private static float ChainDelaySeconds =>
            !MoreNPCsPreferences.Registered ? 5f : MoreNPCsPreferences.SupervisorChain_ChainDelaySeconds.Value;
        private static float InitialStartDelay =>
            !MoreNPCsPreferences.Registered ? 10f : MoreNPCsPreferences.SupervisorChain_InitialStartDelay.Value;

        public static void Initialize(NPC npc, string supervisorId) => MelonCoroutines.Start(InitRoutine(npc, supervisorId));

        private static IEnumerator InitRoutine(NPC npc, string supervisorId)
        {
            yield return new WaitForSeconds(InitialStartDelay);
            if (npc == null) yield break;
            MelonCoroutines.Start(IdleLoop(npc, supervisorId));
            MelonCoroutines.Start(DelayedChainCoroutine(npc, supervisorId));
        }

        public static void ScheduleChainDelayed(NPC npc, string supervisorId)
        {
            if (npc == null) return;
            MelonCoroutines.Start(DelayedChainCoroutine(npc, supervisorId));
        }

        private static IEnumerator DelayedChainCoroutine(NPC npc, string supervisorId)
        {
            yield return new WaitForSeconds(ChainDelaySeconds);
            if (npc == null) yield break;
            if (SupervisorDialogue.IsPlayerInMenu) yield break;
            if (SupervisorDrugDistributor.IsRunning || SupervisorEarningsCollector.IsCollecting) yield break;
            yield return RunChainCoroutine(npc, supervisorId);
        }

        private static IEnumerator RunChainCoroutine(NPC npc, string supervisorId)
        {
            if (npc == null) yield break;
            if (SupervisorDialogue.IsPlayerInMenu) yield break;

            if (SupervisorDrugDistributor.TryStartDistribute(npc, supervisorId))
            {
                while (SupervisorDrugDistributor.IsRunning && !SupervisorDialogue.IsPlayerInMenu)
                    yield return null;
                if (SupervisorDialogue.IsPlayerInMenu) yield break;
            }

            if (SupervisorEarningsCollector.TryStartCollect(npc, supervisorId))
            {
                while (SupervisorEarningsCollector.IsCollecting && !SupervisorDialogue.IsPlayerInMenu)
                    yield return null;
                if (SupervisorDialogue.IsPlayerInMenu) yield break;
            }

            SupervisorIdleController.GoHome(npc);
        }

        private static IEnumerator IdleLoop(NPC npc, string supervisorId)
        {
            var wait = new WaitForSeconds(IdleCheckInterval);
            while (npc != null)
            {
                yield return wait;
                if (SupervisorDialogue.IsPlayerInMenu) continue;
                if (SupervisorDrugDistributor.IsRunning || SupervisorEarningsCollector.IsCollecting) continue;
                if (!SupervisorIdleController.IsAtHome(npc)) continue;
                if (!SupervisorDrugDistributor.CanDistribute(npc, supervisorId) && !SupervisorEarningsCollector.CanCollect(npc, supervisorId)) continue;
                MelonCoroutines.Start(RunChainCoroutine(npc, supervisorId));
            }
        }
    }
}
