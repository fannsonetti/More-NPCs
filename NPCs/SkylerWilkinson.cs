using MelonLoader;
using S1API.Entities;
using S1API.Entities.Schedule;
using S1API.Map;
using S1API.Money;
using S1API.Economy;
using S1API.Entities.NPCs.Northtown;
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
    public sealed class SkylerWilkinson : NPC
    {
        protected override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            Vector3 hounddog = new Vector3(24.5503f, 1.065f, -36.1644f);
            Vector3 hylandauto = new Vector3(24.1091f, 1.065f, -40.8684f);
            Vector3 spawnPos = new Vector3(68.9616f, 5.5412f, -119.5116f);
            // var building = Buildings.GetAll().First();
            builder.WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 500f, maxWeekly: 1200f)
                        .WithOrdersPerWeek(1, 3)
                        .WithPreferredOrderDay(Day.Friday)
                        .WithOrderTime(0700)
                        .WithStandards(CustomerStandard.High)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.15f)
                        .WithDependence(baseAddiction: 0f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.39f), (DrugType.Methamphetamine, 0.21f), (DrugType.Cocaine, 0.77f)
                        })
                        // .WithPreferredPropertiesById("Munchies", "Energizing", "Cyclopean");
                        .WithPreferredProperties(Property.Schizophrenic, Property.Glowie, Property.Toxic);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("alison_knight", "officer_marcus");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new StayInBuildingSpec { BuildingName = "Tall Tower", StartTime = 0700, DurationMinutes = 240 });
                    plan.Add(new WalkToSpec { Destination = hounddog, StartTime = 1100, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = hylandauto, StartTime = 1215, FaceDestinationDirection = true });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Tall Tower", StartTime = 1300, DurationMinutes = 240 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Supermarket", StartTime = 1700, DurationMinutes = 120 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Cafe", StartTime = 1900, DurationMinutes = 120 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Wilkinson House", StartTime = 2100, DurationMinutes = 600 });
                });
        }

        public SkylerWilkinson() : base(
            id: "skyler_wilkinson",
            firstName: "Skyler",
            lastName: "Wilkinson",
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
                Region = Region.Suburbia;

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
                .Set<S1API.Entities.Appearances.CustomizationFields.Gender>(1.0f) // 0..1
                .Set<S1API.Entities.Appearances.CustomizationFields.Height>(0.97f)
                .Set<S1API.Entities.Appearances.CustomizationFields.Weight>(0.3f)
                .Set<S1API.Entities.Appearances.CustomizationFields.SkinColor>(new Color(0.784f, 0.654f, 0.545f))
                .Set<S1API.Entities.Appearances.CustomizationFields.EyeBallTint>(new Color(1.0f, 1f, 1f))
                .Set<S1API.Entities.Appearances.CustomizationFields.PupilDilation>(0.75f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyebrowScale>(0.882f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyebrowThickness>(1.19f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyebrowRestingHeight>(-0.019f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyebrowRestingAngle>(2.516f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyeLidRestingStateLeft>((0.387f, 0.4f))
                .Set<S1API.Entities.Appearances.CustomizationFields.EyeLidRestingStateRight>((0.387f, 0.4f))
                .Set<S1API.Entities.Appearances.CustomizationFields.HairColor>(new Color(0.716f, 0.527f, 0.226f))
                .Set<S1API.Entities.Appearances.CustomizationFields.HairStyle>("Avatar/Hair/SidePartBob/SidePartBob")
                .WithFaceLayer<S1API.Entities.Appearances.FaceLayerFields.Face>("Avatar/Layers/Face/Face_SlightSmile", Color.black)
                .WithBodyLayer<S1API.Entities.Appearances.BodyLayerFields.Shirts>("Avatar/Layers/Top/ButtonUp", new Color(1f, 1f, 1f))
                .WithBodyLayer<S1API.Entities.Appearances.BodyLayerFields.Pants>("Avatar/Layers/Bottom/FemaleUnderwear", new Color(1f, 1f, 1f))
                .WithAccessoryLayer<S1API.Entities.Appearances.AccessoryFields.Feet>("Avatar/Accessories/Feet/Flats/Flats", new Color(0.151f, 0.151f, 0.151f))
                .WithAccessoryLayer<S1API.Entities.Appearances.AccessoryFields.Bottom>("Avatar/Accessories/Bottom/MediumSkirt/MediumSkirt", new Color(0.178f, 0.217f, 0.406f));
        }
    }
}


