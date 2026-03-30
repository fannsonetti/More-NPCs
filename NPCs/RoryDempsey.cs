using MelonLoader;
using S1API.Economy;
using S1API.Entities;
using S1API.Entities.Schedule;
using S1API.GameTime;
using S1API.Map;
using S1API.Map.Buildings;
using S1API.Products;
using S1API.Properties;
using UnityEngine;

namespace MoreNPCs.NPCs
{
    /// <summary>
    /// Rory Dempsey - Westville regular. Connections: George Greene. Lives in Room 3.
    /// </summary>
    public sealed class RoryDempsey : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            Vector3 spawnPos = new Vector3(-113.1828f, -2.835f, 61.2241f);
            Vector3 busStop = new Vector3(-13.0495f, 1.065f, 95.5169f);
            var cornerStore = Building.Get<CornerStore>();
            var arcade = Building.Get<Arcade>();

            builder.WithIdentity("rory_dempsey", "Rory", "Dempsey")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.18f;
                    av.Height = 1.01f;
                    av.Weight = 0.44f;
                    av.SkinColor = new Color(0.67f, 0.56f, 0.47f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.98f, 0.90f, 0.85f);
                    av.PupilDilation = 0.72f;
                    av.EyebrowScale = 1.08f;
                    av.EyebrowThickness = 0.94f;
                    av.EyebrowRestingHeight = -0.18f;
                    av.EyebrowRestingAngle = 1.9f;
                    av.LeftEye = (0.29f, 0.39f);
                    av.RightEye = (0.29f, 0.39f);
                    av.HairColor = new Color(0.20f, 0.15f, 0.11f);
                    av.HairPath = "Avatar/Hair/Franklin/Franklin";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Neutral", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Stubble", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/RolledButtonUp", new Color(0.37f, 0.42f, 0.35f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.24f, 0.25f, 0.27f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(0.16f, 0.16f, 0.16f));
                    av.WithAccessoryLayer("Avatar/Accessories/Hands/Polex/Polex", new Color(0.70f, 0.70f, 0.72f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(400f, 600f)
                        .WithOrdersPerWeek(1, 3)
                        .WithPreferredOrderDay(Day.Monday)
                        .WithOrderTime(1930)
                        .WithStandards(CustomerStandard.Low)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(0f, 1f)
                        .WithCallPoliceChance(0.15f)
                        .WithDependence(0.12f, 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.42f), (DrugType.Methamphetamine, 0.18f), (DrugType.Shrooms, -0.10f), (DrugType.Cocaine, -0.22f)
                        })
                        .WithPreferredProperties(Property.Calming, Property.Foggy, Property.ThoughtProvoking);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("george_greene");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new WalkToSpec { Destination = spawnPos, StartTime = 0755, FaceDestinationDirection = true });
                    plan.StayInBuilding(cornerStore, 0840, 104);
                    plan.StayInBuilding(arcade, 1025, 114);
                    plan.Add(new SitSpec { SeatSetPath = "Map/Hyland Point/Region_Westville/Corner Store/OutdoorBench", StartTime = 1210, DurationMinutes = 94 });
                    plan.UseVendingMachine(1345);
                    plan.UseATM(1545);
                    plan.Add(new StayInBuildingSpec { BuildingName = "Room 5", StartTime = 1935, DurationMinutes = 700 }); // until 7am, no 4am kickout
                });
        }

        public RoryDempsey() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.52f;
                Region = Region.Westville;
                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"RoryDempsey OnCreated failed: {ex.Message}");
            }
        }
    }
}


