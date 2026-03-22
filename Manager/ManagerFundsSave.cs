using S1API.Internal.Abstraction;
using S1API.Saveables;

namespace MoreNPCs.Manager
{
    /// <summary>Persists funds (player-given) and business earnings. Player can only take back what they've given from funds.</summary>
    public sealed class ManagerFundsSave : Saveable
    {
        public static ManagerFundsSave? Instance { get; private set; }
        private static float _runtimeStored;
        private static float _runtimeEarnings;

        [SaveableField("manager_funds")]
        private float _storedFunds;

        [SaveableField("manager_earnings")]
        private float _businessEarnings;

        public float StoredFunds { get => _storedFunds; set => _storedFunds = value; }
        public float BusinessEarnings { get => _businessEarnings; set => _businessEarnings = value; }

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
            var take = UnityEngine.Mathf.Min(available, maxAmount);
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

        public ManagerFundsSave() => Instance = this;
        protected override void OnCreated() { base.OnCreated(); Instance = this; }
        protected override void OnLoaded()
        {
            base.OnLoaded();
            if (_runtimeStored > 0) { _storedFunds += _runtimeStored; _runtimeStored = 0; }
            if (_runtimeEarnings > 0) { _businessEarnings += _runtimeEarnings; _runtimeEarnings = 0; }
        }
        protected override void OnDestroyed() { Instance = null; base.OnDestroyed(); }
    }
}
