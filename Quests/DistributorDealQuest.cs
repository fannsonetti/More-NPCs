using System;
using System.Reflection;
using MelonLoader;
using MoreNPCs.Distributor;
using MoreNPCs.Utils;
using S1API.GameTime;
using S1API.Quests;
using UnityEngine;

namespace MoreNPCs.Quests
{
    /// <summary>Tracked wholesale drop deal — cartel-style HUD (subtitle timer + entry deliver line).</summary>
    public sealed class DistributorDealQuest : Quest
    {
        private const string QuestTitle = "Deal for Richard Roe";

        protected override string Title => QuestTitle;
        protected override string Description => "Anonymous wholesale drop.";
        protected override bool AutoBegin => false;
        protected override Sprite? QuestIcon => QuestIconLoader.Load("quest_deal.png");

        private bool _tickHooked;
        private bool _active;
        private int _deadlineDay;
        private int _deadlineTime24;
        private string _deliverLine = string.Empty;
        private int _lastRemainingMins = int.MinValue;
        private readonly Action _tickHandler;

        public bool IsActive => _active;

        public DistributorDealQuest()
        {
            _tickHandler = UpdateTimingOnTick;
        }

        public void StartDeal(int deadlineDay, int deadlineTime24, int quantity, string drugName, Vector3 waypoint)
        {
            _deadlineDay = deadlineDay;
            _deadlineTime24 = deadlineTime24;
            _deliverLine = BuildDeliverLine(quantity, drugName);
            _active = true;
            _lastRemainingMins = int.MinValue;

            for (int i = QuestEntries.Count - 1; i >= 0; i--)
                QuestEntries[i]?.Complete();

            AddEntry(_deliverLine, waypoint);
            ConfigureNativeExpiry(deadlineDay, deadlineTime24);
            TrySetTracked(true);
            Begin();
            if (QuestEntries.Count >= 1)
                QuestEntries[0].Begin();

            TrySetMainEntryTitle(_deliverLine);
            UpdateTimingSubtitle(force: true);
            EnsureTickHooked();
        }

        public void RestoreDeal(int deadlineDay, int deadlineTime24, int quantity, string drugName, Vector3 waypoint)
        {
            _deadlineDay = deadlineDay;
            _deadlineTime24 = deadlineTime24;
            _deliverLine = BuildDeliverLine(quantity, drugName);
            _active = true;
            _lastRemainingMins = int.MinValue;

            if (QuestEntries.Count == 0)
            {
                StartDeal(deadlineDay, deadlineTime24, quantity, drugName, waypoint);
                return;
            }

            ConfigureNativeExpiry(deadlineDay, deadlineTime24);
            TrySetMainEntryTitle(_deliverLine);
            UpdateTimingSubtitle(force: true);
            EnsureTickHooked();
        }

        public void CompleteDeal()
        {
            _active = false;
            if (QuestEntries.Count >= 1)
                QuestEntries[0].Complete();
            Complete();
        }

        public void FailDeal()
        {
            _active = false;
            TryExpireQuest();
        }

        private static string BuildDeliverLine(int quantity, string drugName) =>
            $"Deliver {quantity}x {drugName} to {DistributorTrawlerDrop.DisplayName}";

        private void EnsureTickHooked()
        {
            if (_tickHooked) return;
            TimeManager.OnTick += _tickHandler;
            _tickHooked = true;
        }

        private void UpdateTimingOnTick()
        {
            if (!_active) return;
            UpdateTimingSubtitle();
        }

        private void UpdateTimingSubtitle(bool force = false)
        {
            int remainingMins = GameTimeHelper.GetMinutesUntil(_deadlineDay, _deadlineTime24);
            if (!force && remainingMins == _lastRemainingMins) return;
            _lastRemainingMins = remainingMins;

            // Cartel quest pattern: subtitle carries the countdown under the quest title.
            TrySetSubtitle("\n" + FormatTimingLine(remainingMins));
            TrySetMainEntryTitle(_deliverLine);
            TryRefreshHud();
        }

        internal static string FormatTimingLine(int remainingMins)
        {
            if (remainingMins <= 0)
                return "<color=#FF5555>(0 hours remaining)</color>";

            int days = remainingMins / 1440;
            int hours = (remainingMins % 1440) / 60;
            if (hours == 0 && days == 0)
                hours = 1;

            string color = days > 0 ? "green" : "#F7B119";
            if (days > 0)
                return $"<color={color}>({days} day{(days == 1 ? "" : "s")}, {hours} hour{(hours == 1 ? "" : "s")} remaining)</color>";

            return $"<color={color}>({hours} hour{(hours == 1 ? "" : "s")} remaining)</color>";
        }

        private void ConfigureNativeExpiry(int deadlineDay, int deadlineTime24)
        {
            try
            {
                var s1Quest = GetS1Quest();
                if (s1Quest == null) return;

                var gdtType = FindType("ScheduleOne.GameTime.GameDateTime");
                if (gdtType == null) return;

                object expiry = Activator.CreateInstance(gdtType, deadlineDay, deadlineTime24)!;
                s1Quest.GetType()
                    .GetMethod("ConfigureExpiry", BindingFlags.Public | BindingFlags.Instance)
                    ?.Invoke(s1Quest, new object[] { true, expiry });
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"DistributorDealQuest.ConfigureNativeExpiry: {ex.Message}");
            }
        }

        private void TrySetTracked(bool tracked)
        {
            try
            {
                GetS1Quest()?.GetType()
                    .GetMethod("SetIsTracked", BindingFlags.Public | BindingFlags.Instance)
                    ?.Invoke(GetS1Quest(), new object[] { tracked });
            }
            catch { }
        }

        private void TrySetSubtitle(string text)
        {
            try
            {
                GetS1Quest()?.GetType()
                    .GetMethod("SetSubtitle", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.Invoke(GetS1Quest(), new object[] { text });
            }
            catch { }
        }

        private void TrySetMainEntryTitle(string title)
        {
            if (QuestEntries.Count < 1) return;

            try
            {
                var wrapper = QuestEntries[0];
                foreach (var fieldName in new[] { "S1QuestEntry", "_questEntry", "Entry", "QuestEntry" })
                {
                    var field = wrapper.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
                    var s1Entry = field?.GetValue(wrapper);
                    if (s1Entry == null) continue;

                    s1Entry.GetType()
                        .GetMethod("SetEntryTitle", BindingFlags.Public | BindingFlags.Instance)
                        ?.Invoke(s1Entry, new object[] { title });
                    return;
                }

                var titleProp = wrapper.GetType().GetProperty("Title", BindingFlags.Public | BindingFlags.Instance);
                if (titleProp?.CanWrite == true)
                    titleProp.SetValue(wrapper, title);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"DistributorDealQuest.SetMainEntryTitle: {ex.Message}");
            }
        }

        private void TryRefreshHud()
        {
            try
            {
                GetS1Quest()?.GetType()
                    .GetMethod("UpdateHUDUI", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.Invoke(GetS1Quest(), null);
            }
            catch { }
        }

        private void TryExpireQuest()
        {
            try
            {
                var s1Quest = GetS1Quest();
                var expire = s1Quest?.GetType().GetMethod("Expire", BindingFlags.Public | BindingFlags.Instance);
                if (expire != null)
                {
                    expire.Invoke(s1Quest, new object[] { true });
                    return;
                }
            }
            catch { }

            if (QuestEntries.Count >= 1)
                QuestEntries[0].Complete();
            Complete();
        }

        private object? GetS1Quest()
        {
            var type = typeof(Quest);
            var s1Field = type.GetField("S1Quest", BindingFlags.NonPublic | BindingFlags.Instance)
                ?? type.GetField("_quest", BindingFlags.NonPublic | BindingFlags.Instance);
            return s1Field?.GetValue(this);
        }

        private static Type? FindType(string fullName)
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
