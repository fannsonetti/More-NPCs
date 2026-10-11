namespace MoreNPCs.Distributor
{
    /// <summary>Polls active distributor deals for locker completion and expiry.</summary>
    public static class DistributorRuntime
    {
        private static float _nextCheckTime;

        public static void Update()
        {
            if (!DistributorRunSave.IsDealActive() && !DistributorDropBox.IsActive)
                return;

            if (UnityEngine.Time.time < _nextCheckTime) return;
            _nextCheckTime = UnityEngine.Time.time + 1f;

            if (DistributorRunSave.SoldThisRun)
            {
                DistributorDropBox.Disable();
                DistributorRunSave.ClearRun();
                return;
            }

            if (DistributorRunSave.IsDealActive())
            {
                if (DistributorRunSave.IsDealExpired())
                {
                    DistributorRunScheduler.ExpireActiveDeal();
                    return;
                }

                DistributorDropBox.CheckCompletion();
            }
        }
    }
}
