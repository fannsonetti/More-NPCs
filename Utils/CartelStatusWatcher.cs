using MelonLoader;
using S1API.Cartel;
using UnityEngine;

namespace MoreNPCs.Utils
{
    public class CartelStatusWatcher
    {
        private float _nextCheckTime = 0f;
        private CartelStatus _lastStatus;

        public void Update()
        {
            if (Time.time < _nextCheckTime)
                return;

            _nextCheckTime = Time.time + 30f;

            var cartel = Cartel.Instance;
            if (cartel == null)
                return;

            var status = cartel.Status;
            var hours = cartel.HoursSinceStatusChange;
            string statusName = GetStatusName(status);

            if (status != _lastStatus)
            {
                _lastStatus = status;
                MelonLogger.Msg($"[Cartel] Status changed to: {statusName} (ID {(int)status}), Hours since change: {hours}");
            }
        }

        private string GetStatusName(CartelStatus status)
        {
            return status switch
            {
                CartelStatus.Unknown => "Unknown",
                CartelStatus.Truced => "Truced",
                CartelStatus.Hostile => "Hostile",
                CartelStatus.Defeated => "Defeated",
                _ => $"Unknown ({(int)status})"
            };
        }
    }
}

