using MelonLoader;
using S1API.Entities;
using S1API.Entities.Schedule;
using S1API.Map;
using S1API.Money;
using S1API.Economy;
using S1API.Entities.NPCs.Suburbia;
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
    public sealed class OfficerMarcus : NPC
    {
        protected override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            Vector3 pos1 = new Vector3(27.4168f, 1.065f, -14.4305f);
            Vector3 pos2 = new Vector3(-22.5378f, 1.065f, -43.1838f);
            Vector3 pos3 = new Vector3(-150.581f, -2.935f, 118.1681f);
            Vector3 pos4 = new Vector3(-139.3423f, -4.335f, 18.8808f);
            Vector3 pos5 = new Vector3(-121.0444f, -2.935f, 78.6722f);
            Vector3 pos6 = new Vector3(-22.7021f, 1.065f, 46.8433f);
            Vector3 spawnPos = new Vector3(16.0717f, 1.065f, 38.0883f);
            builder.WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 800f, maxWeekly: 1200f)
                        .WithOrdersPerWeek(1, 5)
                        .WithPreferredOrderDay(Day.Sunday)
                        .WithOrderTime(2330)
                        .WithStandards(CustomerStandard.High)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.90f)
                        .WithDependence(baseAddiction: 0.0f, dependenceMultiplier: 1.1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.52f), (DrugType.Methamphetamine, -0.84f), (DrugType.Cocaine, -0.09f)
                        })
                        .WithPreferredProperties(Property.Athletic, Property.AntiGravity, Property.Sneaky);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(1.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("alison_knight", "carl_bundy", "jack_knight");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new WalkToSpec { Destination = pos1, StartTime = 700, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = pos2, StartTime = 900, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = pos3, StartTime = 1100, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = pos4, StartTime = 1300, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = pos5, StartTime = 1500, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = pos6, StartTime = 1700, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = pos3, StartTime = 1900, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = pos5, StartTime = 2100, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = pos2, StartTime = 2300, FaceDestinationDirection = true });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Police Station", StartTime = 2400, DurationMinutes = 420 });
                });
        }

        public OfficerMarcus() : base(
            id: "officer_marcus",
            firstName: "Officer",
            lastName: "Marcus",
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
                .Set<S1API.Entities.Appearances.CustomizationFields.Gender>(0.0f)
                .Set<S1API.Entities.Appearances.CustomizationFields.Height>(1.0f)
                .Set<S1API.Entities.Appearances.CustomizationFields.Weight>(0.4f)
                .Set<S1API.Entities.Appearances.CustomizationFields.SkinColor>(new Color(0.713f, 0.592f, 0.486f))
                .Set<S1API.Entities.Appearances.CustomizationFields.EyeBallTint>(new Color(1.0f, 0.8f, 0.8f))
                .Set<S1API.Entities.Appearances.CustomizationFields.PupilDilation>(1.0f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyebrowScale>(1.1f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyebrowThickness>(1.0f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyebrowRestingHeight>(-0.432f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyebrowRestingAngle>(-2.451f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyeLidRestingStateLeft>((0.219f, 0.5f))
                .Set<S1API.Entities.Appearances.CustomizationFields.EyeLidRestingStateRight>((0.219f, 0.5f))
                .Set<S1API.Entities.Appearances.CustomizationFields.HairColor>(new Color(0.31f, 0.2f, 0.12f))
                .Set<S1API.Entities.Appearances.CustomizationFields.HairStyle>("Avatar/Hair/BuzzCut/BuzzCut")
                .WithFaceLayer<S1API.Entities.Appearances.FaceLayerFields.Face>("Avatar/Layers/Face/Face_NeutralPout", Color.black)
                .WithFaceLayer<S1API.Entities.Appearances.FaceLayerFields.Eyes>("Avatar/Layers/Face/OldPersonWrinkles", new Color(0.957f, 0.474f, 0.938f))
                .WithFaceLayer<S1API.Entities.Appearances.FaceLayerFields.FacialHair>("Avatar/Layers/Face/FacialHair_Goatee", new Color(0.31f, 0.2f, 0.12f))
                .WithBodyLayer<S1API.Entities.Appearances.BodyLayerFields.Shirts>("Avatar/Layers/Top/RolledButtonUp", new Color(0.178f, 0.217f, 0.406f))
                .WithBodyLayer<S1API.Entities.Appearances.BodyLayerFields.Pants>("Avatar/Layers/Bottom/Jeans", new Color(0.235f, 0.235f, 0.235f))
                .WithAccessoryLayer<S1API.Entities.Appearances.AccessoryFields.Feet>("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(0.151f, 0.151f, 0.151f))
                .WithAccessoryLayer<S1API.Entities.Appearances.AccessoryFields.Waist>("Avatar/Accessories/Waist/PoliceBelt/PoliceBelt", new Color(0.151f, 0.151f, 0.151f))
                .WithAccessoryLayer<S1API.Entities.Appearances.AccessoryFields.Chest>("Avatar/Accessories/Chest/BulletProofVest/BulletProofVest_Police", Color.white);
        }
    }
}


