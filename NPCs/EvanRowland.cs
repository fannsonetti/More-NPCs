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
    /// Evan Rowland - Nerdy kid. Connected only to Bobby Cooley. Lives in Charles' House.
    /// </summary>
    public sealed class EvanRowland : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var charlesHouse = Building.Get<CharlesHouse>();
            Vector3 arcade = new Vector3(-22f, 1.065f, 45f);
            Vector3 busStop = new Vector3(-13.0495f, 1.065f, 95.5169f);
            // Charles' House interior (Westville) — not sidewalk; aligns with overnight StayInBuilding
            Vector3 spawnPos = new Vector3(-55.2f, 1.065f, 72.8f);

            builder.WithIdentity("evan_rowland", "Evan", "Rowland")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.1f;
                    av.Height = 0.8f;
                    av.Weight = 0.38f;
                    av.SkinColor = new Color(0.65f, 0.52f, 0.42f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.98f, 0.90f, 0.88f);
                    av.PupilDilation = 0.72f;
                    av.EyebrowScale = 0.95f;
                    av.EyebrowThickness = 0.85f;
                    av.EyebrowRestingHeight = -0.15f;
                    av.EyebrowRestingAngle = 1.4f;
                    av.LeftEye = (0.28f, 0.38f);
                    av.RightEye = (0.28f, 0.38f);
                    av.HairColor = new Color(0.18f, 0.14f, 0.10f);
                    av.HairPath = "Avatar/Hair/BowlCut/BowlCut";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Neutral", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/Tucked T-Shirt", new Color(0.35f, 0.42f, 0.55f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.28f, 0.28f, 0.30f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.20f, 0.20f, 0.22f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/SmallRoundGlasses/SmallRoundGlasses", new Color(0.15f, 0.15f, 0.15f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(50f, 250f)
                        .WithOrdersPerWeek(1, 2)
                        .WithPreferredOrderDay(Day.Friday)
                        .WithOrderTime(1530)
                        .WithStandards(CustomerStandard.VeryLow)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(0f, 1f)
                        .WithCallPoliceChance(0.09f)
                        .WithDependence(0.0f, 1f)
                        .WithAffinities(new[] { (DrugType.Marijuana, 0.1f), (DrugType.Methamphetamine, -0.4f), (DrugType.Shrooms, 0.2f), (DrugType.Cocaine, -0.5f) })
                        .WithPreferredProperties(Property.Calming, Property.Refreshing, Property.Focused);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(3.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("bobby_cooley");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    // Home until first outing (1 min before Arcade); covers 4 AM cold start
                    plan.StayInBuilding(charlesHouse, 000, 535);
                    plan.Add(new StayInBuildingSpec { BuildingName = "Arcade", StartTime = 0856, DurationMinutes = 119 });
                    plan.Add(new SitSpec { SeatSetPath = "Map/Hyland Point/Bus stops/Bus Stop (8)/OutdoorBench", StartTime = 1056, DurationMinutes = 129 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Arcade", StartTime = 1226, DurationMinutes = 119 });
                    plan.UseVendingMachine(1356);
                    // Home 17:26 → midnight (next day loop picks up 000 block above)
                    plan.StayInBuilding(charlesHouse, 1726, 394);
                });
        }

        public EvanRowland() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.28f;
                Region = Region.Westville;
                Schedule.Enable();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"EvanRowland OnCreated failed: {ex.Message}");
            }
        }
    }
}

