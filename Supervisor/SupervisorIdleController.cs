using S1API.Entities;
using MoreNPCs.Utils;
using UnityEngine;

namespace MoreNPCs.Supervisor
{
    /// <summary>Supervisor home position and go-home helper.</summary>
    public static class SupervisorIdleController
    {
        public static readonly Vector3 HomePosition = SupervisorConfig.DefaultSpawnPosition;
        private static float ArriveThreshold =>
            !MoreNPCsPreferences.Registered ? 2f : MoreNPCsPreferences.SupervisorIdle_ArriveThreshold.Value;

        public static void GoHome(NPC npc)
        {
            if (npc?.Movement == null) return;
            var home = NPCIdleLocations.GetSupervisorIdlePosition(npc.ID, HomePosition);
            var threshold = ArriveThreshold;
            if (Vector3.Distance(npc.Movement.FootPosition, home) < threshold) return;
            npc.Movement.SetDestination(home);
        }

        public static bool IsAtHome(NPC npc)
        {
            if (npc?.Movement == null) return false;
            var threshold = ArriveThreshold;
            return Vector3.Distance(npc.Movement.FootPosition, NPCIdleLocations.GetSupervisorIdlePosition(npc.ID, HomePosition)) < threshold;
        }
    }
}
