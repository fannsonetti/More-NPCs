using System;
using System.Collections.Generic;
using S1API.Internal.Abstraction;
using S1API.Saveables;

namespace MoreNPCs.Manager
{
    /// <summary>Persists which businesses the manager is managing. Max 4. Earns passive income daily.</summary>
    public sealed class ManagerBusinessSave : Saveable
    {
        public static ManagerBusinessSave? Instance { get; private set; }
        private static readonly List<string> _runtimeAssigned = new List<string>();
        public const int MaxAssignedBusinesses = 4;

        [SaveableField("manager_businesses")]
        private List<string> _assignedBusinesses = new List<string>();

        public IReadOnlyList<string> AssignedBusinessNames =>
            Instance != null ? (IReadOnlyList<string>)_assignedBusinesses : _runtimeAssigned;

        public static IReadOnlyList<string> GetAssignedStatic() =>
            Instance?.AssignedBusinessNames ?? (IReadOnlyList<string>)_runtimeAssigned;

        public static void AssignStatic(string businessName)
        {
            var list = Instance != null ? Instance._assignedBusinesses : _runtimeAssigned;
            if (list.Count >= MaxAssignedBusinesses) return;
            if (!list.Contains(businessName)) list.Add(businessName);
        }

        public static void UnassignStatic(string businessName)
        {
            if (Instance != null) Instance._assignedBusinesses.Remove(businessName);
            else _runtimeAssigned?.Remove(businessName);
        }

        public static bool IsAssignedStatic(string businessName)
        {
            if (string.IsNullOrWhiteSpace(businessName)) return false;
            var list = Instance?.AssignedBusinessNames ?? (IReadOnlyList<string>)_runtimeAssigned;
            if (list == null) return false;
            var key = businessName.Trim();
            foreach (var n in list)
                if (string.Equals(n?.Trim(), key, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        public ManagerBusinessSave() => Instance = this;
        protected override void OnCreated() { base.OnCreated(); Instance = this; }
        protected override void OnLoaded()
        {
            base.OnLoaded();
            if (_runtimeAssigned?.Count > 0)
            {
                foreach (var name in _runtimeAssigned)
                {
                    if (!_assignedBusinesses.Contains(name) && _assignedBusinesses.Count < MaxAssignedBusinesses)
                        _assignedBusinesses.Add(name);
                }
                _runtimeAssigned.Clear();
            }
        }
        protected override void OnDestroyed() { Instance = null; base.OnDestroyed(); }
    }
}
