using System.Collections.Generic;
using S1API.Internal.Abstraction;
using S1API.Saveables;

namespace MoreNPCs.Utils
{
    /// <summary>Persists which NPCs have had the come-to-me unlock notification sent (so we only send once).</summary>
    public sealed class ComeToMeSave : Saveable
    {
        public static ComeToMeSave? Instance { get; private set; }

        [SaveableField("notify_sent_ids")]
        private List<string> _notifySentIds = new List<string>();

        private static readonly HashSet<string> _runtimeAdded = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);

        public static bool HasNotifySent(string npcId)
        {
            if (string.IsNullOrEmpty(npcId)) return false;
            if (Instance != null)
            {
                foreach (var id in Instance._notifySentIds)
                    if (string.Equals(id, npcId, System.StringComparison.OrdinalIgnoreCase)) return true;
                return false;
            }
            lock (_runtimeAdded) return _runtimeAdded.Contains(npcId);
        }

        public static void MarkNotifySent(string npcId)
        {
            if (string.IsNullOrEmpty(npcId)) return;
            if (Instance != null)
            {
                if (!Instance._notifySentIds.Contains(npcId)) Instance._notifySentIds.Add(npcId);
            }
            else
                lock (_runtimeAdded) _runtimeAdded.Add(npcId);
            Saveable.RequestGameSave();
        }

        public ComeToMeSave() => Instance = this;

        protected override void OnLoaded()
        {
            base.OnLoaded();
            lock (_runtimeAdded)
            {
                foreach (var id in _runtimeAdded)
                    if (!_notifySentIds.Contains(id)) _notifySentIds.Add(id);
                _runtimeAdded.Clear();
            }
        }

        protected override void OnDestroyed()
        {
            Instance = null;
            base.OnDestroyed();
        }
    }
}
