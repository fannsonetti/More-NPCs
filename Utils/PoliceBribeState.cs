using MoreNPCs.Save;
using S1API.Internal.Abstraction;
using S1API.Saveables;

namespace MoreNPCs.Saveables
{
    public class PoliceBribeState : Saveable
    {
        public static PoliceBribeState Instance { get; private set; } = new PoliceBribeState();

        [SaveableField("BribeState")]
        private PoliceBribeData _data = new PoliceBribeData();

        private PoliceBribeState()
        {
            Instance = this;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            Instance = this;
            
            // Ensure data is initialized if it was null
            if (_data == null)
            {
                _data = new PoliceBribeData();
            }
        }

        protected override void OnCreated()
        {
            base.OnCreated();
            Instance = this;
        }

        public bool IsBribed(string officerId)
        {
            if (string.IsNullOrWhiteSpace(officerId)) return false;
            if (_data == null) return false;
            return _data.Contains(officerId);
        }

        public void MarkBribed(string officerId)
        {
            if (string.IsNullOrWhiteSpace(officerId)) return;
            if (_data == null)
            {
                _data = new PoliceBribeData();
            }
            
            if (!_data.Contains(officerId))
            {
                _data.SetBribed(officerId);
                // Request save after marking bribed
                Saveable.RequestGameSave();
            }
        }
    }
}

