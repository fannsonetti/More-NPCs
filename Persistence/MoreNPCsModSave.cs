using System.Collections.Generic;
using MoreNPCs.Supervisor;
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

        private static readonly HashSet<string> _runtimeUnlockIds =
            new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);

        private static float _runtimeSilasCash;
        private static List<string> _runtimeSilasDealers = new List<string>();
        private static float _runtimeDominicCash;
        private static List<string> _runtimeDominicDealers = new List<string>();

        [SaveableField("npc_unlock_intro_text_sent_ids")]
        private List<string> _unlockIntroTextSentNpcIds = new List<string>();

        [SaveableField("silas_assigned_dealers")]
        private List<string> _silasAssignedDealerIds = new List<string>();

        [SaveableField("silas_stored_cash")]
        private float _silasStoredCash;

        [SaveableField("dominic_assigned_dealers")]
        private List<string> _dominicAssignedDealerIds = new List<string>();

        [SaveableField("dominic_stored_cash")]
        private float _dominicStoredCash;

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

        /// <summary>Silas: assigned dealers and cash he is holding for the player.</summary>
        public static class SilasCartelSupervisor
        {
            public static int MaxAssignedDealers => SupervisorManager.MaxAssignedDealers;

            public static IReadOnlyList<string> GetAssignedDealerIds() =>
                Instance != null
                    ? (IReadOnlyList<string>)Instance._silasAssignedDealerIds
                    : (IReadOnlyList<string>)(_runtimeSilasDealers ?? new List<string>());

            public static float GetStoredCashTotal() =>
                Instance != null ? Instance._silasStoredCash : _runtimeSilasCash;

            public static void AddToStoredCashStatic(float amount)
            {
                if (Instance != null) Instance._silasStoredCash += amount;
                else _runtimeSilasCash += amount;
            }

            public static float TakeAllStoredCashStatic()
            {
                if (Instance != null)
                {
                    var v = Instance._silasStoredCash;
                    Instance._silasStoredCash = 0;
                    return v;
                }
                var r = _runtimeSilasCash;
                _runtimeSilasCash = 0;
                return r;
            }

            public static void AssignDealerRuntime(string dealerId)
            {
                if (_runtimeSilasDealers == null) _runtimeSilasDealers = new List<string>();
                if (!_runtimeSilasDealers.Contains(dealerId) && _runtimeSilasDealers.Count < MaxAssignedDealers)
                    _runtimeSilasDealers.Add(dealerId);
            }

            public static void UnassignDealerRuntime(string dealerId) => _runtimeSilasDealers?.Remove(dealerId);

            public static bool IsAssignedStatic(string dealerId) =>
                Instance != null
                    ? Instance._silasAssignedDealerIds.Contains(dealerId)
                    : (_runtimeSilasDealers?.Contains(dealerId) ?? false);
        }

        /// <summary>Dominic: assigned dealers and cash he is holding for the player.</summary>
        public static class DominicCartelSupervisor
        {
            public static int MaxAssignedDealers => SupervisorManager.MaxAssignedDealers;

            public static IReadOnlyList<string> GetAssignedDealerIds() =>
                Instance != null
                    ? (IReadOnlyList<string>)Instance._dominicAssignedDealerIds
                    : (IReadOnlyList<string>)(_runtimeDominicDealers ?? new List<string>());

            public static float GetStoredCashTotal() =>
                Instance != null ? Instance._dominicStoredCash : _runtimeDominicCash;

            public static void AddToStoredCashStatic(float amount)
            {
                if (Instance != null) Instance._dominicStoredCash += amount;
                else _runtimeDominicCash += amount;
            }

            public static float TakeAllStoredCashStatic()
            {
                if (Instance != null)
                {
                    var v = Instance._dominicStoredCash;
                    Instance._dominicStoredCash = 0;
                    return v;
                }
                var r = _runtimeDominicCash;
                _runtimeDominicCash = 0;
                return r;
            }

            public static void AssignDealerRuntime(string dealerId)
            {
                if (_runtimeDominicDealers == null) _runtimeDominicDealers = new List<string>();
                if (!_runtimeDominicDealers.Contains(dealerId) && _runtimeDominicDealers.Count < MaxAssignedDealers)
                    _runtimeDominicDealers.Add(dealerId);
            }

            public static void UnassignDealerRuntime(string dealerId) => _runtimeDominicDealers?.Remove(dealerId);

            public static bool IsAssignedStatic(string dealerId) =>
                Instance != null
                    ? Instance._dominicAssignedDealerIds.Contains(dealerId)
                    : (_runtimeDominicDealers?.Contains(dealerId) ?? false);
        }

        public void SilasAssignDealer(string dealerId)
        {
            if (!_silasAssignedDealerIds.Contains(dealerId) && _silasAssignedDealerIds.Count < SilasCartelSupervisor.MaxAssignedDealers)
                _silasAssignedDealerIds.Add(dealerId);
        }

        public void SilasUnassignDealer(string dealerId) => _silasAssignedDealerIds.Remove(dealerId);

        public void DominicAssignDealer(string dealerId)
        {
            if (!_dominicAssignedDealerIds.Contains(dealerId) && _dominicAssignedDealerIds.Count < DominicCartelSupervisor.MaxAssignedDealers)
                _dominicAssignedDealerIds.Add(dealerId);
        }

        public void DominicUnassignDealer(string dealerId) => _dominicAssignedDealerIds.Remove(dealerId);

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
            if (_runtimeSilasCash > 0) { _silasStoredCash += _runtimeSilasCash; _runtimeSilasCash = 0; }
            if (_runtimeSilasDealers?.Count > 0)
            {
                foreach (var id in _runtimeSilasDealers)
                {
                    if (!_silasAssignedDealerIds.Contains(id) && _silasAssignedDealerIds.Count < SilasCartelSupervisor.MaxAssignedDealers)
                        _silasAssignedDealerIds.Add(id);
                }
                _runtimeSilasDealers.Clear();
            }
            if (_runtimeDominicCash > 0) { _dominicStoredCash += _runtimeDominicCash; _runtimeDominicCash = 0; }
            if (_runtimeDominicDealers?.Count > 0)
            {
                foreach (var id in _runtimeDominicDealers)
                {
                    if (!_dominicAssignedDealerIds.Contains(id) && _dominicAssignedDealerIds.Count < DominicCartelSupervisor.MaxAssignedDealers)
                        _dominicAssignedDealerIds.Add(id);
                }
                _runtimeDominicDealers.Clear();
            }
        }

        protected override void OnDestroyed()
        {
            Instance = null;
            base.OnDestroyed();
        }
    }
}
