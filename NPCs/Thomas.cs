using MelonLoader;
using S1API.Economy;
using S1API.Entities;
using S1API.Entities.NPCs.Downtown;
using S1API.Entities.Schedule;
using S1API.GameTime;
using S1API.Map;
using S1API.Money;
using S1API.Products;
using S1API.Properties;
using S1API.Vehicles;
using UnityEngine;
using UnityEngine.UI;
// add this:
using ScheduleOne.Cartel;

namespace CustomNPCTest.NPCs
{
    public sealed class ThomasBenzie : NPC
    {
        protected override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            Vector3 redRoom = new Vector3(168.0324f, 15.215f, -51.5853f);
            Vector3 greenRoom = new Vector3(153.6791f, 15.215f, -52.9755f);
            Vector3 blueRoom = new Vector3(158.9682f, 15.215f, -62.4593f);
            Vector3 grayRoom = new Vector3(173.7f, 15.215f, -57.85f);
            Vector3 Window = new Vector3(158.964f, 11.465f, -62.2987f);
            Vector3 spawnPos = new Vector3(158.964f, 11.465f, -62.2987f);

            builder.WithSpawnPosition(spawnPos)
                   .EnsureCustomer()
                   .WithCustomerDefaults(cd =>
                   {
                       cd.WithSpending(minWeekly: 2000f, maxWeekly: 10000f)
                         .WithOrdersPerWeek(0, 1)
                         .WithPreferredOrderDay(Day.Friday)
                         .WithOrderTime(1500)
                         .WithStandards(CustomerStandard.VeryHigh)
                         .AllowDirectApproach(true)
                         .GuaranteeFirstSample(false)
                         .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                         .WithCallPoliceChance(0.05f)
                         .WithDependence(baseAddiction: 0.0f, dependenceMultiplier: 1f)
                         .WithAffinities(new[] { (DrugType.Marijuana, 1f), (DrugType.Methamphetamine, 1f), (DrugType.Cocaine, 1f) })
                         .WithPreferredProperties();
                   })
                   .WithRelationshipDefaults(r =>
                   {
                       r.WithDelta(2.5f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById();
                   })
                   .WithSchedule(plan =>
                   {
                       plan.EnsureDealSignal();
                       plan.Add(new WalkToSpec { Destination = redRoom, StartTime = 700, FaceDestinationDirection = true });
                       plan.Add(new WalkToSpec { Destination = greenRoom, StartTime = 900, FaceDestinationDirection = true });
                       plan.Add(new WalkToSpec { Destination = blueRoom, StartTime = 1100, FaceDestinationDirection = true });
                       plan.Add(new WalkToSpec { Destination = grayRoom, StartTime = 1300, FaceDestinationDirection = true });
                       plan.Add(new WalkToSpec { Destination = Window, StartTime = 1500, FaceDestinationDirection = true });
                       plan.Add(new WalkToSpec { Destination = redRoom, StartTime = 1700, FaceDestinationDirection = true });
                       plan.Add(new WalkToSpec { Destination = greenRoom, StartTime = 1900, FaceDestinationDirection = true });
                       plan.Add(new WalkToSpec { Destination = blueRoom, StartTime = 2100, FaceDestinationDirection = true });
                       plan.Add(new WalkToSpec { Destination = grayRoom, StartTime = 2300, FaceDestinationDirection = true });
                       plan.Add(new WalkToSpec { Destination = Window, StartTime = 100, FaceDestinationDirection = true });
                   });
        }

        public ThomasBenzie() : base(
            id: "thomas_benzie",
            firstName: "Thomas",
            lastName: "Benzie",
            icon: null)
        { }

        protected override void OnCreated()
        {
            try
            {
                var cartel = Cartel.Instance;
                int defeatedId = 3; // 0=Strangers, 1=Friendly, 2=Hostile, 3=Defeated

                // ❌ Skip spawn if Cartel IS Friendly
                if (cartel != null && (int)cartel.Status == defeatedId)
                {
                    MelonLogger.Msg($"[Thomas] Not spawning: Cartel is Friendly (status {defeatedId}).");
                    try { Schedule.Disable(); } catch { /* ignore */ }
                    if (this != null)
                        UnityEngine.Object.Destroy(this.gameObject);
                    return;
                }

                // continue normal spawn for all other statuses
                base.OnCreated();

                ApplyConsistentAppearance();
                Appearance.Build();

                Aggressiveness = 5f;
                Region = Region.Uptown;

                Schedule.Enable();
                Schedule.InitializeActions();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"ThomasBenzie OnCreated failed: {ex.Message}");
                MelonLogger.Error($"StackTrace: {ex.StackTrace}");
            }
        }


        private void ApplyConsistentAppearance()
        {
            Appearance.Set<S1API.Entities.Appearances.CustomizationFields.Gender>(0.0f);
        }
    }
}
