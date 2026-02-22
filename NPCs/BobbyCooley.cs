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
    public sealed class BobbyCooley : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var kyleandaustin = Building.Get<KyleAndAustinsHouse>();
            Vector3 behindcounter = new Vector3(15.7046f, 1.215f, -1.6526f);
            Vector3 shelf1 = new Vector3(15.2149f, 1.215f, -8.1838f);
            Vector3 shelf2 = new Vector3(11.042f, 1.215f, -8.2586f);
            Vector3 shelf3 = new Vector3(13.2172f, 1.215f, -6.2293f);
            Vector3 pos1 = new Vector3(69.7895f, 1.065f, 15.4409f);
            Vector3 pos2 = new Vector3(65.9639f, 1.065f, 87.5656f);
            Vector3 pos3 = new Vector3(-30.741f, 1.065f, 72.7557f);
            Vector3 spawnPos = new Vector3(15.7046f, 1.215f, -1.6526f);
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
                    cd.WithSpending(minWeekly: 400f, maxWeekly: 600f)
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
                        .WithConnectionsById("meg_cooley");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new WalkToSpec { Destination = shelf1, StartTime = 657, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 340, 0) * Vector3.forward});
                    plan.Add(new WalkToSpec { Destination = behindcounter, StartTime = 727, FaceDestinationDirection = true , Forward = Quaternion.Euler(0, 160, 0) * Vector3.forward});
                    plan.Add(new WalkToSpec { Destination = shelf2, StartTime = 833, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = shelf3, StartTime = 856, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 155, 0) * Vector3.forward});
                    plan.Add(new WalkToSpec { Destination = behindcounter, StartTime = 1004, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 160, 0) * Vector3.forward});
                    plan.UseVendingMachine(1197);
                    plan.Add(new WalkToSpec { Destination = shelf1, StartTime = 1227, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 340, 0) * Vector3.forward});
                    plan.Add(new WalkToSpec { Destination = behindcounter, StartTime = 1326, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 160, 0) * Vector3.forward});
                    plan.Add(new WalkToSpec { Destination = pos1, StartTime = 1433, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = pos2, StartTime = 1604, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = pos3, StartTime = 1726, FaceDestinationDirection = true });
                    plan.StayInBuilding(kyleandaustin, 1826, 750); // 1 min before shelf1 at 657
                });
        }

        public BobbyCooley() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 5f;
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


