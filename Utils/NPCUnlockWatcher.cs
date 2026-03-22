using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MelonLoader;
using MoreNPCs.Manager;
using MoreNPCs.NPCs;
using MoreNPCs.Supervisor;
using S1API.Entities;
using S1API.GameTime;
using S1API.Property;
using UnityEngine;

namespace MoreNPCs.Utils
{
    /// <summary>Unlocks Dominic at 6 dealers, Silas at 12 dealers, Manager when buying Taco Ticklers. Sends text on unlock.</summary>
    public class NPCUnlockWatcher
    {
        private const float CheckIntervalSeconds = 30f;
        private const float LockEnforceIntervalSeconds = 5f;
        private const int DominicDealerThreshold = 6;
        private const int SilasDealerThreshold = 12;
        private const float ComeToMeRelationshipThreshold = 5f;
        private const float DailyRelationshipGain = 0.1f;
        private float _nextCheckTime;
        private float _nextLockEnforceTime;
        private int _lastRelationshipDay = -1;

        private static readonly string[] TacoTicklerKeywords = { "taco tickler", "tacotickler" };
        private static readonly string[] ComeToMeNPCIds = { "silas_mercer", "dominic_cross", "thomas_ashford" };
        private static readonly HashSet<string> _comeToMeAddedThisSession = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, bool> _lastKnownUnlockStates = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

        public void Initialize()
        {
            TimeManager.OnDayPass += OnDayPassed;
        }

        private void OnDayPassed() { }

        public void Update()
        {
            TryEnforceLockedState();
            TryDailyRelationshipGain();
            TryUnlockComeToMe();
            TryRefreshDialogueState();

            if (Time.time < _nextCheckTime) return;
            _nextCheckTime = Time.time + CheckIntervalSeconds;
            TryUnlockRecurring();
        }

        private void TryRefreshDialogueState()
        {
            if (!NPC.CustomNpcsReady) return;

            try
            {
                var all = NPC.All;
                if (all == null) return;

                foreach (var npc in all)
                {
                    if (npc?.Relationship == null) continue;
                    var id = npc.ID;
                    if (string.IsNullOrEmpty(id)) continue;
                    if (!ComeToMeNPCIds.Contains(id)) continue;

                    var isUnlocked = npc.Relationship.IsUnlocked;
                    if (_lastKnownUnlockStates.TryGetValue(id, out var previous) && previous == isUnlocked) continue;

                    _lastKnownUnlockStates[id] = isUnlocked;
                    RefreshDialogueFor(npc);
                }
            }
            catch (Exception ex) { MelonLogger.Warning($"Dialogue refresh check failed: {ex.Message}"); }
        }

        private void TryDailyRelationshipGain()
        {
            if (!NPC.CustomNpcsReady) return;
            try
            {
                int currentDay = TimeManager.ElapsedDays;
                if (_lastRelationshipDay == currentDay) return;
                _lastRelationshipDay = currentDay;

                var dealerCount = GetRecruitedDealerCount();
                var ownsTacoTicklers = PlayerOwnsTacoTicklers();
                var all = NPC.All;
                if (all == null) return;

                foreach (var npc in all)
                {
                    if (npc?.Relationship == null || !npc.Relationship.IsUnlocked) continue;
                    var id = npc.ID;
                    if (string.IsNullOrEmpty(id)) continue;

                    bool shouldGain = false;
                    if (string.Equals(id, "dominic_cross", StringComparison.OrdinalIgnoreCase))
                        shouldGain = dealerCount >= DominicDealerThreshold;
                    else if (string.Equals(id, "silas_mercer", StringComparison.OrdinalIgnoreCase))
                        shouldGain = dealerCount >= SilasDealerThreshold;
                    else if (string.Equals(id, "thomas_ashford", StringComparison.OrdinalIgnoreCase))
                        shouldGain = ownsTacoTicklers;

                    if (shouldGain)
                        npc.Relationship.Add(DailyRelationshipGain, network: true);
                }
            }
            catch (Exception ex) { MelonLogger.Warning($"Daily relationship gain failed: {ex.Message}"); }
        }

        private void TryUnlockComeToMe()
        {
            if (!NPC.CustomNpcsReady) return;
            try
            {
                var all = NPC.All;
                if (all == null) return;

                foreach (var npc in all)
                {
                    if (npc?.Relationship == null || !npc.Relationship.IsUnlocked) continue;
                    var id = npc.ID;
                    if (string.IsNullOrEmpty(id) || !ComeToMeNPCIds.Contains(id)) continue;
                    if (_comeToMeAddedThisSession.Contains(id)) continue;

                    if (npc.Relationship.Delta >= ComeToMeRelationshipThreshold)
                    {
                        _comeToMeAddedThisSession.Add(id);
                        if (string.Equals(id, "thomas_ashford", StringComparison.OrdinalIgnoreCase) && npc is ThomasAshford thomas)
                            ManagerTextingSetup.AddComeToMeOption(thomas);
                        else
                            SupervisorTextingSetup.AddComeToMeOption(npc);
                        if (!ComeToMeSave.HasNotifySent(id))
                        {
                            ComeToMeSave.MarkNotifySent(id);
                            if (string.Equals(id, "thomas_ashford", StringComparison.OrdinalIgnoreCase) && npc is ThomasAshford managerNpc)
                                ManagerTextingSetup.SendMessageFrom(managerNpc, "You can tell me to come to you now.");
                            else
                                SupervisorTextingSetup.SendMessageFrom(npc, "You can tell me to come to you now.");
                        }
                    }
                }
            }
            catch (Exception ex) { MelonLogger.Warning($"Come-to-me unlock failed: {ex.Message}"); }
        }

        private void TryEnforceLockedState()
        {
            if (Time.time < _nextLockEnforceTime) return;
            _nextLockEnforceTime = Time.time + LockEnforceIntervalSeconds;
            if (!NPC.CustomNpcsReady) return;

            try
            {
                var dealerCount = GetRecruitedDealerCount();
                var ownsTacoTicklers = PlayerOwnsTacoTicklers();

                var all = NPC.All;
                if (all == null) return;

                foreach (var npc in all)
                {
                    if (npc?.Relationship == null || !npc.Relationship.IsUnlocked) continue;

                    var id = npc.ID;
                    if (string.IsNullOrEmpty(id)) continue;

                    bool shouldBeLocked = false;

                    if (string.Equals(id, "dominic_cross", StringComparison.OrdinalIgnoreCase))
                        shouldBeLocked = dealerCount < DominicDealerThreshold;
                    else if (string.Equals(id, "silas_mercer", StringComparison.OrdinalIgnoreCase))
                        shouldBeLocked = dealerCount < SilasDealerThreshold;
                    else if (string.Equals(id, "thomas_ashford", StringComparison.OrdinalIgnoreCase))
                        shouldBeLocked = !ownsTacoTicklers;

                    if (shouldBeLocked)
                    {
                        TryLockRelationship(npc);
                        RefreshDialogueFor(npc);
                    }
                }
            }
            catch (Exception ex) { MelonLogger.Warning($"Lock enforcement failed: {ex.Message}"); }
        }

        private static void TryLockRelationship(NPC npc)
        {
            try
            {
                var rel = npc.Relationship;
                if (rel == null) return;

                var relType = rel.GetType();
                var lockMethod = relType.GetMethod("Lock", BindingFlags.Public | BindingFlags.Instance);
                if (lockMethod != null && lockMethod.GetParameters().Length == 0)
                {
                    lockMethod.Invoke(rel, null);
                    return;
                }

                var dataProp = relType.GetProperty("Data", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                var data = dataProp?.GetValue(rel);
                if (data != null)
                {
                    var dataType = data.GetType();
                    var setUnlocked = dataType.GetProperty("IsUnlocked", BindingFlags.Public | BindingFlags.Instance);
                    if (setUnlocked?.CanWrite == true)
                    {
                        setUnlocked.SetValue(data, false);
                        return;
                    }
                    var unlockField = dataType.GetField("IsUnlocked", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                        ?? dataType.GetField("<IsUnlocked>k__BackingField", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (unlockField != null)
                    {
                        unlockField.SetValue(data, false);
                    }
                }
            }
            catch { }
        }

        private void TryUnlockRecurring()
        {
            if (!NPC.CustomNpcsReady) return;
            try
            {
                var dealerCount = GetRecruitedDealerCount();
                var ownsTacoTicklers = PlayerOwnsTacoTicklers();

                var all = NPC.All;
                if (all == null) return;
                foreach (var npc in all)
                {
                    if (npc == null) continue;
                    var id = npc.ID;
                    if (string.IsNullOrEmpty(id)) continue;
                    var rel = npc.Relationship;
                    if (rel == null || rel.IsUnlocked) continue;

                    if (string.Equals(id, "dominic_cross", StringComparison.OrdinalIgnoreCase))
                    {
                        if (dealerCount >= DominicDealerThreshold)
                        {
                            rel.Unlock(NPCRelationship.UnlockType.DirectApproach, notify: false);
                            SupervisorTextingSetup.SendMessageFrom(npc, "Heard you're building something. Name's Dominic. I run supply, collect cash, keep your dealers moving. Ten percent of what I bring in, up to six of them. Find me downtown when you're ready.");
                            MelonLogger.Msg("Unlocked Dominic Cross (6 dealers)");
                        }
                        continue;
                    }

                    if (string.Equals(id, "silas_mercer", StringComparison.OrdinalIgnoreCase))
                    {
                        if (dealerCount >= SilasDealerThreshold)
                        {
                            rel.Unlock(NPCRelationship.UnlockType.DirectApproach, notify: false);
                            SupervisorTextingSetup.SendMessageFrom(npc, "Twelve dealers and no one helping you manage them? I'm Silas. I handle supply runs, pickups, the logistics. Ten percent of what I collect, six dealers max. I'm around Uptown when you're ready.");
                            MelonLogger.Msg("Unlocked Silas Mercer (12 dealers)");
                        }
                        continue;
                    }

                    if (string.Equals(id, "thomas_ashford", StringComparison.OrdinalIgnoreCase))
                    {
                        if (ownsTacoTicklers)
                        {
                            rel.Unlock(NPCRelationship.UnlockType.DirectApproach, notify: false);
                            if (npc is ThomasAshford thomas)
                                ManagerTextingSetup.SendMessageFrom(thomas, "Heard you bought the Taco Ticklers. Thomas Ashford. Come see me when you're ready to talk business.");
                            MelonLogger.Msg("Unlocked Thomas Ashford (Taco Ticklers)");
                            RefreshDialogueFor(npc);
                        }
                        continue;
                    }
                }
            }
            catch (Exception ex) { MelonLogger.Warning($"NPCUnlockWatcher: {ex.Message}"); }
        }

        private static void RefreshDialogueFor(NPC npc)
        {
            try
            {
                if (npc is ThomasAshford thomas)
                {
                    ManagerDialogue.RefreshFor(thomas);
                    return;
                }

                if (string.Equals(npc.ID, SupervisorIds.Silas, StringComparison.OrdinalIgnoreCase))
                {
                    SupervisorDialogue.RefreshFor(npc, "Silas Mercer", SupervisorIds.Silas);
                    return;
                }

                if (string.Equals(npc.ID, SupervisorIds.Dominic, StringComparison.OrdinalIgnoreCase))
                {
                    SupervisorDialogue.RefreshFor(npc, "Dominic Cross", SupervisorIds.Dominic);
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"RefreshDialogueFor failed: {ex.Message}");
            }
        }

        private static int GetRecruitedDealerCount()
        {
            try
            {
                return GameDealerFinder.GetRecruitedDealersFromGame(true)?.Count ?? 0;
            }
            catch
            {
                return 0;
            }
        }

        private static bool PlayerOwnsTacoTicklers()
        {
            try
            {
                var owned = BusinessManager.GetOwnedBusinesses();
                if (owned == null || owned.Count == 0) return false;
                foreach (var b in owned)
                {
                    var name = b?.PropertyName?.Trim().ToLowerInvariant();
                    if (string.IsNullOrEmpty(name)) continue;
                    if (TacoTicklerKeywords.Any(kw => name.Contains(kw))) return true;
                }
                return false;
            }
            catch { return false; }
        }
    }
}
