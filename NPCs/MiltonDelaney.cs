using MelonLoader;
using S1API.Economy;
using S1API.Entities;
using S1API.Entities.NPCs.Downtown;
using S1API.Entities.Schedule;
using S1API.GameTime;
using S1API.Map;
using S1API.Money;
using S1API.Products;
using S1API.Properties;
using S1API.Vehicles;
using UnityEngine;
using UnityEngine.UI;

namespace CustomNPCTest.NPCs
{
    /// <summary>
    /// An example S1API NPC that opts into a physical rig.
    /// Demonstrates movement and inventory usage.
    /// </summary>
    public sealed class MiltonDelaney : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            Vector3 hospital1 = new Vector3(105.9473f, 1.815f, 56.774f);
            Vector3 hospital2 = new Vector3(100.1469f, 1.065f, 55.4907f);
            Vector3 hospital3 = new Vector3(100.0152f, 1.815f, 56.8694f);
            Vector3 spawnPos = new Vector3(134.1605f, 6.0623f, 114.3804f);
            builder.WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 400f, maxWeekly: 800f)
                        .WithOrdersPerWeek(1, 5)
                        .WithPreferredOrderDay(Day.Saturday)
                        .WithOrderTime(1100)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.35f)
                        .WithDependence(baseAddiction: 0.0f, dependenceMultiplier: 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.64f), (DrugType.Methamphetamine, 0.86f), (DrugType.Cocaine, -0.22f)
                        })
                        .WithPreferredProperties(Property.Munchies, Property.Refreshing, Property.Zombifying);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.5f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("michael_boog", "tobias_wentworth");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new StayInBuildingSpec { BuildingName = "Hyland Medical", StartTime = 1130, DurationMinutes = 90 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "HAM Legal", StartTime = 1300, DurationMinutes = 120 });
                    plan.Add(new WalkToSpec { Destination = hospital3, StartTime = 1500, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = hospital1, StartTime = 1600, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = hospital2, StartTime = 1700, FaceDestinationDirection = true });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Hyland Medical", StartTime = 1930, DurationMinutes = 90 });
                    plan.Add(new WalkToSpec { Destination = hospital3, StartTime = 2000, FaceDestinationDirection = true });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Apartment Buiding", StartTime = 2100, DurationMinutes = 870 });
                });
        }

        public MiltonDelaney() : base(
            id: "milton_delaney",
            firstName: "Milton",
            lastName: "Delaney",
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
                Region = Region.Downtown;

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
                .Set<S1API.Entities.Appearances.CustomizationFields.Height>(1f)
                .Set<S1API.Entities.Appearances.CustomizationFields.Weight>(0.7f)
                .Set<S1API.Entities.Appearances.CustomizationFields.SkinColor>(new Color(0.874f, 0.741f, 0.631f))
                .Set<S1API.Entities.Appearances.CustomizationFields.EyeBallTint>(new Color(1.0f, 1.0f, 1.0f))
                .Set<S1API.Entities.Appearances.CustomizationFields.PupilDilation>(0.75f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyebrowScale>(1.208f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyebrowThickness>(1.6f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyebrowRestingHeight>(-0.335f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyebrowRestingAngle>(10f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyeLidRestingStateLeft>((0.296f, 0.164f))
                .Set<S1API.Entities.Appearances.CustomizationFields.EyeLidRestingStateRight>((0.296f, 0.164f))
                .Set<S1API.Entities.Appearances.CustomizationFields.HairColor>(new Color(0.519f, 0.519f, 0.519f))
                .Set<S1API.Entities.Appearances.CustomizationFields.HairStyle>("Avatar/Hair/Balding/Balding")
                .WithFaceLayer<S1API.Entities.Appearances.FaceLayerFields.Face>("Avatar/Layers/Face/Face_Agitated", Color.black)
                .WithFaceLayer<S1API.Entities.Appearances.FaceLayerFields.FacialHair>("Avatar/Layers/Face/FacialHair_Stubble", Color.black)
                .WithFaceLayer<S1API.Entities.Appearances.FaceLayerFields.Eyes>("Avatar/Layers/Face/OldPersonWrinkles", new Color32(255, 0, 255, 5))
                .WithBodyLayer<S1API.Entities.Appearances.BodyLayerFields.Shirts>("Avatar/Layers/Top/Tucked T-Shirt", new Color(0.34f, 0.38f, 0.42f))
                .WithBodyLayer<S1API.Entities.Appearances.BodyLayerFields.Pants>("Avatar/Layers/Bottom/Jeans", new Color(0.29f, 0.28f, 0.20f))
                .WithAccessoryLayer<S1API.Entities.Appearances.AccessoryFields.Feet>("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.151f, 0.151f, 0.151f))
                .WithAccessoryLayer<S1API.Entities.Appearances.AccessoryFields.Chest>("Avatar/Accessories/Chest/Blazer/Blazer", new Color(0.33f, 0.24f, 0.14f))
                .WithAccessoryLayer<S1API.Entities.Appearances.AccessoryFields.Waist>("Avatar/Accessories/Waist/Belt/Belt", new Color(0.151f, 0.151f, 0.151f));
        }
    }
}
