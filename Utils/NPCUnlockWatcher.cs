using System;
using System.Collections.Generic;
using System.Linq;
using MelonLoader;
using MoreNPCs.Distributor;
using MoreNPCs.Manager;
using MoreNPCs.NPCs;
using MoreNPCs.Persistence;
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
        private const float NpcRelationScanIntervalSeconds = 2f;
        private const int DominicDealerThreshold = 6;
        private const int SilasDealerThreshold = 12;
        private const int DistributorDealerThreshold = 15;
        private const float MeetAtPlayerSmsRelationshipThreshold = 5f;
        private const float DailyRelationshipGain = 0.1f;
        private float _nextCheckTime;
        private float _nextRelationScanTime;
        private int _lastRelationshipDay = -1;

        private static readonly string[] TacoTicklerKeywords = { "taco tickler", "tacotickler" };
        private static readonly HashSet<string> _meetAtPlayerSmsUnlockedThisSession = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, bool> _lastKnownUnlockStates = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

        private NPC? _dominic;
        private NPC? _silas;
        private NPC? _thomas;
        private NPC? _ppHyland;
        private GrantEllison? _grant;
        private bool _trackedNpcsResolved;

        public void Update()
        {
            if (!NPC.CustomNpcsReady) return;

            TryResolveTrackedNpcs();
            TryDailyRelationshipGain();

            if (Time.time >= _nextRelationScanTime)
            {
                _nextRelationScanTime = Time.time + NpcRelationScanIntervalSeconds;
                TryUnlockMeetAtPlayerSms();
                TryRefreshDialogueState();
            }

            if (Time.time < _nextCheckTime) return;
            _nextCheckTime = Time.time + CheckIntervalSeconds;
            TryUnlockRecurring();
        }

        private void TryResolveTrackedNpcs()
        {
            if (_trackedNpcsResolved) return;
            var all = NPC.All;
            if (all == null) return;

            foreach (var npc in all)
            {
                if (npc == null) continue;
                var id = npc.ID;
                if (string.IsNullOrEmpty(id)) continue;

                if (string.Equals(id, "dominic_cross", StringComparison.OrdinalIgnoreCase))
                    _dominic = npc;
                else if (string.Equals(id, "silas_mercer", StringComparison.OrdinalIgnoreCase))
                    _silas = npc;
                else if (string.Equals(id, "thomas_ashford", StringComparison.OrdinalIgnoreCase))
                    _thomas = npc;
                else if (string.Equals(id, "pp_hyland", StringComparison.OrdinalIgnoreCase))
                    _ppHyland = npc;
                else if (DistributorIds.MatchesNpcId(id) && npc is GrantEllison g)
                    _grant = g;
            }

            _trackedNpcsResolved = _dominic != null || _silas != null || _thomas != null || _ppHyland != null || _grant != null;
        }

        private void TryRefreshDialogueState()
        {
            try
            {
                foreach (var npc in TrackedRoleNpcs())
                {
                    if (npc?.Relationship == null) continue;
                    var id = npc.ID;
                    if (string.IsNullOrEmpty(id)) continue;

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
            try
            {
                int currentDay = TimeManager.ElapsedDays;
                if (_lastRelationshipDay == currentDay) return;
                _lastRelationshipDay = currentDay;

                var dealerCount = GetRecruitedDealerCount();
                var ownsTacoTicklers = PlayerOwnsTacoTicklers();

                foreach (var npc in TrackedRoleNpcs())
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

        private void TryUnlockMeetAtPlayerSms()
        {
            try
            {
                foreach (var npc in TrackedSupervisorAndManagerNpcs())
                {
                    if (npc?.Relationship == null || !npc.Relationship.IsUnlocked) continue;
                    var id = npc.ID;
                    if (string.IsNullOrEmpty(id)) continue;
                    if (_meetAtPlayerSmsUnlockedThisSession.Contains(id)) continue;

                    if (npc.Relationship.Delta >= MeetAtPlayerSmsRelationshipThreshold)
                    {
                        _meetAtPlayerSmsUnlockedThisSession.Add(id);
                        if (string.Equals(id, "thomas_ashford", StringComparison.OrdinalIgnoreCase) && npc is ThomasAshford thomas)
                            ManagerTextingSetup.AddComeToMeOption(thomas);
                        else
                            SupervisorTextingSetup.AddComeToMeOption(npc);
                        if (!MoreNPCsModSave.NpcUnlockIntroTexts.HasBeenSentFor(id))
                        {
                            MoreNPCsModSave.NpcUnlockIntroTexts.MarkSentFor(id);
                            if (string.Equals(id, "thomas_ashford", StringComparison.OrdinalIgnoreCase) && npc is ThomasAshford managerNpc)
                                ManagerTextingSetup.SendMessageFrom(managerNpc, "You can tell me to come to you now.");
                            else
                                SupervisorTextingSetup.SendMessageFrom(npc, "You can tell me to come to you now.");
                        }
                    }
                }
            }
            catch (Exception ex) { MelonLogger.Warning($"Meet-at-player SMS unlock failed: {ex.Message}"); }
        }

        private void TryUnlockRecurring()
        {
            try
            {
                var dealerCount = GetRecruitedDealerCount();
                var ownsTacoTicklers = PlayerOwnsTacoTicklers();

                TryUnlockNpc(_dominic, "dominic_cross", () => dealerCount >= DominicDealerThreshold, npc =>
                {
                    npc.Relationship!.Unlock(NPCRelationship.UnlockType.DirectApproach, notify: false);
                    SupervisorTextingSetup.SendMessageFrom(npc, "Heard you're building something. Name's Dominic. I run supply, collect cash, keep your dealers moving. Ten percent of what I bring in, up to six of them. Find me downtown when you're ready.");
                    MelonLogger.Msg("Unlocked Dominic Cross (6 dealers)");
                });

                TryUnlockNpc(_silas, "silas_mercer", () => dealerCount >= SilasDealerThreshold, npc =>
                {
                    npc.Relationship!.Unlock(NPCRelationship.UnlockType.DirectApproach, notify: false);
                    SupervisorTextingSetup.SendMessageFrom(npc, "Twelve dealers and no one helping you manage them? I'm Silas. I handle supply runs, pickups, the logistics. Ten percent of what I collect, six dealers max. I'm around Uptown when you're ready.");
                    MelonLogger.Msg("Unlocked Silas Mercer (12 dealers)");
                });

                TryUnlockNpc(_thomas, "thomas_ashford", () => ownsTacoTicklers, npc =>
                {
                    npc.Relationship!.Unlock(NPCRelationship.UnlockType.DirectApproach, notify: false);
                    if (npc is ThomasAshford thomas)
                        ManagerTextingSetup.SendMessageFrom(thomas, "Heard you bought the Taco Ticklers. Thomas Ashford. Come see me when you're ready to talk business.");
                    MelonLogger.Msg("Unlocked Thomas Ashford (Taco Ticklers)");
                    RefreshDialogueFor(npc);
                });

                TryUnlockNpc(_grant, DistributorIds.Grant, () => dealerCount >= DistributorDealerThreshold, npc =>
                {
                    npc.Relationship!.Unlock(NPCRelationship.UnlockType.DirectApproach, notify: false);
                    MoreNPCsModSave.NpcUnlockIntroTexts.MarkSentFor(DistributorIds.Grant);
                    npc.SendTextMessage(DistributorText.UnlockSms());
                    MelonLogger.Msg("Unlocked Richard Roe wholesale contact (15 dealers)");
                    DistributorWeeklyOrder.EnsureCurrentWeekOrder();
                    if (npc is GrantEllison grant)
                        DistributorTextingSetup.Setup(grant);
                });
            }
            catch (Exception ex) { MelonLogger.Warning($"NPCUnlockWatcher: {ex.Message}"); }
        }

        private static void TryUnlockNpc(NPC? npc, string id, Func<bool> condition, Action<NPC> unlock)
        {
            if (npc?.Relationship == null || npc.Relationship.IsUnlocked) return;
            if (!string.Equals(npc.ID, id, StringComparison.OrdinalIgnoreCase)) return;
            if (!condition()) return;
            unlock(npc);
        }

        private IEnumerable<NPC> TrackedRoleNpcs()
        {
            if (_dominic != null) yield return _dominic;
            if (_silas != null) yield return _silas;
            if (_thomas != null) yield return _thomas;
            if (_ppHyland != null) yield return _ppHyland;
        }

        private IEnumerable<NPC> TrackedSupervisorAndManagerNpcs()
        {
            if (_dominic != null) yield return _dominic;
            if (_silas != null) yield return _silas;
            if (_thomas != null) yield return _thomas;
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
                    return;
                }

                if (string.Equals(npc.ID, "pp_hyland", StringComparison.OrdinalIgnoreCase) && npc is PPHyland hyland)
                {
                    PPHylandDialogue.RefreshFor(hyland);
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
