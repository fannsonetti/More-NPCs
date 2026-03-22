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
    /// An example S1API NPC that opts into a physical rig.
    /// Demonstrates movement and inventory usage.
    /// </summary>
    public sealed class CodeeLubbin : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var cornerStore = Building.Get<CornerStore>();
            var mickHouse = Building.Get<MicksHouse>();
            Vector3 playZone = new Vector3(-135.7772f, -3.0349f, 44.1562f);
            Vector3 dadsWork = new Vector3(-62.4378f, 1.065f, 46.811f);
            Vector3 busStop = new Vector3(-13.0495f, 1.065f, 95.5169f);
            Vector3 spawnPos = new Vector3(-130.4420f, -2.9550f, 48.9370f);
            // var building = Buildings.GetAll().First();
            builder.WithIdentity("bobby_cooley", "Bobby", "Cooley")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 0.8f;
                    av.Weight = 0.4f;
                    av.SkinColor = new Color(0.713f, 0.592f, 0.486f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(1.0f, 0.8f, 0.8f);
                    av.PupilDilation = 1.0f;
                    av.EyebrowScale = 0.95f;
                    av.EyebrowThickness = 0.98f;
                    av.EyebrowRestingHeight = 0.48f;
                    av.EyebrowRestingAngle = -5.64f;
                    av.LeftEye = (0.56f, 0.44f);
                    av.RightEye = (0.56f, 0.44f);
                    av.HairColor = new Color(0.716f, 0.527f, 0.226f);
                    av.HairPath = "Avatar/Hair/Spiky/Spiky";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/Freckles", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/T-Shirt", new Color(0.317f, 0.503f, 0.243f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.207f, 0.236f, 0.378f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.236f, 0.236f, 0.236f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 100f, maxWeekly: 200f)
                        .WithOrdersPerWeek(2, 4)
                        .WithPreferredOrderDay(Day.Wednesday)
                        .WithOrderTime(1330)
                        .WithStandards(CustomerStandard.VeryLow)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.05f)
                        .WithDependence(baseAddiction: 0.0f, dependenceMultiplier: 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 1f), (DrugType.Methamphetamine, 1f), (DrugType.Shrooms, 1f), (DrugType.Cocaine, 1f)
                        })
                        .WithPreferredProperties();
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(3.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("bobby_cooley", "evan_rowland");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new StayInBuildingSpec { BuildingName = "Mick's House", StartTime = 0, DurationMinutes = 449 });
                    plan.Add(new WalkToSpec { Destination = playZone, StartTime = 750, FaceDestinationDirection = true });
                    plan.StayInBuilding(cornerStore, 835, 114);
                    plan.Add(new WalkToSpec { Destination = dadsWork, StartTime = 1035, FaceDestinationDirection = true });
                    plan.UseVendingMachine(1230);
                    plan.Add(new WalkToSpec { Destination = busStop, StartTime = 1330, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = playZone, StartTime = 1515, FaceDestinationDirection = true });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Mick's House", StartTime = 1830, DurationMinutes = 779 });
                });
        }

        public CodeeLubbin() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 0.32f; // kid, friendly
                Region = Region.Westville;

                // Customer.RequestProduct();

                Schedule.Enable();

            }
            catch (Exception ex)
            {
                MelonLogger.Error($"ExamplePhysicalNPC OnCreated failed: {ex.Message}");
                MelonLogger.Error($"StackTrace: {ex.StackTrace}");
            }
        }
    }
}


