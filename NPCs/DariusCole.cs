using MelonLoader;
using S1API.Entities;
using S1API.Entities.Schedule;
using S1API.Map;
using S1API.Map.ParkingLots;
using S1API.Money;
using S1API.Economy;
using S1API.Entities.NPCs.Docks;
using S1API.GameTime;
using S1API.Growing;
using S1API.Map.Buildings;
using S1API.Products;
using S1API.Properties;
using S1API.Vehicles;
using UnityEngine;
using System.Linq;
using S1API.Avatar;

namespace CustomNPCTest.NPCs
{
    /// <summary>
    /// An example S1API NPC that opts into a physical rig.
    /// Demonstrates movement and inventory usage.
    /// </summary>
    public sealed class DariusCole : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var northIndustrial = Building.Get<NorthIndustrialBuilding>();
            var hospital = Building.Get<HylandMedical>();
            var budsBar = Building.Get<BudsBar>();
            var unit2 = Building.Get<StorageUnit2>();
            Vector3 spawnPos = new Vector3(-71.2847f, -2.935f, 145.6754f);
            Vector3 gasMart = new Vector3(17.6433f, 1.215f, -3.36f);
            Vector3 barbershop = new Vector3(-66.189f, -3.025f, 124.795f);
            Vector3 sidewalkCorner = new Vector3(124.3275f, 1.065f, 37.519f);
            builder.WithIdentity("darius_cole", "Darius", "Cole")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 1.0f;
                    av.Weight = 0.9f;
                    av.SkinColor = new Color(0.282f, 0.239f, 0.203f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(1.0f, 0.8f, 0.8f);
                    av.PupilDilation = 0.70f;
                    av.EyebrowScale = 1.1f;
                    av.EyebrowThickness = 1.3f;
                    av.EyebrowRestingHeight = -0.5f;
                    av.EyebrowRestingAngle = 0.0f;
                    av.LeftEye = (0.5f, 0.5f);
                    av.RightEye = (0.5f, 0.5f);
                    av.HairColor = new Color(0.075f, 0.075f, 0.075f);
                    av.HairPath = "Avatar/Hair/Afro/Afro";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Smug", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/Facialhair_Goatee", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/OldPersonWrinklesv", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/RolledButtonUp", new Color(0.178f, 0.217f, 0.406f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.481f, 0.331f, 0.225f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(1.0f, 1.0f, 1.0f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/Vest/Vest", new Color(0.236f, 0.236f, 0.236f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/Belt/Belt", new Color(0.151f, 0.151f, 0.151f));
                    av.WithAccessoryLayer("Avatar/Accessories/Neck/GoldChain/GoldChain", new Color(1.000f, 0.756f, 0.212f));
                    av.WithAccessoryLayer("Avatar/Accessories/Hands/Polex/Polex", new Color(1.000f, 0.756f, 0.212f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 400f, maxWeekly: 800f)
                        .WithOrdersPerWeek(1, 4)
                        .WithPreferredOrderDay(Day.Saturday)
                        .WithOrderTime(930)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.20f)
                        .WithDependence(baseAddiction: 0.25f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, -0.37f), (DrugType.Methamphetamine, 0.64f), (DrugType.Cocaine, 0.12f)
                        })
                        .WithPreferredProperties(Property.Jennerising, Property.Refreshing, Property.Foggy);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("peter_file", "jessi_waters");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal()
                       .LocationDialogue(sidewalkCorner, 700, faceDestinationDir: true)
                       .UseVendingMachine(800)
                       .UseATM(845)
                       .StayInBuilding(hospital, 930, 150)
                       .LocationDialogue(gasMart, 1200)
                       .UseVendingMachine(1400)
                       .StayInBuilding(northIndustrial, 1425, 125)
                       .StayInBuilding(budsBar, 1630, 150)
                       .StayInBuilding(unit2, 1900, 90)
                       .WalkTo(barbershop, 2030, faceDestinationDir: false)
                       .StayInBuilding(unit2, 2130, 570);
                });
        }

        public DariusCole() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 5f;
                Region = Region.Docks;

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


