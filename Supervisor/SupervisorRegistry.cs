using MoreNPCs.NPCs;
using S1API.Entities;

namespace MoreNPCs.Supervisor
{
    /// <summary>Tracks supervisor NPCs.</summary>
    public static class SupervisorRegistry
    {
        public static SilasMercer? PrimarySupervisor { get; set; }
        public static NPC? CurrentSupervisorWithMenu { get; set; }
        public static string? CurrentSupervisorId { get; set; }
        public static NPC? LastTradedSupervisor { get; set; }
        public static string? LastTradedSupervisorId { get; set; }
    }
}
