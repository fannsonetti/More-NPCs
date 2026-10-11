using MelonLoader;
using MoreNPCs.NPCs;
using MoreNPCs.Quests;
using MoreNPCs.Utils;
using S1API.GameTime;

namespace MoreNPCs.Distributor
{
    /// <summary>Rolls distributor deal events after sleep and sends SMS.</summary>
    public static class DistributorRunScheduler
    {
        private static bool _subscribed;
        private static readonly System.Random Rng = new System.Random();

        public static void EnsureSubscribed()
        {
            if (_subscribed) return;
            try
            {
                TimeManager.OnDayPass += OnDayPassed;
                _subscribed = true;
            }
            catch (System.Exception ex)
            {
                MelonLogger.Warning($"DistributorRunScheduler subscribe: {ex.Message}");
            }
        }

        private static void OnDayPassed()
        {
            try
            {
                if (DistributorRunSave.IsDealActive() && DistributorRunSave.IsDealExpired())
                    ExpireActiveDeal();

                if (DistributorRunSave.ActiveVenue != DistributorVenue.None
                    && DistributorRunSave.SoldThisRun)
                {
                    DistributorDropBox.Disable();
                    DistributorRunSave.ClearRun();
                }

                var grant = DistributorRegistry.Grant;
                if (grant == null || !IsGrantUnlocked(grant)) return;
                if (DistributorRunSave.IsDealActive()) return;

                DistributorRunSave.IncrementDaysWithoutEvent();

                float chance = DistributorConfig.VisitChancePerSleep;
                int pityDays = DistributorConfig.PityDaysWithoutEvent;
                bool forceEvent = DistributorRunSave.DaysWithoutEvent >= pityDays;
                bool rollHit = forceEvent || Rng.NextDouble() < chance;
                if (!rollHit) return;

                StartDealForGrant(grant);
            }
            catch (System.Exception ex)
            {
                MelonLogger.Warning($"DistributorRunScheduler: {ex.Message}");
            }
        }

        internal static void ForceDeal(GrantEllison grant) => StartDealForGrant(grant, rerollOrder: false);

        private static void StartDealForGrant(GrantEllison grant, bool rerollOrder = true)
        {
            if (rerollOrder)
                DistributorWeeklyOrder.RollOrderForDeal();
            else if (!DistributorWeeklyOrder.HasCurrentOrder())
                DistributorWeeklyOrder.EnsureCurrentWeekOrder();

            int deadlineDay = GameTimeHelper.ElapsedDays + DistributorConfig.DealDeadlineDays;
            DistributorRunSave.BeginDeal(deadlineDay, 401);

            DistributorTextingSetup.SendDealOffer(grant);

            DistributorQuestManager.StartDeal();
            DistributorDropBox.Enable();
        }

        internal static void ExpireActiveDeal()
        {
            var grant = DistributorRegistry.GrantNpc;
            grant?.SendTextMessage(DistributorText.DealExpiredSms());
            DistributorDropBox.FailDeal();
        }

        private static bool IsGrantUnlocked(S1API.Entities.NPC grant) =>
            grant.Relationship?.IsUnlocked == true
            || MoreNPCs.Persistence.MoreNPCsModSave.NpcUnlockIntroTexts.HasBeenSentFor(DistributorIds.Grant)
            || MoreNPCs.Persistence.MoreNPCsModSave.NpcUnlockIntroTexts.HasBeenSentFor(DistributorIds.LegacyVinny);
    }
}
