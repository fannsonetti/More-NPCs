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
    /// Cory Lubbin - Westville styled kid. Connected to Bobby Cooley.
    /// </summary>
    public sealed class CoryLubbin : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var cornerStore = Building.Get<CornerStore>();
            var micksHouse = Building.Get<MicksHouse>();
            Vector3 playZone = new Vector3(-135.7772f, -3.0349f, 44.1562f);
            Vector3 dadsWork = new Vector3(-62.4378f, 1.065f, 46.811f);
            Vector3 busStop = new Vector3(-13.0495f, 1.065f, 95.5169f);
            Vector3 spawnPos = new Vector3(-130.4420f, -2.9550f, 48.9370f);

            builder.WithIdentity("cory_lubbin", "Cory", "Lubbin")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.08f;
                    av.Height = 0.80f;
                    av.Weight = 0.40f;
                    av.SkinColor = new Color(0.66f, 0.54f, 0.44f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.99f, 0.89f, 0.84f);
                    av.PupilDilation = 0.72f;
                    av.EyebrowScale = 0.98f;
                    av.EyebrowThickness = 0.88f;
                    av.EyebrowRestingHeight = -0.08f;
                    av.EyebrowRestingAngle = 1.20f;
                    av.LeftEye = (0.28f, 0.38f);
                    av.RightEye = (0.28f, 0.38f);
                    av.HairColor = new Color(0.56f, 0.44f, 0.27f);
                    av.HairPath = "Avatar/Hair/BuzzCut/BuzzCut";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/Freckles", new Color(0.5f, 0.35f, 0.25f));
                    av.WithBodyLayer("Avatar/Layers/Top/Buttonup", new Color(0.60f, 0.17f, 0.16f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.23f, 0.25f, 0.27f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.16f, 0.16f, 0.18f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/Oakleys/Oakleys", new Color(0.12f, 0.12f, 0.13f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(400f, 600f)
                        .WithOrdersPerWeek(2, 4)
                        .WithPreferredOrderDay(Day.Saturday)
                        .WithOrderTime(1500)
                        .WithStandards(CustomerStandard.VeryLow)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(0.00f, 1.00f)
                        .WithCallPoliceChance(0.09f)
                        .WithDependence(0.05f, 1.00f)
                        .WithAffinities(new[] { (DrugType.Marijuana, 0.38f), (DrugType.Methamphetamine, -0.14f), (DrugType.Shrooms, 0.18f), (DrugType.Cocaine, -0.25f) })
                        .WithPreferredProperties(Property.Energizing, Property.Refreshing, Property.Sneaky);
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
                    plan.Add(new WalkToSpec { Destination = playZone, StartTime = 0805, FaceDestinationDirection = true });
                    plan.StayInBuilding(cornerStore, 0910, 94);
                    plan.Add(new WalkToSpec { Destination = dadsWork, StartTime = 1045, FaceDestinationDirection = true });
                    plan.Add(new SitSpec { SeatSetPath = "Map/Hyland Point/Region_Northtown/Pawn shop/Interior/Double Sofa", StartTime = 1215, DurationMinutes = 119 });
                    plan.UseVendingMachine(1415);
                    plan.Add(new SitSpec { SeatSetPath = "Map/Hyland Point/Region_Westville/OutdoorBench", StartTime = 1540, DurationMinutes = 94 });
                    plan.Add(new WalkToSpec { Destination = playZone, StartTime = 1715, FaceDestinationDirection = true });
                    plan.StayInBuilding(micksHouse, 1900, 784);
                });
        }

        public CoryLubbin() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.31f;
                Region = Region.Westville;
                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"CoryLubbin OnCreated failed: {ex.Message}");
            }
        }
    }
}


