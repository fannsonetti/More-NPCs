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
            var bank = Building.Get<HylandBank>();
            var church = Building.Get<Church>();
            var upscaleApartments = Building.Get<UpscaleApartments>();
            Vector3 bleuball = new Vector3(70.766f, 1.3618f, -13.7107f);
            Vector3 ocean = new Vector3(74.5873f, 1.015f, 100.9171f);
            builder.WithIdentity("darius_cole", "Darius", "Cole")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 1.0f;
                    av.Weight = 0.9f;
                    av.SkinColor = new Color(0.282f, 0.239f, 0.203f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(1.0f, 0.9f, 0.9f);
                    av.PupilDilation = 0.70f;
                    av.EyebrowScale = 1.1f;
                    av.EyebrowThickness = 1.3f;
                    av.EyebrowRestingHeight = -0.5f;
                    av.EyebrowRestingAngle = 0.0f;
                    av.LeftEye = (0.5f, 0.5f);
                    av.RightEye = (0.5f, 0.5f);
                    av.HairColor = new Color(0.075f, 0.075f, 0.075f);
                    av.HairPath = "Avatar/Hair/Afro/Afro";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SmugPout", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Goatee", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/OldPersonWrinkles", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/RolledButtonUp", new Color(0.178f, 0.217f, 0.406f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.481f, 0.331f, 0.225f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(1.0f, 1.0f, 1.0f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/OpenVest/OpenVest", new Color(0.236f, 0.236f, 0.236f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/Belt/Belt", new Color(0.151f, 0.151f, 0.151f));
                    av.WithAccessoryLayer("Avatar/Accessories/Neck/GoldChain/GoldChain", new Color(1.000f, 0.756f, 0.212f));
                    av.WithAccessoryLayer("Avatar/Accessories/Hands/Polex/Polex", new Color(1.000f, 0.756f, 0.212f));
                })
                .WithSpawnPosition(ocean)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 1000f, maxWeekly: 2000f)
                        .WithOrdersPerWeek(1, 4)
                        .WithPreferredOrderDay(Day.Tuesday)
                        .WithOrderTime(1930)
                        .WithStandards(CustomerStandard.High)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.50f)
                        .WithDependence(baseAddiction: 0.25f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, -0.13f), (DrugType.Methamphetamine, 0.87f), (DrugType.Cocaine, -0.17f)
                        })
                        .WithPreferredProperties(Property.Laxative, Property.Shrinking, Property.Zombifying);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("leo_rivers", "michael_boog");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal()
                       .LocationDialogue(bleuball, 730, faceDestinationDir: true)
                       .UseVendingMachine(830)
                       .StayInBuilding(bank, 900, 330)
                       .UseVendingMachine(1430)
                       .StayInBuilding(bank, 1455, 125)
                       .StayInBuilding(church, 1700, 120)
                       .StayInBuilding(upscaleApartments, 1900, 45)
                       .UseATM(1945)
                       .WalkTo(bleuball, 2015, faceDestinationDir: false)
                       .StayInBuilding(upscaleApartments, 2200, 570);
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
                Region = Region.Uptown;

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


