using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MelonLoader;
using MoreNPCs.NPCs;
using MoreNPCs.Utils;
using S1API.Entities;
using S1API.Property;
using UnityEngine;

namespace MoreNPCs.Manager
{
    /// <summary>Thomas walks to assigned businesses in order, deposits stored funds as laundering (cut from prefs).</summary>
    public static class ManagerLaunderingChain
    {
        private const float IdleCheckInterval = 15f;
        private const float ChainDelaySeconds = 5f;
        private const float InitialStartDelay = 10f;
        private const float ArrivalRadiusDefault = 2.5f;
        private const float ArrivalRadiusLaundromat = 0.5f;
        private const float WalkTimeout = 180f;

        private static float LaunderCutPercent =>
            !MoreNPCsPreferences.Registered ? 0.10f : MoreNPCsPreferences.Manager_LaunderCutPercent.Value;

        private static readonly (string Name, Vector3 Position)[] BusinessRoute = new[]
        {
            ("Taco Ticklers", new Vector3(-32.5f, 1.065f, 80.5f)),
            ("Laundromat", new Vector3(-27f, 1.565f, 24f)),
            ("Car Wash", new Vector3(-6f, 1.215f, -18f)),
            ("Post Office", new Vector3(50f, 1.115f, -1.35f))
        };

        private static bool _running;

        public static bool IsRunning => _running;

        public static void Initialize(ThomasAshford npc)
        {
            if (npc == null) return;
            MelonCoroutines.Start(InitRoutine(npc));
        }

        public static void ScheduleDelayed(ThomasAshford npc)
        {
            if (npc == null) return;
            MelonCoroutines.Start(DelayedChainCoroutine(npc));
        }

        private static IEnumerator InitRoutine(ThomasAshford npc)
        {
            yield return new WaitForSeconds(InitialStartDelay);
            if (npc == null) yield break;
            MelonCoroutines.Start(IdleLoop(npc));
            MelonCoroutines.Start(DelayedChainCoroutine(npc));
        }

        private static IEnumerator DelayedChainCoroutine(ThomasAshford npc)
        {
            yield return new WaitForSeconds(ChainDelaySeconds);
            if (npc == null) yield break;
            if (ManagerDialogue.IsPlayerInMenu) yield break;
            if (_running) yield break;
            yield return RunChainCoroutine(npc);
        }

        private static IEnumerator IdleLoop(ThomasAshford npc)
        {
            var wait = new WaitForSeconds(IdleCheckInterval);
            while (npc != null)
            {
                yield return wait;
                if (ManagerDialogue.IsPlayerInMenu) continue;
                if (_running) continue;
                if (ManagerBusinessSave.GetStored() <= 0) continue;
                var assigned = ManagerBusinessSave.GetAssignedStatic();
                if (assigned == null || assigned.Count == 0) continue;
                var hasRoute = BusinessRoute.Any(r => IsAssigned(r.Name, assigned));
                if (!hasRoute) continue;
                if (!HasLaunderingCapacity(assigned)) continue;
                MelonCoroutines.Start(RunChainCoroutine(npc));
            }
        }

        private static bool IsAssigned(string businessName, IReadOnlyList<string> assigned)
        {
            if (string.IsNullOrEmpty(businessName) || assigned == null) return false;
            var key = businessName.Trim().ToLowerInvariant();
            foreach (var a in assigned)
            {
                if (string.IsNullOrEmpty(a)) continue;
                if (ArtificialBusinessMapping.MatchesRouteStop(businessName, a)) return true;
                if (a.Trim().ToLowerInvariant().Contains(key) || key.Contains(a.Trim().ToLowerInvariant())) return true;
                if (MatchesBusiness(key, a)) return true;
            }
            return false;
        }

        private static bool MatchesBusiness(string key, string assigned)
        {
            if (key.Contains("taco") || key.Contains("tickler")) return assigned.ToLowerInvariant().Contains("taco") || assigned.ToLowerInvariant().Contains("tickler");
            if (key.Contains("laundromat")) return assigned.ToLowerInvariant().Contains("laundromat");
            if (key.Contains("car") && key.Contains("wash")) return assigned.ToLowerInvariant().Contains("car") && assigned.ToLowerInvariant().Contains("wash");
            if (key.Contains("post") && key.Contains("office")) return assigned.ToLowerInvariant().Contains("post") && assigned.ToLowerInvariant().Contains("office");
            return false;
        }

        private static IEnumerator RunChainCoroutine(ThomasAshford npc)
        {
            if (npc == null) yield break;
            if (ManagerDialogue.IsPlayerInMenu) yield break;
            var stored = ManagerBusinessSave.GetStored();
            if (stored <= 0) yield break;
            var assigned = ManagerBusinessSave.GetAssignedStatic();
            if (assigned == null || assigned.Count == 0) yield break;
            if (!HasLaunderingCapacity(assigned)) yield break;

            _running = true;
            try
            {
                ManagerTextingSetup.SendMessageFrom(npc, "Heading out to run the businesses. I'll text when I'm done.");
                foreach (var (name, pos) in BusinessRoute)
                {
                    if (!IsAssigned(name, assigned)) continue;
                    if (ManagerBusinessSave.GetStored() <= 0) break;
                    if (ManagerDialogue.IsPlayerInMenu) yield break;

                    var radius = string.Equals(name, "Laundromat", StringComparison.OrdinalIgnoreCase) ? ArrivalRadiusLaundromat : ArrivalRadiusDefault;
                    npc.Movement.SetDestination(pos);
                    float timeout = WalkTimeout;
                    while (npc != null && Vector3.Distance(npc.Movement.FootPosition, pos) > radius && timeout > 0)
                    {
                        if (ManagerDialogue.IsPlayerInMenu) yield break;
                        timeout -= Time.deltaTime;
                        yield return null;
                    }
                    if (ManagerDialogue.IsPlayerInMenu || npc == null) yield break;
                    yield return new WaitForSeconds(0.5f);

                    TryLaunderAtBusiness(name);
                }
                npc.Movement.SetDestination(NPCIdleLocations.GetThomasIdlePosition(npc.Movement.FootPosition));
                ManagerTextingSetup.SendMessageFrom(npc, "Finished the run. All deposits are in.");
            }
            finally { _running = false; }
        }

        private static void TryLaunderAtBusiness(string businessDisplayName)
        {
            try
            {
                var biz = BusinessManager.FindBusinessByName(businessDisplayName)
                    ?? BusinessManager.GetAllBusinesses()?.FirstOrDefault(b => b != null && b.IsOwned && NameMatches(b.PropertyName, businessDisplayName));
                if (biz == null || !biz.IsOwned) return;
                if (biz.IsAtLaunderingCapacity) return;

                var stored = ManagerBusinessSave.GetStored();
                if (stored <= 0) return;

                var capacity = biz.AppliedLaunderLimit - biz.CurrentLaunderTotal;
                if (capacity <= 0) return;

                var launderAmount = Mathf.Min(stored / (1f + LaunderCutPercent), capacity);
                if (launderAmount <= 0) return;

                var amountToDeduct = launderAmount * (1f + LaunderCutPercent);
                if (amountToDeduct > stored) amountToDeduct = stored;

                ManagerBusinessSave.TakeStatic(amountToDeduct);
                biz.AddLaunderingOperation(launderAmount, 0);
            }
            catch (Exception ex) { MelonLogger.Warning($"ManagerLaunderingChain TryLaunderAtBusiness: {ex.Message}"); }
        }

        private static bool HasLaunderingCapacity(IReadOnlyList<string> assigned)
        {
            if (assigned == null) return false;
            foreach (var (name, _) in BusinessRoute)
            {
                if (!IsAssigned(name, assigned)) continue;
                var biz = BusinessManager.FindBusinessByName(name)
                    ?? BusinessManager.GetAllBusinesses()?.FirstOrDefault(b => b != null && b.IsOwned && NameMatches(b.PropertyName, name));
                if (biz != null && biz.IsOwned && !biz.IsAtLaunderingCapacity) return true;
            }
            return false;
        }

        private static bool NameMatches(string propName, string displayName)
        {
            if (string.IsNullOrEmpty(propName) || string.IsNullOrEmpty(displayName)) return false;
            var a = propName.Trim().ToLowerInvariant();
            var b = displayName.Trim().ToLowerInvariant();
            return a.Contains(b) || b.Contains(a);
        }
    }
}
