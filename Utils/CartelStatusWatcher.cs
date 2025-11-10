using MelonLoader;
using S1API.Cartel;
using UnityEngine;

namespace MoreNPCs.Utils
{
    public class CartelStatusWatcher
    {
        private float _nextCheckTime = 0f;
        private CartelStatus _lastStatus;

        // optional: cache Thomas's GameObject
        private GameObject thomasObj;

        public void Update()
        {
            // Only check every 10 seconds
            if (Time.time < _nextCheckTime)
                return;

            _nextCheckTime = Time.time + 10f;

            var cartel = Cartel.Instance;
            if (cartel == null)
                return;

            var status = cartel.Status;
            var hours = cartel.HoursSinceStatusChange;
            string statusName = GetStatusName(status);

            // find Thomas the first time
            if (thomasObj == null)
                thomasObj = GameObject.Find("Thomas");

            if (thomasObj != null && status == CartelStatus.Hostile)
            {
                if (thomasObj.activeSelf)
                {
                    thomasObj.SetActive(false);
                    MelonLogger.Msg("[CartelWatcher] Cartel is Hostile — Thomas disabled.");
                }
            }
            else if (thomasObj != null && !thomasObj.activeSelf)
            {
                thomasObj.SetActive(true);
                MelonLogger.Msg($"[CartelWatcher] Cartel is {statusName} — Thomas re-enabled.");
            }

            // logging change
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

