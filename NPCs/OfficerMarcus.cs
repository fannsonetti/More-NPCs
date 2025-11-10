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
    public sealed class OfficerMarcus : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var policeStation = Building.Get<PoliceStation>();
            MelonLogger.Msg("Configuring prefab for NPC 1");
            Vector3 posA = new Vector3(27.4168f, 1.065f, -14.4305f);
            Vector3 posB = new Vector3(-22.5378f, 1.065f, -43.1838f);
            Vector3 posC = new Vector3(-150.581f, -2.935f, 118.1681f);
            Vector3 posD = new Vector3(-139.3423f, -4.335f, 18.8808f);
            Vector3 posE = new Vector3(-121.0444f, -2.935f, 78.6722f);
            Vector3 posF = new Vector3(-22.7021f, 1.065f, 46.8433f);
            Vector3 spawnPos = new Vector3(16.0717f, 1.065f, 38.0883f);
            builder.WithIdentity("officer_marcus", "Officer", "Marcus")
                .WithAppearanceDefaults(av =>
                {
                    var skinColor = new Color(0.713f, 0.592f, 0.486f);
                    av.Gender = 0.0f;
                    av.Height = 1.0f;
                    av.Weight = 0.4f;
                    av.SkinColor = skinColor;
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = Color.white;
                    av.PupilDilation = 1.0f;
                    av.EyebrowScale = 1.1f;
                    av.EyebrowThickness = 1.0f;
                    av.EyebrowRestingHeight = -0.432f;
                    av.EyebrowRestingAngle = -2.451f;
                    av.LeftEye = (0.219f, 0.5f);
                    av.RightEye = (0.219f, 0.5f);
                    av.HairColor = new Color(0.31f, 0.2f, 0.12f);
                    av.HairPath = "Avatar/Hair/Buzzcut/Buzzcut";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_NeutralPout", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/OldPersonWrinkles", new Color32(0, 0, 0, 0));
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Goatee", new Color(0.31f, 0.2f, 0.12f));
                    av.WithBodyLayer("Avatar/Layers/Top/RolledButtonUp", new Color(0.178f, 0.217f, 0.406f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.235f, 0.235f, 0.235f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(0.151f, 0.151f, 0.151f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/PoliceBelt/PoliceBelt", Color.white);
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/BulletproofVest/BulletproofVest_Police", Color.white);
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 400f, maxWeekly: 1000f)
                        .WithOrdersPerWeek(1, 4)
                        .WithPreferredOrderDay(Day.Sunday)
                        .WithOrderTime(900)
                        .WithStandards(CustomerStandard.VeryLow)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(true)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.15f)
                        .WithDependence(baseAddiction: 0.1f, dependenceMultiplier: 1.1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.45f), (DrugType.Cocaine, -0.2f)
                        })
                        // .WithPreferredPropertiesById("Munchies", "Energizing", "Cyclopean");
                        .WithPreferredProperties(Property.Munchies, Property.Energizing, Property.Cyclopean);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(1.5f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        // .WithConnectionsById("kyle_cooley", "ludwig_meyer", "austin_steiner")
                        .WithConnections(Get<KyleCooley>(), Get<LudwigMeyer>(), Get<AustinSteiner>());
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal()
                        .WalkTo(posA, 700, faceDestinationDir: true)
                        .WalkTo(posB, 900, faceDestinationDir: true)
                        .WalkTo(posC, 1100, faceDestinationDir: true)
                        .WalkTo(posD, 1300, faceDestinationDir: true)
                        .WalkTo(posE, 1500, faceDestinationDir: true)
                        .WalkTo(posF, 1700, faceDestinationDir: true)
                        .WalkTo(posC, 1900, faceDestinationDir: true)
                        .WalkTo(posE, 2100, faceDestinationDir: true)
                        .WalkTo(posB, 2300, faceDestinationDir: true)
                        .StayInBuilding(policeStation, 2359, 420);
                })
                .WithInventoryDefaults(inv =>
                {
                    // Startup items that will always be in inventory when spawned
                    inv.WithStartupItems("m1911", "m1911mag")
                        // Random cash between $50 and $500
                        .WithRandomCash(min: 50, max: 500)
                        // Preserve inventory across sleep cycles
                        .WithClearInventoryEachNight(false);
                });
        }

        public OfficerMarcus() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 3f;
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
