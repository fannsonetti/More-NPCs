using MelonLoader;
using S1API.Economy;
using S1API.Entities;
using S1API.Entities.Schedule;
using S1API.Entities.NPCs.Westville;
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
    public sealed class VictorHughes : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var shermanHouse = Building.Get<ShermanHouse>();
            var thePissHut = Building.Get<ThePissHut>();
            var cornerStore = Building.Get<CornerStore>();
            Vector3 northWaterfront = new Vector3(-63.2588f, -4.035f, 168.9073f);
            Vector3 westGasmart = new Vector3(-113.1828f, -2.835f, 61.2241f);
            builder.WithIdentity("victor_hughes", "Victor", "Hughes")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 1.03f;
                    av.Weight = 0.28f;
                    av.SkinColor = new Color(0.615f, 0.498f, 0.392f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(1.0f, 0.8f, 0.8f);
                    av.PupilDilation = 0.75f;
                    av.EyebrowScale = 1.17f;
                    av.EyebrowThickness = 1.44f;
                    av.EyebrowRestingHeight = -0.286f;
                    av.EyebrowRestingAngle = 2.945f;
                    av.LeftEye = (0.30f, 0.39f);
                    av.RightEye = (0.30f, 0.39f);
                    av.HairColor = new Color(0.075f, 0.075f, 0.075f);
                    av.HairPath = "Avatar/Hair/Spiky/Spiky";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SmugPout", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Stubble", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/Tucked T-Shirt", new Color(0.943f, 0.576f, 0.316f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.236f, 0.236f, 0.236f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.717f, 0.717f, 0.717f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/OpenVest/OpenVest", new Color(0.151f, 0.151f, 0.151f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/Belt/Belt", new Color(0.151f, 0.151f, 0.151f));
                    av.WithAccessoryLayer("Avatar/Accessories/Hands/Polex/Polex", new Color(0.717f, 0.717f, 0.717f));
                })
                .WithSpawnPosition(westGasmart)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 500f, maxWeekly: 700f)
                        .WithOrdersPerWeek(1, 4)
                        .WithPreferredOrderDay(Day.Saturday)
                        .WithOrderTime(1900)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0f)
                        .WithDependence(baseAddiction: 0.25f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.93f), (DrugType.Methamphetamine, -0.03f), (DrugType.Shrooms, -0.05f), (DrugType.Cocaine, -0.95f)
                        })
                        .WithPreferredProperties(Property.Lethal, Property.Euphoric, Property.ThoughtProvoking);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("jamal_bennett");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(cornerStore, 897, 149);
                    plan.Add(new WalkToSpec { Destination = northWaterfront, StartTime = 1127, FaceDestinationDirection = true });
                    plan.StayInBuilding(thePissHut, 1418, 124);
                    plan.UseVendingMachine(1543);
                    plan.StayInBuilding(cornerStore, 1626, 111);
                    plan.UseATM(1818);
                    plan.StayInBuilding(shermanHouse, 1927, 96);
                    plan.Add(new WalkToSpec { Destination = westGasmart, StartTime = 2104, FaceDestinationDirection = false });
                    plan.StayInBuilding(shermanHouse, 2233, 623);
                });
        }

        public VictorHughes() : base()
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


