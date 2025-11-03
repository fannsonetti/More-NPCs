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
    public sealed class HenryMitchell : NPC
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
            Vector3 spawnPos = new Vector3(-10.8076f, 1.065f, 66.7038f);
            // var building = Buildings.GetAll().First();
            builder.WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 500f, maxWeekly: 900f)
                        .WithOrdersPerWeek(1, 5)
                        .WithPreferredOrderDay(Day.Thursday)
                        .WithOrderTime(1200)
                        .WithStandards(CustomerStandard.Low)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.33f)
                        .WithDependence(baseAddiction: 0.0f, dependenceMultiplier: 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.15f), (DrugType.Methamphetamine, -0.52f), (DrugType.Cocaine, 0.64f)
                        })
                        .WithPreferredProperties(Property.Calming, Property.ThoughtProvoking, Property.TropicThunder);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("jane_lucero", "billy_kramer");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new WalkToSpec { Destination = pos3, StartTime = 900, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = pos1, StartTime = 1100, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = pos2, StartTime = 1300, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = pos4, StartTime = 1500, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = pos5, StartTime = 1700, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = pos2, StartTime = 1900, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = pos4, StartTime = 2100, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = pos1, StartTime = 2300, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = pos5, StartTime = 100, FaceDestinationDirection = true });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Docks Industrial Building", StartTime = 200, DurationMinutes = 420 });
                });
        }

        public HenryMitchell() : base(
            id: "henry_mitchell",
            firstName: "Henry",
            lastName: "Mitchell",
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
                .Set<S1API.Entities.Appearances.CustomizationFields.Height>(1.0f)
                .Set<S1API.Entities.Appearances.CustomizationFields.Weight>(0.25f)
                .Set<S1API.Entities.Appearances.CustomizationFields.SkinColor>(new Color(0.713f, 0.592f, 0.486f))
                .Set<S1API.Entities.Appearances.CustomizationFields.EyeBallTint>(new Color(1.0f, 0.8f, 0.8f))
                .Set<S1API.Entities.Appearances.CustomizationFields.PupilDilation>(0.75f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyebrowScale>(1.39f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyebrowThickness>(0.7f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyebrowRestingHeight>(-0.48f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyebrowRestingAngle>(-4.64f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyeLidRestingStateLeft>((0.18f, 0.24f))
                .Set<S1API.Entities.Appearances.CustomizationFields.EyeLidRestingStateRight>((0.18f, 0.24f))
                .Set<S1API.Entities.Appearances.CustomizationFields.HairColor>(new Color(0.55f, 0.41f, 0.31f))
                .Set<S1API.Entities.Appearances.CustomizationFields.HairStyle>("Avatar/Hair/CloseBuzzCut/CloseBuzzCut")
                .WithFaceLayer<S1API.Entities.Appearances.FaceLayerFields.Face>("Avatar/Layers/Face/Face_SlightSmile", Color.black)
                .WithFaceLayer<S1API.Entities.Appearances.FaceLayerFields.FacialHair>("Avatar/Layers/Face/FacialHair_Stubble", Color.black)
                .WithBodyLayer<S1API.Entities.Appearances.BodyLayerFields.Shirts>("Avatar/Layers/Top/T-Shirt", new Color(0.481f, 0.331f, 0.225f))
                .WithBodyLayer<S1API.Entities.Appearances.BodyLayerFields.Pants>("Avatar/Layers/Bottom/Jeans", new Color(0.23529411852359773f, 0.23529411852359773f, 0.23529411852359773f))
                .WithAccessoryLayer<S1API.Entities.Appearances.AccessoryFields.Feet>("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.23529411852359773f, 0.23529411852359773f, 0.23529411852359773f))
                .WithAccessoryLayer<S1API.Entities.Appearances.AccessoryFields.Chest>("Avatar/Accessories/Chest/Blazer/Blazer", new Color(0.613f, 0.493f, 0.344f));
        }
    }
}


