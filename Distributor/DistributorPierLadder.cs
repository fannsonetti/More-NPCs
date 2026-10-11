using MelonLoader;
using UnityEngine;

namespace MoreNPCs.Distributor
{
    /// <summary>Clones the uptown medical-practice ladder for pier access near the distributor drop.</summary>
    internal static class DistributorPierLadder
    {
        private const string SourceLadderPath =
            "Map/Hyland Point/Region_Uptown/Medical Practice/Functional Ladder";

        private static GameObject? _ladder;

        public static void Ensure()
        {
            if (_ladder != null) return;

            var template = GameObject.Find(SourceLadderPath);
            if (template == null)
            {
                MelonLogger.Warning("DistributorPierLadder: template not found at '{0}'.", SourceLadderPath);
                return;
            }

            _ladder = UnityEngine.Object.Instantiate(template);
            _ladder.name = "DistributorPierLadder";
            _ladder.transform.position = DistributorConfig.PierLadderPosition;
            _ladder.transform.rotation = Quaternion.Euler(DistributorConfig.PierLadderEuler);
            _ladder.SetActive(true);

            MelonLogger.Msg("DistributorPierLadder: placed at {0}.", DistributorConfig.PierLadderPosition);
        }

        public static void Destroy()
        {
            if (_ladder != null)
                UnityEngine.Object.Destroy(_ladder);
            _ladder = null;
        }
    }
}
