using System.Collections.Generic;
using MoreNPCs.Distributor;
using MoreNPCs.Supervisor;
using MoreNPCs.Utils;
using S1API.Internal.Abstraction;
using S1API.Saveables;

namespace MoreNPCs.Persistence
{
    /// <summary>
    /// Single save slot for this mod (one folder / JSON bundle under the game’s mod save data).
    /// Holds: one-time unlock intro texts, Silas’s dealer network, and Dominic’s dealer network.
    /// </summary>
    public sealed class MoreNPCsModSave : Saveable
    {
        public static MoreNPCsModSave? Instance { get; private set; }

        public static new void RequestGameSave() => Saveable.RequestGameSave();

        private static readonly HashSet<string> _runtimeUnlockIds =
            new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);

        private static float _runtimeSilasCash;
        private static List<string> _runtimeSilasDealers = new List<string>();
        private static float _runtimeDominicCash;
        private static List<string> _runtimeDominicDealers = new List<string>();

        [SaveableField("npc_unlock_intro_text_sent_ids")]
        private List<string> _unlockIntroTextSentNpcIds = new List<string>();

        [SaveableField("silas_assigned_dealers")]
        internal List<string> _silasAssignedDealerIds = new List<string>();

        [SaveableField("silas_stored_cash")]
        internal float _silasStoredCash;

        [SaveableField("dominic_assigned_dealers")]
        internal List<string> _dominicAssignedDealerIds = new List<string>();

        [SaveableField("dominic_stored_cash")]
        internal float _dominicStoredCash;

        [SaveableField("distributor_run_venue")]
        internal int _distributorRunVenue;

        [SaveableField("distributor_run_day")]
        internal int _distributorRunDay = -1;

        [SaveableField("distributor_window_end")]
        internal int _distributorWindowEnd;

        [SaveableField("distributor_deadline_day")]
        internal int _distributorDeadlineDay = -1;

        [SaveableField("distributor_deadline_time")]
        internal int _distributorDeadlineTime = 401;

        [SaveableField("distributor_sold_this_run")]
        internal bool _distributorSoldThisRun;

        [SaveableField("distributor_days_without_event")]
        internal int _distributorDaysWithoutEvent;

        [SaveableField("distributor_order_week")]
        internal int _distributorOrderWeek = -1;

        [SaveableField("distributor_order_drug")]
        internal int _distributorOrderDrug;

        [SaveableField("distributor_order_quantity")]
        internal int _distributorOrderQuantity;

        [SaveableField("distributor_order_kind")]
        internal int _distributorOrderKind;

        [SaveableField("distributor_order_required_props")]
        internal List<string> _distributorOrderRequiredProps = new List<string>();

        [SaveableField("distributor_order_forbidden_prop")]
        internal string _distributorOrderForbiddenProp = string.Empty;

        /// <summary>
        /// Tracks which NPCs have already received their post-unlock intro text (SMS), so it only sends once per NPC.
        /// </summary>
        public static class NpcUnlockIntroTexts
        {
            public static bool HasBeenSentFor(string npcId)
            {
                if (string.IsNullOrEmpty(npcId)) return false;
                if (Instance != null)
                {
                    foreach (var id in Instance._unlockIntroTextSentNpcIds)
                        if (string.Equals(id, npcId, System.StringComparison.OrdinalIgnoreCase)) return true;
                    return false;
                }
                lock (_runtimeUnlockIds) return _runtimeUnlockIds.Contains(npcId);
            }

            public static void MarkSentFor(string npcId)
            {
                if (string.IsNullOrEmpty(npcId)) return;
                if (Instance != null)
                {
                    if (!Instance._unlockIntroTextSentNpcIds.Contains(npcId))
                        Instance._unlockIntroTextSentNpcIds.Add(npcId);
                }
                else
                    lock (_runtimeUnlockIds) _runtimeUnlockIds.Add(npcId);
                Saveable.RequestGameSave();
            }
        }

        public static class SilasCartelSupervisor
        {
            public static int MaxAssignedDealers => SupervisorManager.MaxAssignedDealers;

            public static IReadOnlyList<string> GetAssignedDealerIds() =>
                SupervisorsSave.Instance != null
                    ? (IReadOnlyList<string>)SupervisorsSave.Instance.SilasDealerIds
                    : (IReadOnlyList<string>)(SupervisorsSave.RuntimeSilasDealers ?? new List<string>());

            public static float GetStoredCashTotal() =>
                SupervisorsSave.Instance != null
                    ? SupervisorsSave.Instance.SilasStoredCash
                    : SupervisorsSave.RuntimeSilasCash;

            public static void AddToStoredCashStatic(float amount)
            {
                if (SupervisorsSave.Instance != null) SupervisorsSave.Instance.SilasStoredCash += amount;
                else SupervisorsSave.RuntimeSilasCash += amount;
            }

            public static float TakeAllStoredCashStatic()
            {
                if (SupervisorsSave.Instance != null)
                {
                    var v = SupervisorsSave.Instance.SilasStoredCash;
                    SupervisorsSave.Instance.SilasStoredCash = 0;
                    return v;
                }
                var r = SupervisorsSave.RuntimeSilasCash;
                SupervisorsSave.RuntimeSilasCash = 0;
                return r;
            }

            public static void AssignDealerRuntime(string dealerId)
            {
                var list = SupervisorsSave.GetOrCreateRuntimeSilasDealers();
                if (!list.Contains(dealerId) && list.Count < MaxAssignedDealers)
                    list.Add(dealerId);
            }

            public static void UnassignDealerRuntime(string dealerId) =>
                SupervisorsSave.GetOrCreateRuntimeSilasDealers().Remove(dealerId);

            public static bool IsAssignedStatic(string dealerId) =>
                SupervisorsSave.Instance != null
                    ? SupervisorsSave.Instance.SilasDealerIds.Contains(dealerId)
                    : (SupervisorsSave.RuntimeSilasDealers?.Contains(dealerId) ?? false);
        }

        /// <summary>Dominic: assigned dealers and cash he is holding for the player.</summary>
        public static class DominicCartelSupervisor
        {
            public static int MaxAssignedDealers => SupervisorManager.MaxAssignedDealers;

            public static IReadOnlyList<string> GetAssignedDealerIds() =>
                SupervisorsSave.Instance != null
                    ? (IReadOnlyList<string>)SupervisorsSave.Instance.DominicDealerIds
                    : (IReadOnlyList<string>)(SupervisorsSave.RuntimeDominicDealers ?? new List<string>());

            public static float GetStoredCashTotal() =>
                SupervisorsSave.Instance != null
                    ? SupervisorsSave.Instance.DominicStoredCash
                    : SupervisorsSave.RuntimeDominicCash;

            public static void AddToStoredCashStatic(float amount)
            {
                if (SupervisorsSave.Instance != null) SupervisorsSave.Instance.DominicStoredCash += amount;
                else SupervisorsSave.RuntimeDominicCash += amount;
            }

            public static float TakeAllStoredCashStatic()
            {
                if (SupervisorsSave.Instance != null)
                {
                    var v = SupervisorsSave.Instance.DominicStoredCash;
                    SupervisorsSave.Instance.DominicStoredCash = 0;
                    return v;
                }
                var r = SupervisorsSave.RuntimeDominicCash;
                SupervisorsSave.RuntimeDominicCash = 0;
                return r;
            }

            public static void AssignDealerRuntime(string dealerId)
            {
                var list = SupervisorsSave.GetOrCreateRuntimeDominicDealers();
                if (!list.Contains(dealerId) && list.Count < MaxAssignedDealers)
                    list.Add(dealerId);
            }

            public static void UnassignDealerRuntime(string dealerId) =>
                SupervisorsSave.GetOrCreateRuntimeDominicDealers().Remove(dealerId);

            public static bool IsAssignedStatic(string dealerId) =>
                SupervisorsSave.Instance != null
                    ? SupervisorsSave.Instance.DominicDealerIds.Contains(dealerId)
                    : (SupervisorsSave.RuntimeDominicDealers?.Contains(dealerId) ?? false);
        }

        public void SilasAssignDealer(string dealerId)
        {
            if (SupervisorsSave.Instance != null)
            {
                if (!SupervisorsSave.Instance.SilasDealerIds.Contains(dealerId)
                    && SupervisorsSave.Instance.SilasDealerIds.Count < SilasCartelSupervisor.MaxAssignedDealers)
                    SupervisorsSave.Instance.SilasDealerIds.Add(dealerId);
                return;
            }
            if (!_silasAssignedDealerIds.Contains(dealerId) && _silasAssignedDealerIds.Count < SilasCartelSupervisor.MaxAssignedDealers)
                _silasAssignedDealerIds.Add(dealerId);
        }

        public void SilasUnassignDealer(string dealerId)
        {
            if (SupervisorsSave.Instance != null) SupervisorsSave.Instance.SilasDealerIds.Remove(dealerId);
            else _silasAssignedDealerIds.Remove(dealerId);
        }

        public void DominicAssignDealer(string dealerId)
        {
            if (SupervisorsSave.Instance != null)
            {
                if (!SupervisorsSave.Instance.DominicDealerIds.Contains(dealerId)
                    && SupervisorsSave.Instance.DominicDealerIds.Count < DominicCartelSupervisor.MaxAssignedDealers)
                    SupervisorsSave.Instance.DominicDealerIds.Add(dealerId);
                return;
            }
            if (!_dominicAssignedDealerIds.Contains(dealerId) && _dominicAssignedDealerIds.Count < DominicCartelSupervisor.MaxAssignedDealers)
                _dominicAssignedDealerIds.Add(dealerId);
        }

        public void DominicUnassignDealer(string dealerId)
        {
            if (SupervisorsSave.Instance != null) SupervisorsSave.Instance.DominicDealerIds.Remove(dealerId);
            else _dominicAssignedDealerIds.Remove(dealerId);
        }

        public MoreNPCsModSave() => Instance = this;

        protected override void OnCreated()
        {
            base.OnCreated();
            Instance = this;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            lock (_runtimeUnlockIds)
            {
                foreach (var id in _runtimeUnlockIds)
                    if (!_unlockIntroTextSentNpcIds.Contains(id)) _unlockIntroTextSentNpcIds.Add(id);
                _runtimeUnlockIds.Clear();
            }
            if (_runtimeSilasCash > 0)
            {
                if (SupervisorsSave.Instance != null) SupervisorsSave.Instance.SilasStoredCash += _runtimeSilasCash;
                else _silasStoredCash += _runtimeSilasCash;
                _runtimeSilasCash = 0;
            }
            if (_runtimeSilasDealers?.Count > 0)
            {
                var target = SupervisorsSave.Instance?.SilasDealerIds ?? _silasAssignedDealerIds;
                foreach (var id in _runtimeSilasDealers)
                {
                    if (!target.Contains(id) && target.Count < SilasCartelSupervisor.MaxAssignedDealers)
                        target.Add(id);
                }
                _runtimeSilasDealers.Clear();
            }
            if (_runtimeDominicCash > 0)
            {
                if (SupervisorsSave.Instance != null) SupervisorsSave.Instance.DominicStoredCash += _runtimeDominicCash;
                else _dominicStoredCash += _runtimeDominicCash;
                _runtimeDominicCash = 0;
            }
            if (_runtimeDominicDealers?.Count > 0)
            {
                var target = SupervisorsSave.Instance?.DominicDealerIds ?? _dominicAssignedDealerIds;
                foreach (var id in _runtimeDominicDealers)
                {
                    if (!target.Contains(id) && target.Count < DominicCartelSupervisor.MaxAssignedDealers)
                        target.Add(id);
                }
                _runtimeDominicDealers.Clear();
            }

            PoliceVehicleHelper.ScheduleTrunkFixAfterLoad();
        }

        protected override void OnDestroyed()
        {
            Instance = null;
            base.OnDestroyed();
        }
    }
}
