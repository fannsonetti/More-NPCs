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
    public sealed class Mack : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            Vector3 behindFishShopMack = new Vector3(-101.2265f, -1.485f, -42.492f);
            Vector3 waterMack = new Vector3(-67.4977f, -1.535f, -38.2393f);
            Vector3 raysForest = new Vector3(95.6527f, 1.065f, -19.1027f);
            Vector3 rockInForest = new Vector3(134.1952f, 1.1135f, -24.9628f);
            Vector3 scopeBank = new Vector3(73.4736f, 0.9662f, 39.0171f);
            Vector3 pos6 = new Vector3(-22.7021f, 1.065f, 46.8433f);
            Vector3 spawnPos = new Vector3(-11.8076f, 1.065f, 67.7038f);
            builder.WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 600f, maxWeekly: 1000f)
                        .WithOrdersPerWeek(1, 5)
                        .WithPreferredOrderDay(Day.Monday)
                        .WithOrderTime(0030)
                        .WithStandards(CustomerStandard.Low)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.1f)
                        .WithDependence(baseAddiction: 0.0f, dependenceMultiplier: 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.32f), (DrugType.Methamphetamine, -0.45f), (DrugType.Cocaine, 0.67f)
                        })
                        .WithPreferredProperties(Property.Focused, Property.Sneaky, Property.Euphoric);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("cranky_frank", "diesel");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new WalkToSpec { Destination = behindFishShopMack, StartTime = 0900, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = waterMack, StartTime = 1100, FaceDestinationDirection = true });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Hyland Bank", StartTime = 1300, DurationMinutes = 180 });
                    plan.Add(new WalkToSpec { Destination = raysForest, StartTime = 1600, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = rockInForest, StartTime = 1700, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = scopeBank, StartTime = 2000, FaceDestinationDirection = true });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Slop Shop", StartTime = 2200, DurationMinutes = 210 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Docks Industrial Building", StartTime = 0130, DurationMinutes = 450 });
                });
        }

        public Mack() : base(
            id: "mack",
            firstName: "Mack",
            lastName: "",
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

                Aggressiveness = 15f;
                Region = Region.Docks;

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
                .Set<S1API.Entities.Appearances.CustomizationFields.Height>(1.1f)
                .Set<S1API.Entities.Appearances.CustomizationFields.Weight>(1.0f)
                .Set<S1API.Entities.Appearances.CustomizationFields.SkinColor>(new Color(0.784f, 0.654f, 0.545f))
                .Set<S1API.Entities.Appearances.CustomizationFields.EyeBallTint>(new Color(1.0f, 0.8f, 0.8f))
                .Set<S1API.Entities.Appearances.CustomizationFields.PupilDilation>(0.75f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyebrowScale>(1.23f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyebrowThickness>(1.8f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyebrowRestingHeight>(-5.48f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyebrowRestingAngle>(10f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyeLidRestingStateLeft>((0.235f, 0.235f))
                .Set<S1API.Entities.Appearances.CustomizationFields.EyeLidRestingStateRight>((0.235f, 0.235f))
                .Set<S1API.Entities.Appearances.CustomizationFields.HairColor>(new Color(0.311f, 0.22f, 0.154f))
                .WithFaceLayer<S1API.Entities.Appearances.FaceLayerFields.Face>("Avatar/Layers/Face/Face_Agitated", Color.black)
                .WithFaceLayer<S1API.Entities.Appearances.FaceLayerFields.Eyes>("Avatar/Layers/Face/OldPersonWrinkles", new Color(0.957f, 0.474f, 0.938f))
                .WithFaceLayer<S1API.Entities.Appearances.FaceLayerFields.FacialHair>("Avatar/Layers/Face/FacialHair_Stubble", Color.black) 
                .WithBodyLayer<S1API.Entities.Appearances.BodyLayerFields.Shirts>("Avatar/Layers/Top/T-Shirt", new Color(0.178f, 0.217f, 0.406f))
                .WithBodyLayer<S1API.Entities.Appearances.BodyLayerFields.Pants>("Avatar/Layers/Bottom/Jeans", new Color(0.235f, 0.235f, 0.235f))
                .WithAccessoryLayer<S1API.Entities.Appearances.AccessoryFields.Feet>("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(0.33f, 0.24f, 0.14f))
                .WithAccessoryLayer<S1API.Entities.Appearances.AccessoryFields.Head>("Avatar/Accessories/Head/PorkpieHat/PorkpieHat", new Color(0.396f, 0.396f, 0.396f))
                .WithAccessoryLayer<S1API.Entities.Appearances.AccessoryFields.Chest>("Avatar/Accessories/Chest/Blazer/Blazer", new Color(0.33f, 0.24f, 0.14f));
        }
    }
}


