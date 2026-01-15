using MelonLoader;
using S1API.Entities;
using S1API.Entities.Schedule;
using S1API.Map;
using S1API.Map.ParkingLots;
using S1API.Money;
using S1API.Economy;
using S1API.Entities.NPCs.Northtown;
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
    public sealed class MoeLester : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var petersRoom = Building.Get<PetersRoom>();
            var northApartments = Building.Get<NorthApartments>();
            var budsBar = Building.Get<BudsBar>();
            Vector3 spawnPos = new Vector3(-71.2847f, -2.935f, 145.6754f);
            Vector3 posA = new Vector3(-76.0633f, -1.535f, 44.6816f);
            Vector3 posB = new Vector3(-49.5478f, -4.035f, 168.5777f);
            builder.WithIdentity("moe_lester", "Moe", "Lester")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 1.0f;
                    av.Weight = 0.9f;
                    av.SkinColor = new Color(0.784f, 0.654f, 0.545f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(1.0f, 0.8f, 0.8f);
                    av.PupilDilation = 0.75f;
                    av.EyebrowScale = 1.0f;
                    av.EyebrowThickness = 1.31f;
                    av.EyebrowRestingHeight = -0.432f;
                    av.EyebrowRestingAngle = 0.0f;
                    av.LeftEye = (0.38f, 0.42f);
                    av.RightEye = (0.38f, 0.42f);
                    av.HairColor = new Color(0.509f, 0.375f, 0.161f);
                    av.HairPath = "Avatar/Hair/Receding/Receding";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SmugPout", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/Facialhair_Stubble", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/OldPersonWrinkles", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/FlannelButtonUp", new Color(0.803f, 0.947f, 0.657f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.178f, 0.217f, 0.406f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(1.0f, 1.0f, 1.0f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/Belt/Belt", new Color(0.481f, 0.331f, 0.225f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/RectangleFrameGlasses/RectangleFrameGlasses", new Color(0.151f, 0.151f, 0.151f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 200f, maxWeekly: 800f)
                        .WithOrdersPerWeek(3, 5)
                        .WithPreferredOrderDay(Day.Wednesday)
                        .WithOrderTime(1400)
                        .WithStandards(CustomerStandard.Low)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.25f)
                        .WithDependence(baseAddiction: 0.25f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.52f), (DrugType.Methamphetamine, -0.23f), (DrugType.Cocaine, 0.31f)
                        })
                        .WithPreferredProperties(Property.Jennerising, Property.Refreshing, Property.Glowie);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("jessi_waters","peter_file");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal()
                       .UseVendingMachine(900)
                       .WalkTo(posA, 925, faceDestinationDir: true)
                       .StayInBuilding(petersRoom, 1100, 120)
                       .LocationDialogue(posA, 1300)
                       .UseVendingMachine(1400)
                       .StayInBuilding(petersRoom, 1425, 125)
                       .UseATM(1630)
                       .StayInBuilding(budsBar, 1730, 150)
                       .UseVendingMachine(2000)
                       .WalkTo(posB, 2100, faceDestinationDir: false)
                       .StayInBuilding(northApartments, 2230, 450);
                });
        }

        public MoeLester() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 5f;
                Region = Region.Northtown;

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


