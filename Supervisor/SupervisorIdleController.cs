using S1API.Entities;
using MoreNPCs.Utils;
using UnityEngine;

namespace MoreNPCs.Supervisor
{
    /// <summary>Supervisor home position and go-home helper.</summary>
    public static class SupervisorIdleController
    {
        public static readonly Vector3 HomePosition = SupervisorConfig.DefaultSpawnPosition;
        private const float ArriveThreshold = 2f;

        public static void GoHome(NPC npc)
        {
            if (npc?.Movement == null) return;
            var home = NPCIdleLocations.GetSupervisorIdlePosition(npc.ID, HomePosition);
            if (Vector3.Distance(npc.Movement.FootPosition, home) < ArriveThreshold) return;
            npc.Movement.SetDestination(home);
        }

        public static bool IsAtHome(NPC npc) =>
            npc?.Movement != null && Vector3.Distance(npc.Movement.FootPosition, NPCIdleLocations.GetSupervisorIdlePosition(npc.ID, HomePosition)) < ArriveThreshold;
    }
}
