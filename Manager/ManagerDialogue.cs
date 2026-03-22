using System;
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
    /// <summary>Dialogue for the Manager NPC: manage funds and businesses.</summary>
    public static class ManagerDialogue
    {
        public static bool IsPlayerInMenu { get; private set; }
        private const string ContainerName = "manager_dialogue";
        private static bool _manageBusinessesJustRefreshed;
        private static ThomasAshford? _currentManager;

        public static void SetupFor(ThomasAshford manager)
        {
            if (manager == null) return;
            _currentManager = manager;
            ManagerEarningsCollector.EnsureSubscribed();
            var dialogue = manager.Dialogue;
            if (dialogue == null) return;

            dialogue.BuildAndSetDatabase(db => db.WithModuleEntry("Manager", ContainerName, "Manager"));
            BuildDialogueContainer(manager, false);
            dialogue.UseContainerOnInteract(ContainerName);

            dialogue.OnConversationStart(() =>
            {
                _currentManager = manager;
                IsPlayerInMenu = true;
                BuildDialogueContainer(manager, false);
            });
        }

        public static void RefreshFor(ThomasAshford manager)
        {
            if (manager?.Dialogue == null) return;
            _currentManager = manager;
            BuildDialogueContainer(manager, false);
        }

        private static void OnManagerMenuClosed()
        {
            IsPlayerInMenu = false;
            if (_currentManager != null) ManagerLaunderingChain.ScheduleDelayed(_currentManager);
        }

        private static string SafeBusinessId(string name)
        {
            if (string.IsNullOrEmpty(name)) return "biz_unknown";
            return "biz_" + name.Replace(" ", "_").Replace("'", "").Replace("-", "_");
        }

        private static void BuildDialogueContainer(ThomasAshford manager, bool refreshBusinesses)
        {
            var dialogue = manager?.Dialogue;
            if (dialogue == null) return;

            if (manager.Relationship?.IsUnlocked != true)
            {
                dialogue.ClearCallbacks();
                dialogue.BuildAndRegisterContainer(ContainerName, c =>
                {
                    c.AddNode("ENTRY", "I'm busy. Don't waste my time.", ch => ch.Add("OK", "Alright.", "EXIT"));
                    c.AddNode("EXIT", "");
                });
                return;
            }

            var stored = ManagerFundsSave.GetStored();
            var businessEarnings = ManagerFundsSave.GetBusinessEarnings();
            var playerCash = 0f;
            try { playerCash = S1API.Money.Money.GetCashBalance(); } catch { }

            var businesses = GetOwnedBusinesses();
            var assigned = ManagerBusinessSave.GetAssignedStatic() ?? new List<string>();
            int assignedCount = assigned.Count;
            bool hasAssigned = assignedCount > 0;
            bool canAddMore = assignedCount < ManagerBusinessSave.MaxAssignedBusinesses;

            dialogue.ClearCallbacks();
            dialogue.OnNodeDisplayed("MANAGE_BUSINESSES", () => HandleManageBusinessesDisplay(manager!));
            dialogue.OnNodeDisplayed("FUNDS_MENU", () => HandleFundsMenuDisplayed(manager!));

            dialogue.BuildAndRegisterContainer(ContainerName, c =>
            {
                c.AddNode("ENTRY", "What can I do for you?", choices =>
                {
                    var earnings = ManagerFundsSave.GetBusinessEarnings();
                    var earningsStr = earnings > 0 ? $" <color=#54E717>(${earnings:N0})</color>" : "";
                    choices.Add("funds", "I need to manage your funds", "FUNDS_MENU")
                        .Add("collect", $"I need to collect the earnings{earningsStr}", "ENTRY")
                        .Add("businesses", "I need to manage your businesses", "MANAGE_BUSINESSES")
                        .Add("where_find", "Where can I find you?", "IDLE_LOCATION")
                        .Add("exit", "Nevermind", "EXIT_IMMEDIATE");
                });
                c.AddNode("IDLE_LOCATION", NPCIdleLocations.GetThomasIdleDialogue(), choices => choices.Add("back_idle", "Back", "ENTRY"));

                c.AddNode("FUNDS_MENU", "What would you like to do?", choices =>
                {
                    choices.Add("give", "Give money", "GIVE_MENU").Add("take", "Take money", "TAKE_MENU")
                        .Add("back_funds", "Back", "ENTRY");
                });

                var dailyCap = GetDailyLaunderCapacity(assigned);
                if (dailyCap <= 0) dailyCap = 2000f;
                c.AddNode("GIVE_MENU", "How much would you like to give?", choices =>
                {
                    var ch = choices;
                    var amt1 = (int)(dailyCap * 1);
                    var amt3 = (int)(dailyCap * 3);
                    var amt7 = (int)(dailyCap * 7);
                    var amt30 = (int)(dailyCap * 30);
                    ch = ch.Add("give_1d", $"1 day (${amt1:N0})", "DO_GIVE_1D")
                        .Add("give_3d", $"3 days (${amt3:N0})", "DO_GIVE_3D")
                        .Add("give_7d", $"1 week (${amt7:N0})", "DO_GIVE_7D")
                        .Add("give_30d", $"1 month (${amt30:N0})", "DO_GIVE_30D");
                    ch = ch.Add("back_give", "Back", "FUNDS_MENU");
                });

                c.AddNode("TAKE_MENU", stored > 0 ? $"I'm holding ${stored:N0}. How much would you like?" : "I'm not holding any funds.", choices =>
                {
                    var ch = choices;
                    if (stored > 0)
                    {
                        var amt1 = (int)(dailyCap * 1);
                        var amt3 = (int)(dailyCap * 3);
                        var amt7 = (int)(dailyCap * 7);
                        var amt30 = (int)(dailyCap * 30);
                        if (amt1 > 0 && amt1 <= stored) ch = ch.Add("take_1d", $"1 day (${amt1:N0})", "DO_TAKE_1D");
                        if (amt3 > amt1 && amt3 <= stored) ch = ch.Add("take_3d", $"3 days (${amt3:N0})", "DO_TAKE_3D");
                        if (amt7 > amt3 && amt7 <= stored) ch = ch.Add("take_7d", $"1 week (${amt7:N0})", "DO_TAKE_7D");
                        if (amt30 > amt7 && amt30 <= stored) ch = ch.Add("take_30d", $"1 month (${amt30:N0})", "DO_TAKE_30D");
                        ch = ch.Add("take_all", "Take all", "DO_TAKE_ALL");
                    }
                    ch = ch.Add("back_take", "Back", "FUNDS_MENU");
                });

                c.AddNode("DO_GIVE_1D", "", ch => ch.Add("ok", "OK", "EXIT_IMMEDIATE"));
                c.AddNode("DO_GIVE_3D", "", ch => ch.Add("ok", "OK", "EXIT_IMMEDIATE"));
                c.AddNode("DO_GIVE_7D", "", ch => ch.Add("ok", "OK", "EXIT_IMMEDIATE"));
                c.AddNode("DO_GIVE_30D", "", ch => ch.Add("ok", "OK", "EXIT_IMMEDIATE"));
                c.AddNode("DO_TAKE_1D", "", ch => ch.Add("ok", "OK", "TAKE_MENU"));
                c.AddNode("DO_TAKE_3D", "", ch => ch.Add("ok", "OK", "TAKE_MENU"));
                c.AddNode("DO_TAKE_7D", "", ch => ch.Add("ok", "OK", "TAKE_MENU"));
                c.AddNode("DO_TAKE_30D", "", ch => ch.Add("ok", "OK", "TAKE_MENU"));
                c.AddNode("DO_TAKE_ALL", "", ch => ch.Add("ok", "OK", "TAKE_MENU"));

                c.AddNode("COLLECT_ACTION", "", ch => { });
                c.AddNode("INSUFFICIENT_FUNDS", "You don't have enough.", ch => ch.Add("ok", "OK", "GIVE_MENU"));

                var manageChoices = new List<(string label, string text, string target)>();
                var assignLabel = !canAddMore
                    ? $"Add a business{"".PadRight(40)}<color=#FF6161>   LIMIT REACHED ({assignedCount}/{ManagerBusinessSave.MaxAssignedBusinesses})</color>"
                    : $"Add a business ({assignedCount}/{ManagerBusinessSave.MaxAssignedBusinesses})";
                manageChoices.Add(("assign_biz", assignLabel, "ASSIGN_BUSINESS"));
                if (hasAssigned)
                {
                    manageChoices.Add(("who_biz", "See who's managing", "WHO_MANAGING_BUSINESSES"));
                    manageChoices.Add(("remove_biz", "Remove a business", "REMOVE_BUSINESS"));
                }
                manageChoices.Add(("back_manage", "Back", "ENTRY"));

                c.AddNode("MANAGE_BUSINESSES", "What would you like to do?", choices =>
                {
                    var ch = choices;
                    foreach (var (label, text, target) in manageChoices) ch = ch.Add(label, text, target);
                });

                var assignChoices = new List<(string label, string text, string target)>();
                foreach (var b in businesses)
                {
                    var name = b?.PropertyName?.Trim();
                    if (string.IsNullOrEmpty(name)) continue;
                    if (ManagerBusinessSave.IsAssignedStatic(name)) continue;
                    if (!canAddMore) continue;
                    var safeId = SafeBusinessId(name);
                    assignChoices.Add((safeId, name, $"DO_ASSIGN_{safeId}"));
                }
                assignChoices.Add(("back_assign", "Back", "MANAGE_BUSINESSES"));

                c.AddNode("ASSIGN_BUSINESS", businesses.Count > 0
                    ? "Which business should I manage?"
                    : "You don't own any businesses yet.", choices =>
                {
                    var ch = choices;
                    foreach (var (label, text, target) in assignChoices) ch = ch.Add(label, text, target);
                });

                var whoText = GetWhoIsManagingText(assigned);
                c.AddNode("WHO_MANAGING_BUSINESSES", whoText, choices => choices.Add("back_who", "Back", "MANAGE_BUSINESSES"));

                var removeChoices = new List<(string label, string text, string target)>();
                foreach (var name in assigned)
                {
                    if (string.IsNullOrEmpty(name)) continue;
                    var safeId = SafeBusinessId(name);
                    removeChoices.Add((safeId, name, $"DO_REMOVE_{safeId}"));
                }
                removeChoices.Add(("back_remove", "Back", "MANAGE_BUSINESSES"));

                c.AddNode("REMOVE_BUSINESS", "Which business should I stop managing?", choices =>
                {
                    var ch = choices;
                    foreach (var (label, text, target) in removeChoices) ch = ch.Add(label, text, target);
                });

                foreach (var b in businesses)
                {
                    var name = b?.PropertyName?.Trim();
                    if (string.IsNullOrEmpty(name) || ManagerBusinessSave.IsAssignedStatic(name)) continue;
                    var safeId = SafeBusinessId(name);
                    if (!canAddMore) continue;
                    c.AddNode($"DO_ASSIGN_{safeId}", "", ch => ch.Add("ok", "OK", "MANAGE_BUSINESSES"));
                }
                foreach (var name in assigned)
                {
                    if (string.IsNullOrEmpty(name)) continue;
                    var safeId = SafeBusinessId(name);
                    c.AddNode($"DO_REMOVE_{safeId}", "", ch => ch.Add("ok", "OK", "MANAGE_BUSINESSES"));
                }

                c.AddNode("EXIT_IMMEDIATE", "", ch => { });
                c.SetAllowExit(true);
            });

            dialogue.OnNodeDisplayed("EXIT_IMMEDIATE", () =>
            {
                OnManagerMenuClosed();
                dialogue.End();
            });
            var cap = GetDailyLaunderCapacity(assigned);
            if (cap <= 0) cap = 2000f;
            dialogue.OnNodeDisplayed("DO_GIVE_1D", () => HandleGive(manager!, cap * 1));
            dialogue.OnNodeDisplayed("DO_GIVE_3D", () => HandleGive(manager!, cap * 3));
            dialogue.OnNodeDisplayed("DO_GIVE_7D", () => HandleGive(manager!, cap * 7));
            dialogue.OnNodeDisplayed("DO_GIVE_30D", () => HandleGive(manager!, cap * 30));
            dialogue.OnNodeDisplayed("DO_TAKE_1D", () => HandleTake(manager!, cap * 1));
            dialogue.OnNodeDisplayed("DO_TAKE_3D", () => HandleTake(manager!, cap * 3));
            dialogue.OnNodeDisplayed("DO_TAKE_7D", () => HandleTake(manager!, cap * 7));
            dialogue.OnNodeDisplayed("DO_TAKE_30D", () => HandleTake(manager!, cap * 30));
            dialogue.OnNodeDisplayed("DO_TAKE_ALL", () => HandleTakeAll(manager!));
            dialogue.OnNodeDisplayed("TAKE_MENU", () => HandleTakeMenuDisplayed(manager!));
            dialogue.OnChoiceSelected("collect", () => HandleCollectEarnings(manager!));
            dialogue.OnChoiceSelected("exit", () =>
            {
                OnManagerMenuClosed();
                dialogue.End();
            });

            foreach (var b in businesses)
            {
                var name = b?.PropertyName?.Trim();
                if (string.IsNullOrEmpty(name) || ManagerBusinessSave.IsAssignedStatic(name)) continue;
                var safeId = SafeBusinessId(name);
                if (!canAddMore) continue;
                var nameCapture = name;
                dialogue.OnNodeDisplayed($"DO_ASSIGN_{safeId}", () =>
                {
                    ManagerBusinessSave.AssignStatic(nameCapture);
                    BuildDialogueContainer(manager!, false);
                    dialogue.JumpTo(ContainerName, "MANAGE_BUSINESSES", false);
                });
            }
            foreach (var name in assigned)
            {
                if (string.IsNullOrEmpty(name)) continue;
                var safeId = SafeBusinessId(name);
                var nameCapture = name;
                dialogue.OnNodeDisplayed($"DO_REMOVE_{safeId}", () =>
                {
                    ManagerBusinessSave.UnassignStatic(nameCapture);
                    BuildDialogueContainer(manager!, false);
                    dialogue.JumpTo(ContainerName, "MANAGE_BUSINESSES", false);
                });
            }
        }

        private static void HandleManageBusinessesDisplay(ThomasAshford manager)
        {
            if (_manageBusinessesJustRefreshed) { _manageBusinessesJustRefreshed = false; return; }
            BuildDialogueContainer(manager, true);
            _manageBusinessesJustRefreshed = true;
            manager.Dialogue?.JumpTo(ContainerName, "MANAGE_BUSINESSES", false);
        }

        private static bool _fundsMenuJustRefreshed;

        private static void HandleFundsMenuDisplayed(ThomasAshford manager)
        {
            if (_fundsMenuJustRefreshed) { _fundsMenuJustRefreshed = false; return; }
            BuildDialogueContainer(manager, false);
            _fundsMenuJustRefreshed = true;
            manager.Dialogue?.JumpTo(ContainerName, "FUNDS_MENU", false);
        }

        private static void HandleCollectEarnings(ThomasAshford manager)
        {
            try
            {
                var taken = ManagerFundsSave.TakeAllEarningsStatic();
                if (taken > 0) PlayerCashHelper.TryAddCash(taken);
                IsPlayerInMenu = false;
                _currentManager = null;
                manager.Dialogue?.End();
                if (manager != null) ManagerLaunderingChain.ScheduleDelayed(manager);
            }
            catch (Exception ex) { MelonLogger.Warning($"Manager HandleCollectEarnings: {ex.Message}"); }
        }

        private static bool _takeMenuJustRefreshed;

        private static void HandleTakeMenuDisplayed(ThomasAshford manager)
        {
            if (_takeMenuJustRefreshed) { _takeMenuJustRefreshed = false; return; }
            BuildDialogueContainer(manager, false);
            _takeMenuJustRefreshed = true;
            manager.Dialogue?.JumpTo(ContainerName, "TAKE_MENU", false);
        }

        private static string GetWhoIsManagingText(IReadOnlyList<string> assigned)
        {
            if (assigned == null || assigned.Count == 0) return "I'm not managing any businesses.";
            var lines = new List<string> { "I'm currently managing:" };
            foreach (var n in assigned)
                if (!string.IsNullOrEmpty(n)) lines.Add($"• {n}");
            return string.Join("\n", lines);
        }

        private static void HandleGive(ThomasAshford manager, float amount)
        {
            try
            {
                var balance = S1API.Money.Money.GetCashBalance();
                if (balance < amount)
                {
                    BuildDialogueContainer(manager, false);
                    manager.Dialogue?.JumpTo(ContainerName, "INSUFFICIENT_FUNDS", false);
                    return;
                }
                S1API.Money.Money.ChangeCashBalance(-amount, visualizeChange: true, playCashSound: true);
                ManagerFundsSave.AddStatic(amount);
                IsPlayerInMenu = false;
                _currentManager = null;
                manager.Dialogue?.End();
                if (manager != null) ManagerLaunderingChain.ScheduleDelayed(manager);
            }
            catch (Exception ex) { MelonLogger.Warning($"Manager HandleGive: {ex.Message}"); }
        }

        private static void HandleTake(ThomasAshford manager, float amount)
        {
            try
            {
                var stored = ManagerFundsSave.GetStored();
                var take = Math.Min(amount, stored);
                if (take <= 0) return;
                var taken = ManagerFundsSave.TakeStatic(take);
                if (taken > 0) PlayerCashHelper.TryAddCash(taken);
                BuildDialogueContainer(manager, false);
                manager.Dialogue?.JumpTo(ContainerName, "TAKE_MENU", false);
            }
            catch (Exception ex) { MelonLogger.Warning($"Manager HandleTake: {ex.Message}"); }
        }

        private static void HandleTakeAll(ThomasAshford manager)
        {
            try
            {
                var taken = ManagerFundsSave.TakeStatic(float.MaxValue);
                if (taken > 0) PlayerCashHelper.TryAddCash(taken);
                BuildDialogueContainer(manager, false);
                manager.Dialogue?.JumpTo(ContainerName, "TAKE_MENU", false);
            }
            catch (Exception ex) { MelonLogger.Warning($"Manager HandleTakeAll: {ex.Message}"); }
        }

        private static List<BusinessWrapper> GetOwnedBusinesses()
        {
            try { return BusinessManager.GetOwnedBusinesses() ?? new List<BusinessWrapper>(); }
            catch { return new List<BusinessWrapper>(); }
        }

        private static float GetDailyLaunderCapacity(IReadOnlyList<string> assigned)
        {
            if (assigned == null || assigned.Count == 0) return 0f;
            float total = 0f;
            foreach (var name in assigned)
            {
                if (string.IsNullOrWhiteSpace(name)) continue;
                var key = name.Trim().ToLowerInvariant();
                if (key.Contains("laundromat")) total += 2000f;
                else if (key.Contains("post") || key.Contains("office")) total += 4000f;
                else if (key.Contains("car") && key.Contains("wash")) total += 6000f;
                else if (key.Contains("taco") || key.Contains("tickler")) total += 8000f;
            }
            return total;
        }
    }
}
