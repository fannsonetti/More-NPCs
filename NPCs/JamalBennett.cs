using MelonLoader;
using S1API.Entities;
using S1API.Entities.Schedule;
using S1API.Map;
using S1API.Map.ParkingLots;
using S1API.Money;
using S1API.Economy;
using S1API.Entities.NPCs.Westville;
using S1API.GameTime;
using S1API.Growing;
using S1API.Map.Buildings;
using S1API.Products;
using S1API.Properties;
using S1API.Vehicles;
using UnityEngine;
using System.Linq;

namespace CustomNPCTest.NPCs
{
    /// <summary>
    /// An example S1API NPC that opts into a physical rig.
    /// Demonstrates movement and inventory usage.
    /// </summary>
    public sealed class JamalBennett : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var shermanHouse = Building.Get<ShermanHouse>();
            var janesVan = Building.Get<JanesCaravan>();
            var thePissHut = Building.Get<ThePissHut>();
            var cornerStore = Building.Get<CornerStore>();
            Vector3 spawnPos = new Vector3(-71.2847f, -2.935f, 145.6754f);
            Vector3 belowOverpass = new Vector3(-1.6567f, 0.9804f, -134.9677f);
            Vector3 sunset = new Vector3(-182.3843f, -4.175f, 82.0797f);
            builder.WithIdentity("jamal_bennett", "Jamal", "Bennett")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 1.0f;
                    av.Weight = 0.48f;
                    av.SkinColor = new Color(0.4f, 0.333f, 0.274f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(1.0f, 0.8f, 0.8f);
                    av.PupilDilation = 0.75f;
                    av.EyebrowScale = 1.11f;
                    av.EyebrowThickness = 1.88f;
                    av.EyebrowRestingHeight = -0.322f;
                    av.EyebrowRestingAngle = 3.741f;
                    av.LeftEye = (0.27f, 0.39f);
                    av.RightEye = (0.27f, 0.39f);
                    av.HairColor = new Color(0.075f, 0.075f, 0.075f);
                    av.HairPath = "Avatar/Hair/CloseBuzzCut/CloseBuzzCut";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Neutral", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/RolledButtonUp", new Color(0.236f, 0.236f, 0.236f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.236f, 0.236f, 0.236f));
                    av.WithBodyLayer("Avatar/Layers/Accessories/FingerlessGloves", new Color(0.943f, 0.576f, 0.316f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.717f, 0.717f, 0.717f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/Belt/Belt", new Color(0.151f, 0.151f, 0.151f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/OpenVest/OpenVest", new Color(0.943f, 0.576f, 0.316f));
                    av.WithAccessoryLayer("Avatar/Accessories/Neck/GoldChain/GoldChain", new Color(0.717f, 0.717f, 0.717f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 200f, maxWeekly: 500f)
                        .WithOrdersPerWeek(3, 6)
                        .WithPreferredOrderDay(Day.Sunday)
                        .WithOrderTime(800)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0f)
                        .WithDependence(baseAddiction: 0.25f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.01f), (DrugType.Methamphetamine, 0.73f), (DrugType.Cocaine, -0.31f)
                        })
                        .WithPreferredProperties(Property.Cyclopean, Property.Balding, Property.Disorienting);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("trent_sherman");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal()
                       .StayInBuilding(janesVan, 900, 120)
                       .WalkTo(belowOverpass, 1100, faceDestinationDir: true)
                       .StayInBuilding(thePissHut, 1400, 90)
                       .UseATM(1530)
                       .StayInBuilding(shermanHouse, 1615, 105)
                       .UseVendingMachine(1800)
                       .StayInBuilding(cornerStore, 1930, 90)
                       .WalkTo(sunset, 2100, faceDestinationDir: false)
                       .StayInBuilding(shermanHouse, 2230, 450);
                });
        }

        public JamalBennett() : base()
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


