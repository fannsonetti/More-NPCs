using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using MelonLoader;
using S1API.Entities;
using S1API.Internal.Abstraction;
using S1API.Saveables;
using UnityEngine;

namespace MoreNPCs.Persistence
{
    /// <summary>
    /// Persists modded dealer cash across saves. Vanilla dealer cash often drops for custom dealers;
    /// we snapshot into <c>dealer_cash.json</c> on save and reapply on load.
    /// </summary>
    public sealed class DealersCashSave : Saveable
    {
        public static DealersCashSave? Instance { get; private set; }

        [Serializable]
        public sealed class Entry
        {
            public string Id = string.Empty;
            public float Cash;
        }

        [SaveableField("dealer_cash")]
        internal List<Entry> Entries = new List<Entry>();

        private static List<Entry> _runtimePending = new List<Entry>();
        private static bool _applyRoutineRunning;

        public DealersCashSave() => Instance = this;

        protected override void OnCreated()
        {
            base.OnCreated();
            Instance = this;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            if (_runtimePending.Count > 0 && Entries.Count == 0)
                Entries = new List<Entry>(_runtimePending);
            _runtimePending.Clear();

            if (!_applyRoutineRunning)
                MelonCoroutines.Start(ApplyWhenReady());
        }

        protected override void OnDestroyed()
        {
            Instance = null;
            base.OnDestroyed();
        }

        /// <summary>Call from a save Prefix so fields are current before JSON write.</summary>
        public static void CaptureFromWorld()
        {
            var snapshot = BuildSnapshot();
            if (Instance != null)
                Instance.Entries = snapshot;
            else
                _runtimePending = snapshot;
        }

        public static void UpdateThrottled()
        {
            if (Instance == null) return;
            if (!NPC.CustomNpcsReady) return;
            if (Time.frameCount % 300 != 0) return;
            Instance.Entries = BuildSnapshot();
        }

        private static IEnumerator ApplyWhenReady()
        {
            _applyRoutineRunning = true;
            try
            {
                for (int i = 0; i < 120; i++)
                {
                    if (NPC.CustomNpcsReady && TryApply(Instance?.Entries))
                        yield break;
                    yield return new WaitForSeconds(0.5f);
                }

                MelonLogger.Warning("DealersCashSave: timed out waiting to apply dealer cash.");
            }
            finally
            {
                _applyRoutineRunning = false;
            }
        }

        private static List<Entry> BuildSnapshot()
        {
            var list = new List<Entry>();
            try
            {
                var all = NPC.All;
                if (all == null) return list;

                foreach (var npc in all)
                {
                    if (npc == null || !npc.IsDealer) continue;
                    var id = npc.ID;
                    if (string.IsNullOrEmpty(id)) continue;

                    var cash = TryGetCash(npc);
                    if (cash < 0f) continue;

                    list.Add(new Entry { Id = id, Cash = cash });
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"DealersCashSave.BuildSnapshot: {ex.Message}");
            }

            return list;
        }

        private static bool TryApply(List<Entry>? entries)
        {
            if (entries == null || entries.Count == 0) return true;

            int applied = 0;
            int missing = 0;
            foreach (var entry in entries)
            {
                if (entry == null || string.IsNullOrEmpty(entry.Id)) continue;

                var npc = FindDealer(entry.Id);
                if (npc == null)
                {
                    missing++;
                    continue;
                }

                if (TrySetCash(npc, entry.Cash))
                    applied++;
            }

            if (missing > 0 && applied == 0)
                return false;

            MelonLogger.Msg("DealersCashSave: restored cash on {0} dealer(s).", applied);
            return true;
        }

        private static NPC? FindDealer(string id)
        {
            var all = NPC.All;
            if (all == null) return null;
            foreach (var npc in all)
            {
                if (npc == null || !npc.IsDealer) continue;
                if (string.Equals(npc.ID, id, StringComparison.OrdinalIgnoreCase))
                    return npc;
            }

            return null;
        }

        private static float TryGetCash(NPC npc)
        {
            var comp = GetDealerComponent(npc);
            if (comp == null) return -1f;

            var t = comp.GetType();
            var getCash = t.GetMethod("GetCash", BindingFlags.Public | BindingFlags.Instance);
            if (getCash != null && getCash.GetParameters().Length == 0)
            {
                try { return Convert.ToSingle(getCash.Invoke(comp, null)); }
                catch { }
            }

            var cashProp = t.GetProperty("Cash", BindingFlags.Public | BindingFlags.Instance);
            if (cashProp != null)
            {
                try { return Convert.ToSingle(cashProp.GetValue(comp)); }
                catch { }
            }

            return -1f;
        }

        private static bool TrySetCash(NPC npc, float amount)
        {
            var comp = GetDealerComponent(npc);
            if (comp == null) return false;

            var t = comp.GetType();
            var setCash = t.GetMethod("SetCash", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(float) }, null);
            if (setCash != null)
            {
                try { setCash.Invoke(comp, new object[] { amount }); return true; }
                catch { }
            }

            var cashProp = t.GetProperty("Cash", BindingFlags.Public | BindingFlags.Instance);
            if (cashProp != null && cashProp.CanWrite)
            {
                try { cashProp.SetValue(comp, amount); return true; }
                catch { }
            }

            return false;
        }

        private static object? GetDealerComponent(NPC npc)
        {
            try
            {
                if (npc.Dealer?.IsDealer == true)
                {
                    var field = npc.Dealer.GetType().GetProperty("Component", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    var viaApi = field?.GetValue(npc.Dealer);
                    if (viaApi != null) return viaApi;
                }
            }
            catch { }

            try
            {
                var go = npc.gameObject;
                if (go == null) return null;
                foreach (var behaviour in go.GetComponents<Behaviour>())
                {
                    if (behaviour == null) continue;
                    var name = behaviour.GetType().Name;
                    if (name == "Dealer" || name.EndsWith("Dealer", StringComparison.Ordinal))
                        return behaviour;
                }
            }
            catch { }

            return null;
        }
    }
}
