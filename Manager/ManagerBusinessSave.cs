using System;
using System.Collections.Generic;
using MoreNPCs.Utils;
using S1API.Internal.Abstraction;
using S1API.Saveables;
using UnityEngine;

namespace MoreNPCs.Manager
{
    /// <summary>
    /// Single saveable for manager progress: assigned businesses, funds, earnings, and vanilla NPC purchases.
    /// Keeps one mod data folder with one JSON bundle from this type (plus other Saveables elsewhere).
    /// </summary>
    public sealed class ManagerBusinessSave : Saveable
    {
        public static ManagerBusinessSave? Instance { get; private set; }
        private static readonly List<string> _runtimeAssigned = new List<string>();
        private static readonly List<string> _runtimeVanillaPurchased = new List<string>();
        private static float _runtimeStored;
        private static float _runtimeEarnings;

        public static int MaxAssignedBusinesses =>
            !MoreNPCsPreferences.Registered ? 4 : Math.Max(1, MoreNPCsPreferences.Manager_MaxAssignedBusinesses.Value);

        [SaveableField("manager_businesses")]
        private List<string> _assignedBusinesses = new List<string>();

        /// <summary>Business names bought from vanilla sellers (unlocks for Thomas assignment; separate from S1 property ownership).</summary>
        [SaveableField("vanilla_business_purchases")]
        private List<string> _vanillaPurchased = new List<string>();

        [SaveableField("manager_funds")]
        private float _storedFunds;

        [SaveableField("manager_earnings")]
        private float _businessEarnings;

        public IReadOnlyList<string> AssignedBusinessNames =>
            Instance != null ? (IReadOnlyList<string>)_assignedBusinesses : _runtimeAssigned;

        public float StoredFunds { get => _storedFunds; set => _storedFunds = value; }
        public float BusinessEarnings { get => _businessEarnings; set => _businessEarnings = value; }

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

        public static void MarkVanillaPurchased(string businessName)
        {
            if (string.IsNullOrWhiteSpace(businessName)) return;
            var key = businessName.Trim();
            var list = Instance != null ? Instance._vanillaPurchased : _runtimeVanillaPurchased;
            foreach (var n in list)
                if (string.Equals(n?.Trim(), key, StringComparison.OrdinalIgnoreCase)) return;
            list.Add(key);
        }

        public static bool IsVanillaPurchasedStatic(string businessName)
        {
            if (string.IsNullOrWhiteSpace(businessName)) return false;
            var key = businessName.Trim();
            var list = Instance != null ? Instance._vanillaPurchased : _runtimeVanillaPurchased;
            foreach (var n in list)
                if (string.Equals(n?.Trim(), key, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        /// <summary>Used when correcting legacy saves that marked an entire RE tier as purchased at once.</summary>
        public static void RemoveVanillaPurchasedStatic(string businessName)
        {
            if (string.IsNullOrWhiteSpace(businessName)) return;
            var key = businessName.Trim();
            var list = Instance != null ? Instance._vanillaPurchased : _runtimeVanillaPurchased;
            for (var i = list.Count - 1; i >= 0; i--)
            {
                if (string.Equals(list[i]?.Trim(), key, StringComparison.OrdinalIgnoreCase))
                    list.RemoveAt(i);
            }
        }

        public static IReadOnlyList<string> GetVanillaPurchasedNames()
        {
            if (Instance != null) return Instance._vanillaPurchased;
            return _runtimeVanillaPurchased;
        }

        public static float GetStored() => Instance != null ? Instance.StoredFunds : _runtimeStored;
        public static float GetBusinessEarnings() => Instance != null ? Instance.BusinessEarnings : _runtimeEarnings;

        public static void AddStatic(float amount)
        {
            if (Instance != null) Instance._storedFunds += amount;
            else _runtimeStored += amount;
        }

        public static void AddEarningsStatic(float amount)
        {
            if (Instance != null) Instance._businessEarnings += amount;
            else _runtimeEarnings += amount;
        }

        public static float TakeStatic(float maxAmount)
        {
            var available = Instance != null ? Instance._storedFunds : _runtimeStored;
            var take = Mathf.Min(available, maxAmount);
            if (Instance != null) Instance._storedFunds -= take;
            else _runtimeStored -= take;
            return take;
        }

        public static float TakeAllEarningsStatic()
        {
            var v = Instance != null ? Instance._businessEarnings : _runtimeEarnings;
            if (Instance != null) Instance._businessEarnings = 0;
            else _runtimeEarnings = 0;
            return v;
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
            if (_runtimeVanillaPurchased?.Count > 0)
            {
                foreach (var name in _runtimeVanillaPurchased)
                {
                    if (string.IsNullOrEmpty(name)) continue;
                    var dup = false;
                    foreach (var v in _vanillaPurchased)
                        if (string.Equals(v, name, StringComparison.OrdinalIgnoreCase)) { dup = true; break; }
                    if (!dup) _vanillaPurchased.Add(name);
                }
                _runtimeVanillaPurchased.Clear();
            }
            if (_runtimeStored > 0) { _storedFunds += _runtimeStored; _runtimeStored = 0; }
            if (_runtimeEarnings > 0) { _businessEarnings += _runtimeEarnings; _runtimeEarnings = 0; }
        }
        protected override void OnDestroyed() { Instance = null; base.OnDestroyed(); }
    }
}
