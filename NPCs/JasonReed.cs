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
    public sealed class JasonReed : NPC
    {
        protected override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            Vector3 tacoticklers = new Vector3(-28.9266f, 1.065f, 74.6178f);
            Vector3 tacoticklersStub = new Vector3(-28.9094f, 1.065f, 74.4508f);
            Vector3 outside = new Vector3(-36.3346f, 1.065f, 75.6414f);
            Vector3 outsideStub = new Vector3(-36.7476f, 1.065f, 75.7252f);
            Vector3 spawnPos = new Vector3(-28.9266f, 1.065f, 74.6178f);
            // var building = Buildings.GetAll().First();
            builder.WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 400f, maxWeekly: 900f)
                        .WithOrdersPerWeek(5, 7)
                        .WithPreferredOrderDay(Day.Monday)
                        .WithOrderTime(2020)
                        .WithStandards(CustomerStandard.VeryLow)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.33f)
                        .WithDependence(baseAddiction: 0.1f, dependenceMultiplier: 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.86f), (DrugType.Methamphetamine, 0.25f), (DrugType.Cocaine, -0.67f)
                        })
                        .WithPreferredProperties(Property.LongFaced, Property.CalorieDense, Property.Shrinking);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("kyle_cooley", "austin_steiner");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new WalkToSpec { Destination = tacoticklers, StartTime = 600, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = tacoticklersStub, StartTime = 730, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = outside, StartTime = 1800, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = outsideStub, StartTime = 1830, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = tacoticklers, StartTime = 2000, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = tacoticklersStub, StartTime = 2030, FaceDestinationDirection = true });
                    plan.Add(new UseVendingMachineSpec { StartTime = 2200 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "North apartments", StartTime = 2230, DurationMinutes = 450 });
                });
        }

        public JasonReed() : base(
            id: "jason_reed",
            firstName: "Jason",
            lastName: "Reed",
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
                Region = Region.Northtown;

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
                .Set<S1API.Entities.Appearances.CustomizationFields.Height>(0.95f)
                .Set<S1API.Entities.Appearances.CustomizationFields.Weight>(1.0f)
                .Set<S1API.Entities.Appearances.CustomizationFields.SkinColor>(new Color(0.713f, 0.592f, 0.486f))
                .Set<S1API.Entities.Appearances.CustomizationFields.EyeBallTint>(new Color(1.0f, 0.8f, 0.8f))
                .Set<S1API.Entities.Appearances.CustomizationFields.PupilDilation>(0.75f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyebrowScale>(1.39f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyebrowThickness>(0.5f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyebrowRestingHeight>(-0.48f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyebrowRestingAngle>(-4.64f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyeLidRestingStateLeft>((0.18f, 0.24f))
                .Set<S1API.Entities.Appearances.CustomizationFields.EyeLidRestingStateRight>((0.18f, 0.24f))
                .Set<S1API.Entities.Appearances.CustomizationFields.HairColor>(new Color(0.55f, 0.41f, 0.31f))
                .Set<S1API.Entities.Appearances.CustomizationFields.HairStyle>("Avatar/Hair/SidePartBob/SidePartBob")
                .WithFaceLayer<S1API.Entities.Appearances.FaceLayerFields.Face>("Avatar/Layers/Face/Face_NeutralPout", Color.black)
                .WithFaceLayer<S1API.Entities.Appearances.FaceLayerFields.Eyes>("Avatar/Layers/Face/Freckles", new Color(0.642f, 0.533f, 0.437f))
                .WithFaceLayer<S1API.Entities.Appearances.FaceLayerFields.FacialHair>("Avatar/Layers/Face/FacialHair_Stubble", Color.black)
                .WithBodyLayer<S1API.Entities.Appearances.BodyLayerFields.Shirts>("Avatar/Layers/Top/FastFood T-Shirt", new Color(1.0f, 0.9f, 0.7f))
                .WithBodyLayer<S1API.Entities.Appearances.BodyLayerFields.Pants>("Avatar/Layers/Bottom/Jeans", new Color(0.23529411852359773f, 0.23529411852359773f, 0.23529411852359773f))
                .WithAccessoryLayer<S1API.Entities.Appearances.AccessoryFields.Feet>("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.23529411852359773f, 0.23529411852359773f, 0.23529411852359773f))
                .WithAccessoryLayer<S1API.Entities.Appearances.AccessoryFields.Waist>("Avatar/Accessories/Waist/Belt/Belt", new Color(0.23529411852359773f, 0.23529411852359773f, 0.23529411852359773f));
        }
    }
}


