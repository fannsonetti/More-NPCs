using System.Collections.Generic;
using S1API.Internal.Abstraction;
using S1API.Saveables;

namespace MoreNPCs.Persistence
{
    /// <summary>Supervisor dealer networks and stored cash (Silas + Dominic).</summary>
    public sealed class SupervisorsSave : Saveable
    {
        public static SupervisorsSave? Instance { get; private set; }

        private static float _runtimeSilasCash;
        private static List<string> _runtimeSilasDealers = new List<string>();
        private static float _runtimeDominicCash;
        private static List<string> _runtimeDominicDealers = new List<string>();

        [SaveableField("silas_dealers")]
        internal List<string> SilasDealerIds = new List<string>();

        [SaveableField("silas_cash")]
        internal float SilasStoredCash;

        [SaveableField("dominic_dealers")]
        internal List<string> DominicDealerIds = new List<string>();

        [SaveableField("dominic_cash")]
        internal float DominicStoredCash;

        public SupervisorsSave() => Instance = this;

        protected override void OnCreated()
        {
            base.OnCreated();
            Instance = this;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            MigrateFromLegacyModSave();

            if (_runtimeSilasCash > 0) { SilasStoredCash += _runtimeSilasCash; _runtimeSilasCash = 0; }
            if (_runtimeSilasDealers?.Count > 0)
            {
                foreach (var id in _runtimeSilasDealers)
                {
                    if (!SilasDealerIds.Contains(id) && SilasDealerIds.Count < Supervisor.SupervisorManager.MaxAssignedDealers)
                        SilasDealerIds.Add(id);
                }
                _runtimeSilasDealers.Clear();
            }

            if (_runtimeDominicCash > 0) { DominicStoredCash += _runtimeDominicCash; _runtimeDominicCash = 0; }
            if (_runtimeDominicDealers?.Count > 0)
            {
                foreach (var id in _runtimeDominicDealers)
                {
                    if (!DominicDealerIds.Contains(id) && DominicDealerIds.Count < Supervisor.SupervisorManager.MaxAssignedDealers)
                        DominicDealerIds.Add(id);
                }
                _runtimeDominicDealers.Clear();
            }
        }

        protected override void OnDestroyed()
        {
            Instance = null;
            base.OnDestroyed();
        }

        internal static float RuntimeSilasCash { get => _runtimeSilasCash; set => _runtimeSilasCash = value; }
        internal static float RuntimeDominicCash { get => _runtimeDominicCash; set => _runtimeDominicCash = value; }

        internal static List<string> GetOrCreateRuntimeSilasDealers()
        {
            if (_runtimeSilasDealers == null) _runtimeSilasDealers = new List<string>();
            return _runtimeSilasDealers;
        }

        internal static List<string> GetOrCreateRuntimeDominicDealers()
        {
            if (_runtimeDominicDealers == null) _runtimeDominicDealers = new List<string>();
            return _runtimeDominicDealers;
        }

        internal static IReadOnlyList<string> RuntimeSilasDealers => _runtimeSilasDealers;
        internal static IReadOnlyList<string> RuntimeDominicDealers => _runtimeDominicDealers;

        private void MigrateFromLegacyModSave()
        {
            var legacy = MoreNPCsModSave.Instance;
            if (legacy == null) return;
            if (SilasDealerIds.Count > 0 || DominicDealerIds.Count > 0) return;

            if (legacy._silasAssignedDealerIds?.Count > 0)
                SilasDealerIds = new List<string>(legacy._silasAssignedDealerIds);
            SilasStoredCash = legacy._silasStoredCash;

            if (legacy._dominicAssignedDealerIds?.Count > 0)
                DominicDealerIds = new List<string>(legacy._dominicAssignedDealerIds);
            DominicStoredCash = legacy._dominicStoredCash;
        }
    }
}
