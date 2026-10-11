using System.Collections.Generic;

using MoreNPCs.Utils;

using S1API.Internal.Abstraction;

using S1API.Saveables;



namespace MoreNPCs.Persistence

{

    /// <summary>All distributor state in one save file (order + active drop deal).</summary>

    public sealed class DistributorSave : Saveable

    {

        public static DistributorSave? Instance { get; private set; }



        [SaveableField("run_venue")]

        internal int RunVenue;



        [SaveableField("run_day")]

        internal int RunDay = -1;



        [SaveableField("window_end")]

        internal int WindowEnd;



        [SaveableField("deadline_day")]

        internal int DeadlineDay = -1;



        [SaveableField("deadline_time")]

        internal int DeadlineTime = 401;



        [SaveableField("sold_this_run")]

        internal bool SoldThisRun;



        [SaveableField("days_without_event")]

        internal int DaysWithoutEvent;



        [SaveableField("order_week")]

        internal int OrderWeek = -1;



        [SaveableField("order_drug")]

        internal int OrderDrug;



        [SaveableField("order_quantity")]

        internal int OrderQuantity;



        [SaveableField("order_kind")]

        internal int OrderKind;



        [SaveableField("order_required_props")]

        internal List<string> OrderRequiredProps = new List<string>();



        [SaveableField("order_forbidden_prop")]

        internal string OrderForbiddenProp = string.Empty;



        public DistributorSave() => Instance = this;



        protected override void OnCreated()

        {

            base.OnCreated();

            Instance = this;

        }



        protected override void OnLoaded()

        {

            base.OnLoaded();

            MigrateFromLegacyModSave();

            Distributor.DistributorRunSave.SyncFromLoaded(this);

            Distributor.DistributorWeeklyOrder.SyncFromLoaded(this);

        }



        protected override void OnDestroyed()

        {

            Instance = null;

            base.OnDestroyed();

        }



        private void MigrateFromLegacyModSave()

        {

            var legacy = MoreNPCsModSave.Instance;

            if (legacy == null) return;

            if (OrderWeek >= 0 && OrderQuantity > 0) return;



            if (legacy._distributorOrderWeek >= 0)

            {

                OrderWeek = legacy._distributorOrderWeek;

                OrderDrug = legacy._distributorOrderDrug;

                OrderQuantity = legacy._distributorOrderQuantity;

                OrderKind = legacy._distributorOrderKind;

                OrderRequiredProps = legacy._distributorOrderRequiredProps != null

                    ? new List<string>(legacy._distributorOrderRequiredProps)

                    : new List<string>();

                OrderForbiddenProp = legacy._distributorOrderForbiddenProp ?? string.Empty;

            }



            if (legacy._distributorRunVenue != (int)Distributor.DistributorVenue.None)

            {

                RunVenue = legacy._distributorRunVenue;

                RunDay = legacy._distributorRunDay;

                WindowEnd = legacy._distributorWindowEnd;

                DeadlineDay = legacy._distributorDeadlineDay;

                DeadlineTime = legacy._distributorDeadlineTime > 0 ? legacy._distributorDeadlineTime : 401;

                SoldThisRun = legacy._distributorSoldThisRun;

                DaysWithoutEvent = legacy._distributorDaysWithoutEvent;

            }

        }

    }

}


