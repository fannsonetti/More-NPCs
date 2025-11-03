using MelonLoader;
using UnityEngine;
using ScheduleOne.Cartel;

namespace MoreNPCs.Utils
{
    public class CartelStatusWatcher
    {
        private float _nextCheckTime = 0f;
        private ECartelStatus _lastStatus;

        // optional: cache Thomas’s GameObject
        private GameObject thomasObj;

        public void Update()
        {
            // Only check every 10 seconds
            if (Time.time < _nextCheckTime)
                return;

            _nextCheckTime = Time.time + 10f;

            if (Cartel.Instance == null)
                return;

            var status = Cartel.Instance.Status;
            var hours = Cartel.Instance.HoursSinceStatusChange;
            string statusName = GetStatusName((int)status);

            // find Thomas the first time
            if (thomasObj == null)
                thomasObj = GameObject.Find("Thomas");

            if (thomasObj != null && (int)status == 3) // 2 = Hostile
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

        private string GetStatusName(int statusId)
        {
            return statusId switch
            {
                0 => "Strangers",
                1 => "Friendly",
                2 => "Hostile",
                3 => "Defeated",
                _ => $"Unknown ({statusId})"
            };
        }
    }
}

