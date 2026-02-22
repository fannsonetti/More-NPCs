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
    public sealed class NoraKessler : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var shermanHouse = Building.Get<ShermanHouse>();
            var arcade = Building.Get<Arcade>();
            var sauerkrautSupreme = Building.Get<SauerkrautSupreme>();
            Vector3 westWarehouseWarf = new Vector3(-60.5784f, 1.065f, 80.3446f);
            Vector3 busStop = new Vector3(-13.0495f, 1.065f, 95.5169f);
            builder.WithIdentity("nora_kessler", "Nora", "Kessler")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 2.85f;
                    av.Height = 0.98f;
                    av.Weight = 0.32f;
                    av.SkinColor = new Color(0.615f, 0.498f, 0.392f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(1.0f, 0.8f, 0.8f);
                    av.PupilDilation = 0.75f;
                    av.EyebrowScale = 1.0f;
                    av.EyebrowThickness = 0.9f;
                    av.EyebrowRestingHeight = -0.219f;
                    av.EyebrowRestingAngle = 2.945f;
                    av.LeftEye = (0.30f, 0.39f);
                    av.RightEye = (0.30f, 0.39f);
                    av.HairColor = new Color(0.075f, 0.075f, 0.075f);
                    av.HairPath = "Avatar/Hair/Shoulderlength/ShoulderLength";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Agitated", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/Buttonup", new Color(0.943f, 0.576f, 0.316f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.236f, 0.236f, 0.236f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.717f, 0.717f, 0.717f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/CollarJacket/CollarJacket", new Color(0.151f, 0.151f, 0.151f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/Belt/Belt", new Color(0.151f, 0.151f, 0.151f));
                    av.WithAccessoryLayer("Avatar/Accessories/Neck/GoldChain/GoldChain", new Color(1.000f, 0.756f, 0.212f));
                })
                .WithSpawnPosition(busStop)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 500f, maxWeekly: 700f)
                        .WithOrdersPerWeek(2, 4)
                        .WithPreferredOrderDay(Day.Thursday)
                        .WithOrderTime(1615)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0f)
                        .WithDependence(baseAddiction: 0.25f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, -0.63f), (DrugType.Methamphetamine, 0.13f), (DrugType.Shrooms, 0.68f), (DrugType.Cocaine, 0.46f)
                        })
                        .WithPreferredProperties(Property.Smelly, Property.TropicThunder, Property.Gingeritis);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("trent_sherman","victor_hughes");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(arcade, 1227, 95);
                    plan.Add(new WalkToSpec { Destination = busStop, StartTime = 1403, FaceDestinationDirection = false });
                    plan.UseATM(1542);
                    plan.StayInBuilding(sauerkrautSupreme, 1626, 111);
                    plan.UseVendingMachine(1818);
                    plan.StayInBuilding(shermanHouse, 1928, 98);
                    plan.Add(new WalkToSpec { Destination = busStop, StartTime = 2057, FaceDestinationDirection = false });
                    plan.StayInBuilding(shermanHouse, 2226, 840);
                });
        }

        public NoraKessler() : base()
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


