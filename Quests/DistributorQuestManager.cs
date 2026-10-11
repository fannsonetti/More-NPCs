using System.Collections.Generic;
using System.Reflection;
using MelonLoader;
using MoreNPCs.Distributor;
using MoreNPCs.Utils;
using S1API.Quests;
using UnityEngine;

namespace MoreNPCs.Quests
{
    public static class DistributorQuestManager
    {
        private const string DealGuid = "morenpcs_distributor_deal";
        private const string DealName = "Deal for Richard Roe";

        private static DistributorDealQuest? _cachedDealQuest;

        public static void Initialize() => _ = GetDealQuest();

        public static DistributorDealQuest? GetDealQuest() => GetOrCreateDealQuest();

        public static void StartDeal(bool restore = false)
        {
            DistributorWeeklyOrder.EnsureCurrentWeekOrder();
            var quest = GetDealQuest();
            if (quest == null) return;

            int deadlineDay = DistributorRunSave.DeadlineDay;
            int deadlineTime = DistributorRunSave.DeadlineTime24;
            int qty = DistributorWeeklyOrder.TargetQuantity;
            string drug = DistributorWeeklyOrder.GetDrugDisplayName();
            var waypoint = DistributorConfig.DropPosition;

            if (restore)
                quest.RestoreDeal(deadlineDay, deadlineTime, qty, drug, waypoint);
            else
                quest.StartDeal(deadlineDay, deadlineTime, qty, drug, waypoint);
        }

        public static void CompleteDeal() => GetDealQuest()?.CompleteDeal();

        public static void FailDeal() => GetDealQuest()?.FailDeal();

        private static DistributorDealQuest? GetOrCreateDealQuest()
        {
            if (_cachedDealQuest != null && QuestManagerQuests.Contains(_cachedDealQuest))
                return _cachedDealQuest;
            _cachedDealQuest = null;

            var byName = S1API.Quests.QuestManager.GetQuestByName(DealName);
            if (byName is DistributorDealQuest named)
            {
                _cachedDealQuest = named;
                return named;
            }

            for (int i = 0; i < QuestManagerQuests.Count; i++)
            {
                if (QuestManagerQuests[i] is DistributorDealQuest found)
                {
                    _cachedDealQuest = found;
                    return found;
                }
            }

            var created = S1API.Quests.QuestManager.CreateQuest<DistributorDealQuest>(DealGuid);
            if (created is DistributorDealQuest deal)
            {
                _cachedDealQuest = deal;
                return deal;
            }

            MelonLogger.Error("[DistributorQuestManager] Failed to create DistributorDealQuest");
            return null;
        }

        private static List<Quest> QuestManagerQuests => (List<Quest>)typeof(S1API.Quests.QuestManager)
            .GetField("Quests", BindingFlags.NonPublic | BindingFlags.Static)!
            .GetValue(null)!;
    }
}
