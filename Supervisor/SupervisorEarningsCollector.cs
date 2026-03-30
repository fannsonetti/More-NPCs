using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MelonLoader;
using MoreNPCs.NPCs;
using MoreNPCs.Utils;
using S1API.Entities;
using UnityEngine;

namespace MoreNPCs.Supervisor
{
    /// <summary>Supervisors walk to their assigned dealers to collect their cash.</summary>
    public static class SupervisorEarningsCollector
    {
        private static readonly HashSet<string> _collecting = new HashSet<string>();
        public static bool IsCollecting => _collecting.Count > 0;
        public static bool IsCollectingFor(string supervisorId) => _collecting.Contains(supervisorId);
        private static float WalkTimeout =>
            !MoreNPCsPreferences.Registered ? 120f : MoreNPCsPreferences.Supervisor_WalkTimeoutSeconds.Value;
        private static float MinCashToCollect =>
            !MoreNPCsPreferences.Registered ? 2000f : MoreNPCsPreferences.Supervisor_MinCashToCollect.Value;
        private static float CollectCutPercent =>
            !MoreNPCsPreferences.Registered ? 0.10f : MoreNPCsPreferences.Supervisor_CollectCutPercent.Value;

        public static bool CanCollect(NPC npc, string supervisorId)
        {
            if (_collecting.Count > 0 || npc == null) return false;
            var targets = SupervisorManager.GetDealersWithCashToCollect(supervisorId);
            if (targets == null || targets.Count == 0) return false;
            return targets.Any(t => t.Cash >= MinCashToCollect);
        }

        public static bool TryStartCollect(NPC npc, string supervisorId)
        {
            if (_collecting.Count > 0 || npc == null) return false;
            var targets = SupervisorManager.GetDealersWithCashToCollect(supervisorId);
            if (targets == null || targets.Count == 0) return false;
            if (!targets.Any(t => t.Cash >= MinCashToCollect)) return false;
            _collecting.Add(supervisorId);
            SupervisorTextingSetup.SendMessageFrom(npc, "Going to pick up the take. I'll text when I'm done.");
            MelonCoroutines.Start(CollectCoroutine(npc, supervisorId, targets));
            return true;
        }

        private static IEnumerator CollectCoroutine(NPC npc, string supervisorId, List<SupervisorManager.CollectTarget> targets)
        {
            float totalCollected = 0f;
            try
            {
                foreach (var t in targets)
                {
                    if (t.Cash < MinCashToCollect) continue;
                    var dest = GetDealerPositionNow(t.DealerId);
                    if (dest == Vector3.zero) dest = t.Position;
                    npc.Movement.SetDestination(dest);
                    float timeout = WalkTimeout;
                    while (!IsAsCloseAsPossible(npc, dest) && timeout > 0)
                    {
                        if (SupervisorDialogue.IsPlayerInMenu) yield break;
                        timeout -= Time.deltaTime;
                        yield return null;
                    }
                    if (SupervisorDialogue.IsPlayerInMenu) yield break;
                    yield return new WaitForSeconds(0.5f);
                    float raw = SupervisorManager.TakeDealerCashForSupervisor(supervisorId, t.DealerId);
                    float cut = raw * CollectCutPercent;
                    float toStorage = raw - cut;
                    SupervisorManager.AddToStoredCash(supervisorId, toStorage);
                    totalCollected += toStorage;
                }
                if (totalCollected > 0)
                    SupervisorTextingSetup.SendMessageFrom(npc, $"All done. ${totalCollected:N0} ready for you to collect.");
            }
            finally { _collecting.Remove(supervisorId); }
        }

        private static Vector3 GetDealerPositionNow(string dealerId)
        {
            var dealer = GameDealerFinder.GetRecruitedDealersFromGame(false)
                ?.FirstOrDefault(g => string.Equals(g?.Id, dealerId, StringComparison.OrdinalIgnoreCase));
            return dealer != null ? GameDealerFinder.GetDealerPosition(dealer) : Vector3.zero;
        }

        private static bool IsAsCloseAsPossible(NPC npc, Vector3 dest)
        {
            if (npc?.Movement == null) return false;
            var method = npc.Movement.GetType().GetMethod("IsAsCloseAsPossible", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(Vector3), typeof(float) }, null);
            return method != null && (bool)method.Invoke(npc.Movement, new object[] { dest, 1.5f });
        }
    }
}
