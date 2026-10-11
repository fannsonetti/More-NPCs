using System;

using MoreNPCs.Persistence;
using MoreNPCs.Utils;



namespace MoreNPCs.Distributor

{

    public static class DistributorRunSave

    {

        private static DistributorVenue _runtimeVenue = DistributorVenue.None;

        private static int _runtimeEventDay = -1;

        private static int _runtimeDeadlineDay = -1;

        private static int _runtimeDeadlineTime24 = 401;

        private static bool _runtimeSoldThisRun;

        private static int _runtimeDaysWithoutEvent;



        public static DistributorVenue ActiveVenue => _runtimeVenue;

        public static int EventDay => _runtimeEventDay;

        public static int DeadlineDay => _runtimeDeadlineDay;

        public static int DeadlineTime24 => _runtimeDeadlineTime24;

        public static bool SoldThisRun => _runtimeSoldThisRun;

        public static int DaysWithoutEvent => _runtimeDaysWithoutEvent;



        public static bool IsDealActive()

        {

            if (_runtimeVenue == DistributorVenue.None || _runtimeSoldThisRun)

                return false;

            if (_runtimeDeadlineDay < 0)

                return false;

            return !IsDealExpired();

        }



        /// <summary>Legacy alias.</summary>

        public static bool IsRunActive() => IsDealActive();



        public static bool IsDealExpired() =>

            GameTimeHelper.GetMinutesUntil(_runtimeDeadlineDay, _runtimeDeadlineTime24) <= 0;



        /// <summary>Legacy alias.</summary>

        public static bool IsWindowOpen() => IsDealActive();



        public static void BeginDeal(int deadlineDay, int deadlineTime24)

        {

            _runtimeVenue = DistributorVenue.Meetup;

            _runtimeEventDay = GameTimeHelper.ElapsedDays;

            _runtimeDeadlineDay = deadlineDay;

            _runtimeDeadlineTime24 = deadlineTime24;

            _runtimeSoldThisRun = false;

            _runtimeDaysWithoutEvent = 0;

            PersistRuntime();

        }



        /// <summary>Legacy — maps window end to deadline if needed.</summary>

        public static void BeginRun(DistributorVenue venue, int windowEndTime24)

        {

            int deadlineDay = GameTimeHelper.ElapsedDays + Math.Max(1, DistributorConfig.DealDeadlineDays);

            BeginDeal(deadlineDay, 401);

        }



        public static void MarkSoldThisRun()

        {

            _runtimeSoldThisRun = true;

            PersistRuntime();

        }



        public static void ClearRun()

        {

            _runtimeVenue = DistributorVenue.None;

            _runtimeEventDay = -1;

            _runtimeDeadlineDay = -1;

            _runtimeDeadlineTime24 = 401;

            _runtimeSoldThisRun = false;

            PersistRuntime();

        }



        public static void IncrementDaysWithoutEvent()

        {

            _runtimeDaysWithoutEvent++;

            PersistRuntime();

        }



        internal static void DebugResetSoldFlag()

        {

            _runtimeSoldThisRun = false;

            PersistRuntime();

        }



        internal static void SyncFromLoaded(DistributorSave save)

        {

            _runtimeVenue = (DistributorVenue)save.RunVenue;

            _runtimeEventDay = save.RunDay;

            _runtimeDeadlineDay = save.DeadlineDay >= 0 ? save.DeadlineDay : InferDeadlineDayFromLegacy(save);

            _runtimeDeadlineTime24 = save.DeadlineTime > 0 ? save.DeadlineTime : 401;

            _runtimeSoldThisRun = save.SoldThisRun;

            _runtimeDaysWithoutEvent = save.DaysWithoutEvent;

        }



        internal static void SyncFromLoaded(MoreNPCs.Persistence.MoreNPCsModSave save)

        {

            _runtimeVenue = (DistributorVenue)save._distributorRunVenue;

            _runtimeEventDay = save._distributorRunDay;

            _runtimeDeadlineDay = save._distributorDeadlineDay >= 0

                ? save._distributorDeadlineDay

                : InferDeadlineDayFromLegacy(save._distributorRunDay, save._distributorWindowEnd);

            _runtimeDeadlineTime24 = save._distributorDeadlineTime > 0 ? save._distributorDeadlineTime : 401;

            _runtimeSoldThisRun = save._distributorSoldThisRun;

            _runtimeDaysWithoutEvent = save._distributorDaysWithoutEvent;

        }



        private static int InferDeadlineDayFromLegacy(DistributorSave save)

        {

            if (save.RunDay < 0) return -1;

            if (save.WindowEnd > 0) return save.RunDay + Math.Max(1, DistributorConfig.DealDeadlineDays);

            return save.RunDay + Math.Max(1, DistributorConfig.DealDeadlineDays);

        }



        private static int InferDeadlineDayFromLegacy(int runDay, int windowEnd)

        {

            if (runDay < 0) return -1;

            return runDay + Math.Max(1, DistributorConfig.DealDeadlineDays);

        }



        private static void PersistRuntime()

        {

            if (DistributorSave.Instance != null)

            {

                DistributorSave.Instance.RunVenue = (int)_runtimeVenue;

                DistributorSave.Instance.RunDay = _runtimeEventDay;

                DistributorSave.Instance.DeadlineDay = _runtimeDeadlineDay;

                DistributorSave.Instance.DeadlineTime = _runtimeDeadlineTime24;

                DistributorSave.Instance.WindowEnd = _runtimeDeadlineTime24;

                DistributorSave.Instance.SoldThisRun = _runtimeSoldThisRun;

                DistributorSave.Instance.DaysWithoutEvent = _runtimeDaysWithoutEvent;

                return;

            }



            if (MoreNPCs.Persistence.MoreNPCsModSave.Instance == null) return;

            MoreNPCs.Persistence.MoreNPCsModSave.Instance._distributorRunVenue = (int)_runtimeVenue;

            MoreNPCs.Persistence.MoreNPCsModSave.Instance._distributorRunDay = _runtimeEventDay;

            MoreNPCs.Persistence.MoreNPCsModSave.Instance._distributorDeadlineDay = _runtimeDeadlineDay;

            MoreNPCs.Persistence.MoreNPCsModSave.Instance._distributorDeadlineTime = _runtimeDeadlineTime24;

            MoreNPCs.Persistence.MoreNPCsModSave.Instance._distributorWindowEnd = _runtimeDeadlineTime24;

            MoreNPCs.Persistence.MoreNPCsModSave.Instance._distributorSoldThisRun = _runtimeSoldThisRun;

            MoreNPCs.Persistence.MoreNPCsModSave.Instance._distributorDaysWithoutEvent = _runtimeDaysWithoutEvent;

        }

    }

}


