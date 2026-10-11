using MoreNPCs.NPCs;
using S1API.Entities;

namespace MoreNPCs.Distributor
{
    public static class DistributorRegistry
    {
        public static GrantEllison? Grant { get; set; }
        public static NPC? GrantNpc => Grant;
    }
}
