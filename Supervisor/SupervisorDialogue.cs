using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MelonLoader;
using MoreNPCs.NPCs;
using MoreNPCs.Utils;
using S1API.Entities;
using UnityEngine;

namespace MoreNPCs.Supervisor
{
    /// <summary>
    /// Dialogue setup for supervisor NPCs. Each supervisor has separate dealers and cash.
    /// </summary>
    public static class SupervisorDialogue
    {
        private static bool _isBuildingDialogue;
        private static bool _manageDealersJustRefreshed;
        private static bool _entryJustRefreshed;
        private static readonly Dictionary<string, string> _displayNames = new();
        public static bool IsPlayerInMenu { get; private set; }

        public static void SetupFor(NPC npc, string displayName, string supervisorId)
        {
            if (npc == null) return;
            _displayNames[supervisorId] = displayName;
            var dialogue = npc.Dialogue;
            if (dialogue == null) return;

            var containerName = GetContainerName(supervisorId);
            dialogue.BuildAndSetDatabase(db => db.WithModuleEntry("Supervisor", containerName, "Supervisor"));

            BuildDialogueContainer(npc, supervisorId, displayName, false);
            dialogue.UseContainerOnInteract(containerName);

            dialogue.OnConversationStart(() =>
            {
                IsPlayerInMenu = true;
                SupervisorRegistry.CurrentSupervisorWithMenu = npc;
                SupervisorRegistry.CurrentSupervisorId = supervisorId;
                if (_isBuildingDialogue) return;
                try { BuildDialogueContainer(npc, supervisorId, displayName, false); }
                catch (Exception ex)
                {
                    MelonLogger.Error($"Supervisor OnConversationStarting: {ex.Message}\n{ex.StackTrace}");
                    _isBuildingDialogue = false;
                }
            });

        }

        public static void RefreshFor(NPC npc, string displayName, string supervisorId)
        {
            if (npc?.Dialogue == null) return;
            _displayNames[supervisorId] = displayName;
            BuildDialogueContainer(npc, supervisorId, displayName, false);
        }

        private static string GetContainerName(string supervisorId) =>
            supervisorId == SupervisorIds.Silas ? "silas_supervisor" : "dominic_supervisor";

        private static void BuildDialogueContainer(NPC npc, string supervisorId, string displayName, bool refreshDealers)
        {
            if (_isBuildingDialogue) return;
            _isBuildingDialogue = true;
            var dialogue = npc?.Dialogue;
            if (dialogue == null) { _isBuildingDialogue = false; return; }

            var containerName = GetContainerName(supervisorId);
            try
            {
                if (npc.Relationship?.IsUnlocked != true)
                {
                    dialogue.ClearCallbacks();
                    dialogue.BuildAndRegisterContainer(containerName, c =>
                    {
                        c.AddNode("ENTRY", "I'm busy. Don't waste my time.", ch => ch.Add("OK", "Alright.", "EXIT"));
                        c.AddNode("EXIT", "");
                    });
                    _isBuildingDialogue = false;
                    return;
                }

                if (refreshDealers) GameDealerFinder.InvalidateCache();
                var assignedIds = SupervisorManager.GetAssignedDealerIds(supervisorId);
                var recruitedDealers = SupervisorManager.GetAssignableDealers(supervisorId, refreshDealers);
                if (recruitedDealers == null) recruitedDealers = new List<SupervisorManager.AssignableDealer>();
                int assignedCount = assignedIds?.Count ?? 0;
                bool hasAssigned = assignedCount > 0;

                dialogue.ClearCallbacks();
                dialogue.OnNodeDisplayed("MANAGE_DEALERS", () => HandleManageDealersDisplay(npc, supervisorId));
                dialogue.OnNodeDisplayed("ENTRY", () => HandleEntryDisplay(npc, supervisorId, displayName));

                var storedCash = SupervisorManager.GetStoredCash(supervisorId);
                dialogue.BuildAndRegisterContainer(containerName, c =>
                {
                    c.AddNode("ENTRY", "What can I do for you?", choices =>
                    {
                        choices.Add("trade", "I need to trade some items", "TRADE_ACTION")
                            .Add("collect", $"I need to collect the earnings <color=#54E717>(${storedCash:N0})</color>", "COLLECT_ACTION")
                            .Add("manage", "I need to manage your dealers", "MANAGE_DEALERS")
                            .Add("where_find", "Where can I find you?", "IDLE_LOCATION")
                            .Add("exit", "Nevermind", "EXIT_IMMEDIATE");
                    });
                    c.AddNode("IDLE_LOCATION", NPCIdleLocations.GetSupervisorIdleDialogue(supervisorId), choices => choices.Add("back_idle", "Back", "ENTRY"));
                    c.AddNode("TRADE_ACTION", "", ch => { });
                    c.AddNode("COLLECT_ACTION", "", ch => { });
                    c.AddNode("EXIT_IMMEDIATE", "", ch => { });

                    var manageChoices = new List<(string label, string text, string target)>();
                    var assignLabel = assignedCount >= SupervisorManager.MaxAssignedDealers
                        ? $"I need to assign a dealer{"".PadRight(60)}<color=#FF6161>   DEALER LIMIT REACHED ({assignedCount}/{SupervisorManager.MaxAssignedDealers})</color>"
                        : $"I need to assign a dealer ({assignedCount}/{SupervisorManager.MaxAssignedDealers})";
                    manageChoices.Add(("assign_menu", assignLabel, "ASSIGN_DEALER"));
                    if (hasAssigned)
                    {
                        manageChoices.Add(("who_managing", "Who are you currently managing?", "WHO_MANAGING"));
                        manageChoices.Add(("remove_menu", "I need to remove a dealer", "REMOVE_DEALER"));
                    }
                    manageChoices.Add(("back_manage", "Back", "ENTRY"));

                    c.AddNode("MANAGE_DEALERS", "What would you like to do?", choices =>
                    {
                        var ch = choices;
                        foreach (var (label, text, target) in manageChoices) ch = ch.Add(label, text, target);
                    });

                    var assignChoices = new List<(string label, string text, string target)>();
                    foreach (var dealer in recruitedDealers)
                    {
                        if (string.IsNullOrEmpty(dealer.Id)) continue;
                        if (SupervisorManager.IsAssigned(SupervisorIds.Silas, dealer.Id) || SupervisorManager.IsAssigned(SupervisorIds.Dominic, dealer.Id))
                            continue;
                        if (assignedCount < SupervisorManager.MaxAssignedDealers)
                            assignChoices.Add(($"assign_{dealer.Id}", dealer.DisplayName ?? dealer.Id, $"DO_ASSIGN_{dealer.Id}"));
                    }
                    assignChoices.Add(("back_assign", "Back", "MANAGE_DEALERS"));

                    c.AddNode("ASSIGN_DEALER", "Who do you want under your supervision?", choices =>
                    {
                        var ch = choices;
                        foreach (var (label, text, target) in assignChoices) ch = ch.Add(label, text, target);
                    });

                    var removeChoices = new List<(string label, string text, string target)>();
                    var gameDealers = GameDealerFinder.GetRecruitedDealersFromGame(false);
                    foreach (var id in assignedIds ?? Array.Empty<string>())
                    {
                        if (string.IsNullOrEmpty(id) || !SupervisorManager.IsAssigned(supervisorId, id)) continue;
                        var dealer = recruitedDealers.FirstOrDefault(d => string.Equals(d.Id, id, StringComparison.OrdinalIgnoreCase));
                        var g = gameDealers?.FirstOrDefault(d => d != null && string.Equals(d.Id, id, StringComparison.OrdinalIgnoreCase));
                        var displayName = g?.FullName ?? dealer.DisplayName ?? id;
                        removeChoices.Add(($"remove_{id}", displayName, $"DO_REMOVE_{id}"));
                    }
                    removeChoices.Add(("back_remove", "Back", "MANAGE_DEALERS"));

                    var whoManagingText = GetWhoIsManagingText(supervisorId, recruitedDealers);
                    c.AddNode("WHO_MANAGING", whoManagingText, choices => choices.Add("back_who", "Back", "MANAGE_DEALERS"));
                    c.AddNode("REMOVE_DEALER", "Who should I stop supervising?", choices =>
                    {
                        var ch = choices;
                        foreach (var (label, text, target) in removeChoices) ch = ch.Add(label, text, target);
                    });

                    foreach (var dealer in recruitedDealers)
                    {
                        if (string.IsNullOrEmpty(dealer.Id)) continue;
                        var dealerId = dealer.Id;
                        if (SupervisorManager.IsAssigned(supervisorId, dealerId))
                            c.AddNode($"DO_REMOVE_{dealerId}", "", ch => ch.Add("ok", "OK", "MANAGE_DEALERS"));
                        else if (assignedCount < SupervisorManager.MaxAssignedDealers)
                            c.AddNode($"DO_ASSIGN_{dealerId}", "", ch => ch.Add("ok", "OK", "MANAGE_DEALERS"));
                    }
                    foreach (var id in assignedIds ?? Array.Empty<string>())
                    {
                        if (string.IsNullOrEmpty(id) || !SupervisorManager.IsAssigned(supervisorId, id)) continue;
                        if (recruitedDealers.Any(d => string.Equals(d.Id, id, StringComparison.OrdinalIgnoreCase))) continue;
                        c.AddNode($"DO_REMOVE_{id}", "", ch => ch.Add("ok", "OK", "MANAGE_DEALERS"));
                    }

                    c.AddNode("EXIT", "Take care.", choices => choices.Add("exit_ok", "Later", "ENTRY"));
                    c.SetAllowExit(true);
                });

                dialogue.OnChoiceSelected("exit", () =>
                {
                    IsPlayerInMenu = false;
                    var n = SupervisorRegistry.CurrentSupervisorWithMenu;
                    var id = SupervisorRegistry.CurrentSupervisorId;
                    SupervisorRegistry.CurrentSupervisorWithMenu = null;
                    SupervisorRegistry.CurrentSupervisorId = null;
                    dialogue.End();
                    if (n != null && !string.IsNullOrEmpty(id))
                        SupervisorActivityChain.ScheduleChainDelayed(n, id);
                });
                dialogue.OnChoiceSelected("exit_ok", () =>
                {
                    IsPlayerInMenu = false;
                    var n = SupervisorRegistry.CurrentSupervisorWithMenu;
                    var id = SupervisorRegistry.CurrentSupervisorId;
                    SupervisorRegistry.CurrentSupervisorWithMenu = null;
                    SupervisorRegistry.CurrentSupervisorId = null;
                    dialogue.End();
                    if (n != null && !string.IsNullOrEmpty(id))
                        SupervisorActivityChain.ScheduleChainDelayed(n, id);
                });
                dialogue.OnChoiceSelected("trade", () => HandleTradeSelected(npc, displayName, supervisorId));
                dialogue.OnChoiceSelected("collect", () => HandleCollectSelected(npc, supervisorId));
                dialogue.OnNodeDisplayed("EXIT_IMMEDIATE", () =>
                {
                    IsPlayerInMenu = false;
                    var n = SupervisorRegistry.CurrentSupervisorWithMenu;
                    var id = SupervisorRegistry.CurrentSupervisorId;
                    SupervisorRegistry.CurrentSupervisorWithMenu = null;
                    SupervisorRegistry.CurrentSupervisorId = null;
                    dialogue.End();
                    if (n != null && !string.IsNullOrEmpty(id))
                        SupervisorActivityChain.ScheduleChainDelayed(n, id);
                });

                foreach (var dealer in recruitedDealers)
                {
                    if (string.IsNullOrEmpty(dealer.Id)) continue;
                    var dealerId = dealer.Id;
                    if (SupervisorManager.IsAssigned(supervisorId, dealerId))
                    {
                        dialogue.OnNodeDisplayed($"DO_REMOVE_{dealerId}", () =>
                        {
                            SupervisorManager.UnassignDealer(supervisorId, dealerId);
                            _isBuildingDialogue = false;
                            BuildDialogueContainer(npc, supervisorId, displayName, false);
                            dialogue.JumpTo(containerName, "MANAGE_DEALERS", false);
                        });
                    }
                    else if (assignedCount < SupervisorManager.MaxAssignedDealers)
                    {
                        dialogue.OnNodeDisplayed($"DO_ASSIGN_{dealerId}", () =>
                        {
                            SupervisorManager.AssignDealer(supervisorId, dealerId);
                            _isBuildingDialogue = false;
                            BuildDialogueContainer(npc, supervisorId, displayName, false);
                            dialogue.JumpTo(containerName, "MANAGE_DEALERS", false);
                        });
                    }
                }
                foreach (var id in assignedIds ?? Array.Empty<string>())
                {
                    if (string.IsNullOrEmpty(id) || !SupervisorManager.IsAssigned(supervisorId, id)) continue;
                    if (recruitedDealers.Any(d => string.Equals(d.Id, id, StringComparison.OrdinalIgnoreCase))) continue;
                    var idCapture = id;
                    dialogue.OnNodeDisplayed($"DO_REMOVE_{idCapture}", () =>
                    {
                        SupervisorManager.UnassignDealer(supervisorId, idCapture);
                        _isBuildingDialogue = false;
                        BuildDialogueContainer(npc, supervisorId, displayName, false);
                        dialogue.JumpTo(containerName, "MANAGE_DEALERS", false);
                    });
                }
            }
            finally { _isBuildingDialogue = false; }
        }

        private static void HandleManageDealersDisplay(NPC npc, string supervisorId)
        {
            if (_manageDealersJustRefreshed) { _manageDealersJustRefreshed = false; return; }
            _isBuildingDialogue = false;
            var displayName = _displayNames.TryGetValue(supervisorId, out var dn) ? dn : $"{npc.FirstName} {npc.LastName}".Trim();
            BuildDialogueContainer(npc, supervisorId, displayName, true);
            _manageDealersJustRefreshed = true;
            npc.Dialogue?.JumpTo(GetContainerName(supervisorId), "MANAGE_DEALERS", false);
        }

        private static void HandleEntryDisplay(NPC npc, string supervisorId, string displayName)
        {
            if (_entryJustRefreshed) { _entryJustRefreshed = false; return; }
            _isBuildingDialogue = false;
            BuildDialogueContainer(npc, supervisorId, displayName, false);
            _entryJustRefreshed = true;
            npc.Dialogue?.JumpTo(GetContainerName(supervisorId), "ENTRY", false);
        }

        private static void HandleCollectSelected(NPC npc, string supervisorId)
        {
            float amount = SupervisorManager.TakeAllStoredCash(supervisorId);
            IsPlayerInMenu = false;
            SupervisorRegistry.CurrentSupervisorWithMenu = null;
            SupervisorRegistry.CurrentSupervisorId = null;
            npc.Dialogue?.End();
            if (amount > 0) PlayerCashHelper.TryAddCash(amount);
            SupervisorActivityChain.ScheduleChainDelayed(npc, supervisorId);
        }

        private static void HandleTradeSelected(NPC npc, string displayName, string supervisorId)
        {
            IsPlayerInMenu = false;
            SupervisorRegistry.CurrentSupervisorWithMenu = null;
            SupervisorRegistry.CurrentSupervisorId = null;
            npc.Dialogue?.End();
            SupervisorRegistry.LastTradedSupervisor = npc;
            SupervisorRegistry.LastTradedSupervisorId = supervisorId;
            MelonCoroutines.Start(DeferredOpenTrade(npc, displayName));
        }

        private static IEnumerator DeferredOpenTrade(NPC npc, string displayName)
        {
            yield return null;
            TryOpenInventoryForTrade(npc, displayName);
        }

        private static string GetWhoIsManagingText(string supervisorId, List<SupervisorManager.AssignableDealer> recruitedDealers)
        {
            var assigned = SupervisorManager.GetAssignedDealerIds(supervisorId);
            if (assigned == null || assigned.Count == 0) return "I'm not managing anyone at the moment.";
            var names = new List<string>();
            var gameDealers = GameDealerFinder.GetRecruitedDealersFromGame(false);
            foreach (var id in assigned)
            {
                var g = gameDealers?.FirstOrDefault(d => d != null && string.Equals(d.Id, id, StringComparison.OrdinalIgnoreCase));
                var dealer = recruitedDealers?.FirstOrDefault(d => string.Equals(d.Id, id, StringComparison.OrdinalIgnoreCase));
                names.Add(g?.FullName ?? dealer?.DisplayName ?? id);
            }
            return "I'm currently supervising: " + string.Join(", ", names) + ".";
        }

        private static void TryOpenInventoryForTrade(NPC npc, string displayName)
        {
            try
            {
                npc.Inventory?.EnsureInitialized();
                var go = npc.gameObject;
                if (go == null) return;
                var invType = FindGameType("ScheduleOne.NPCs.NPCInventory");
                var storageMenuType = FindGameType("ScheduleOne.UI.StorageMenu");
                if (invType == null || storageMenuType == null) return;
                var inv = go.GetComponent(invType) ?? go.GetComponentInChildren(invType, true);
                if (inv == null) return;
                var instanceProp = storageMenuType.GetProperty("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.FlattenHierarchy);
                var storageMenu = instanceProp?.GetValue(null);
                if (storageMenu == null) return;
                System.Reflection.MethodInfo? openMethod = null;
                foreach (var m in storageMenuType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                {
                    if (m.Name != "Open") continue;
                    var ps = m.GetParameters();
                    if (ps.Length == 3) { openMethod = m; break; }
                    if (ps.Length == 4 && openMethod == null) openMethod = m;
                }
                if (openMethod == null) return;
                var title = $"{displayName}'s Inventory";
                object[] args = openMethod.GetParameters().Length == 4
                    ? GetTwoSidedTradeArgs(inv, invType, displayName)
                    : new object[] { inv, title, $"Trade items with {displayName}." };
                if (args == null) return;
                openMethod.Invoke(storageMenu, args);
                RegisterTradeClosedHandler(storageMenu, storageMenuType);
            }
            catch { }
        }

        private static object[]? GetTwoSidedTradeArgs(object npcInv, Type invType, string displayName)
        {
            var player = Player.Local;
            if (player?.Transform == null) return null;
            var playerInv = player.Transform.gameObject.GetComponent(invType);
            if (playerInv == null) return null;
            return new object[] { playerInv, npcInv, $"{displayName}'s Inventory", $"Trade items with {displayName}." };
        }

        private static void RegisterTradeClosedHandler(object storageMenu, Type storageMenuType)
        {
            try
            {
                var onClosedProp = storageMenuType.GetProperty("onClosed", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                var onClosed = onClosedProp?.GetValue(storageMenu);
                if (onClosed == null) return;
                var addListener = onClosed.GetType().GetMethod("AddListener", new[] { typeof(UnityEngine.Events.UnityAction) });
                if (addListener == null) return;
                UnityEngine.Events.UnityAction handler = null!;
                handler = () =>
                {
                    try
                    {
                        var removeListener = onClosed.GetType().GetMethod("RemoveListener", new[] { typeof(UnityEngine.Events.UnityAction) });
                        removeListener?.Invoke(onClosed, new object[] { handler });
                        var n = SupervisorRegistry.LastTradedSupervisor;
                        var id = SupervisorRegistry.LastTradedSupervisorId;
                        SupervisorRegistry.LastTradedSupervisor = null;
                        SupervisorRegistry.LastTradedSupervisorId = null;
                        if (n != null && !string.IsNullOrEmpty(id))
                            SupervisorActivityChain.ScheduleChainDelayed(n, id);
                    }
                    catch { }
                };
                addListener.Invoke(onClosed, new object[] { handler });
            }
            catch { }
        }

        private static Type? FindGameType(string fullName)
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                var t = asm.GetType(fullName);
                if (t != null) return t;
            }
            return null;
        }
    }
}
