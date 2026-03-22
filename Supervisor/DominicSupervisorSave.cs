using System.Collections.Generic;
using S1API.Internal.Abstraction;
using S1API.Saveables;

namespace MoreNPCs.Supervisor
{
    /// <summary>Persists Dominic's assigned dealers and stored earnings.</summary>
    public sealed class DominicSupervisorSave : Saveable
    {
        public static DominicSupervisorSave? Instance { get; private set; }
        private static float _runtimeStoredCash;
        private static List<string> _runtimeAssignedDealerIds = new List<string>();

        [SaveableField("assigned_dealers")]
        private List<string> _assignedDealerIds = new List<string>();

        [SaveableField("stored_cash")]
        private float _storedCash;

        public IReadOnlyList<string> AssignedDealerIds =>
            Instance != null ? (IReadOnlyList<string>)_assignedDealerIds : _runtimeAssignedDealerIds;

        public static IReadOnlyList<string> GetAssignedDealerIds() =>
            Instance?.AssignedDealerIds ?? (IReadOnlyList<string>)(_runtimeAssignedDealerIds ?? new List<string>());

        public float StoredCash { get => _storedCash; set => _storedCash = value; }

        public static float GetStoredCashTotal() =>
            Instance != null ? Instance.StoredCash : _runtimeStoredCash;

        public static void AddToStoredCashStatic(float amount)
        {
            if (Instance != null) Instance._storedCash += amount;
            else _runtimeStoredCash += amount;
        }

        public static float TakeAllStoredCashStatic()
        {
            if (Instance != null) { var v = Instance._storedCash; Instance._storedCash = 0; return v; }
            var r = _runtimeStoredCash; _runtimeStoredCash = 0; return r;
        }

        public void AssignDealer(string dealerId)
        {
            var list = Instance != null ? _assignedDealerIds : _runtimeAssignedDealerIds;
            if (!list.Contains(dealerId) && list.Count < MaxAssignedDealers) list.Add(dealerId);
        }

        public void UnassignDealer(string dealerId)
        {
            if (Instance != null) _assignedDealerIds.Remove(dealerId);
            else _runtimeAssignedDealerIds?.Remove(dealerId);
        }

        public static void AssignDealerRuntime(string dealerId)
        {
            if (_runtimeAssignedDealerIds == null) _runtimeAssignedDealerIds = new List<string>();
            if (!_runtimeAssignedDealerIds.Contains(dealerId) && _runtimeAssignedDealerIds.Count < MaxAssignedDealers)
                _runtimeAssignedDealerIds.Add(dealerId);
        }

        public static void UnassignDealerRuntime(string dealerId) => _runtimeAssignedDealerIds?.Remove(dealerId);

        public bool IsAssigned(string dealerId) =>
            (Instance != null ? _assignedDealerIds : _runtimeAssignedDealerIds)?.Contains(dealerId) ?? false;

        public static bool IsAssignedStatic(string dealerId) =>
            Instance?.IsAssigned(dealerId) ?? (_runtimeAssignedDealerIds?.Contains(dealerId) ?? false);

        public DominicSupervisorSave() => Instance = this;
        protected override void OnCreated() { base.OnCreated(); Instance = this; }
        protected override void OnLoaded()
        {
            base.OnLoaded();
            if (_runtimeStoredCash > 0) { _storedCash += _runtimeStoredCash; _runtimeStoredCash = 0; }
            if (_runtimeAssignedDealerIds?.Count > 0)
            {
                foreach (var id in _runtimeAssignedDealerIds)
                {
                    if (!_assignedDealerIds.Contains(id) && _assignedDealerIds.Count < MaxAssignedDealers)
                        _assignedDealerIds.Add(id);
                }
                _runtimeAssignedDealerIds.Clear();
            }
        }
        protected override void OnDestroyed() { Instance = null; base.OnDestroyed(); }

        public const int MaxAssignedDealers = 6;
    }
}
