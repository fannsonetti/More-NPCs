using MelonLoader;
using S1API.Entities;
using S1API.Entities.Schedule;
using S1API.Map;
using S1API.Money;
using S1API.Economy;
using S1API.Entities.NPCs.Downtown;
using S1API.GameTime;
using S1API.Products;
using S1API.Properties;
using UnityEngine;

namespace CustomNPCTest.NPCs
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
            Vector3 behindcounter = new Vector3(15.7046f, 1.215f, -1.6526f);
            Vector3 shelf1 = new Vector3(15.2149f, 1.215f, -8.1838f);
            Vector3 shelf2 = new Vector3(11.042f, 1.215f, -8.2586f);
            Vector3 shelf3 = new Vector3(13.2172f, 1.215f, -6.2293f);
            Vector3 pos1 = new Vector3(69.7895f, 1.065f, 15.4409f);
            Vector3 pos2 = new Vector3(65.9639f, 1.065f, 87.5656f);
            Vector3 pos3 = new Vector3(-30.741f, 1.065f, 72.7557f);
            Vector3 spawnPos = new Vector3(15.7046f, 1.215f, -1.6526f);
            // var building = Buildings.GetAll().First();
            builder.WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 50f, maxWeekly: 200f)
                        .WithOrdersPerWeek(5, 7)
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
                            (DrugType.Marijuana, 1f), (DrugType.Methamphetamine, 1f), (DrugType.Cocaine, 1f)
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
                    plan.Add(new WalkToSpec { Destination = behindcounter, StartTime = 600, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = shelf1, StartTime = 700, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = behindcounter, StartTime = 730, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = shelf2, StartTime = 830, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = shelf3, StartTime = 900, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = behindcounter, StartTime = 1000, FaceDestinationDirection = true });
                    plan.Add(new UseVendingMachineSpec { StartTime = 1200 });
                    plan.Add(new WalkToSpec { Destination = shelf1, StartTime = 1300, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = behindcounter, StartTime = 1330, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = pos1, StartTime = 1430, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = pos2, StartTime = 1600, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = pos3, StartTime = 1730, FaceDestinationDirection = true });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Kyle and Austin's House", StartTime = 1830, DurationMinutes = 690 });
                });
        }

        public BobbyCooley() : base(
            id: "bobby_cooley",
            firstName: "Bobby",
            lastName: "Cooley",
            icon: null)
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                ApplyConsistentAppearance();
                Appearance.Build();

                Aggressiveness = 5f;
                Region = Region.Westville;

                // Customer.RequestProduct();

                Schedule.Enable();
                Schedule.InitializeActions();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"ExamplePhysicalNPC OnCreated failed: {ex.Message}");
                MelonLogger.Error($"StackTrace: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Applies a consistent appearance. Tweak the values below to your liking.
        /// </summary>
        private void ApplyConsistentAppearance()
        {
            // Core biometrics
            Appearance
                .Set<S1API.Entities.Appearances.CustomizationFields.Gender>(0.0f) // 0..1
                .Set<S1API.Entities.Appearances.CustomizationFields.Height>(0.8f)
                .Set<S1API.Entities.Appearances.CustomizationFields.Weight>(0.4f)
                .Set<S1API.Entities.Appearances.CustomizationFields.SkinColor>(new Color(0.713f, 0.592f, 0.486f))
                .Set<S1API.Entities.Appearances.CustomizationFields.EyeBallTint>(new Color(1.0f, 0.8f, 0.8f))
                .Set<S1API.Entities.Appearances.CustomizationFields.PupilDilation>(1.0f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyebrowScale>(0.95f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyebrowThickness>(0.98f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyebrowRestingHeight>(0.48f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyebrowRestingAngle>(-5.64f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyeLidRestingStateLeft>((0.56f, 0.44f))
                .Set<S1API.Entities.Appearances.CustomizationFields.EyeLidRestingStateRight>((0.56f, 0.44f))
                .Set<S1API.Entities.Appearances.CustomizationFields.HairColor>(new Color(0.716f, 0.527f, 0.226f))
                .Set<S1API.Entities.Appearances.CustomizationFields.HairStyle>("Avatar/Hair/Spiky/Spiky")
                .WithFaceLayer<S1API.Entities.Appearances.FaceLayerFields.Face>("Avatar/Layers/Face/Face_SlightSmile", Color.black)
                .WithFaceLayer<S1API.Entities.Appearances.FaceLayerFields.Eyes>("Avatar/Layers/Face/Freckles", new Color32(164, 136, 111, 10))
                .WithBodyLayer<S1API.Entities.Appearances.BodyLayerFields.Shirts>("Avatar/Layers/Top/T-Shirt", new Color(0.298f, 0.547f, 0.198f))
                .WithBodyLayer<S1API.Entities.Appearances.BodyLayerFields.Pants>("Avatar/Layers/Bottom/Jeans", new Color(0.177f, 0.216f, 0.405f))
                .WithAccessoryLayer<S1API.Entities.Appearances.AccessoryFields.Feet>("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.23529411852359773f, 0.23529411852359773f, 0.23529411852359773f));
        }
    }
}


