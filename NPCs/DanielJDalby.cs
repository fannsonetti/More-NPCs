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
    public sealed class DanielJDalby : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            Vector3 casinoBalcony = new Vector3(13.8808f, 5.165f, 92.364f);
            Vector3 slotMachine1 = new Vector3(25.0217f, 1.865f, 95.0692f);
            Vector3 rideTheBus = new Vector3(13.8508f, 1.865f, 94.4451f);
            Vector3 blackJack = new Vector3(17.4742f, 1.865f, 93.5125f);
            Vector3 frontDesk = new Vector3(19.1643f, 1.865f, 87.4606f);
            // var building = Buildings.GetAll().First();
            builder.WithSpawnPosition(casinoBalcony)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 500f, maxWeekly: 1200f)
                        .WithOrdersPerWeek(3, 5)
                        .WithPreferredOrderDay(Day.Saturday)
                        .WithOrderTime(1100)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.15f)
                        .WithDependence(baseAddiction: 0.25f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, -0.52f), (DrugType.Methamphetamine, -0.86f), (DrugType.Cocaine, 0.14f)
                        })
                        .WithPreferredProperties(Property.Slippery, Property.Electrifying, Property.BrightEyed);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("ray_hoffman", "lily_turner");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new WalkToSpec { Destination = frontDesk, StartTime = 1600, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = blackJack, StartTime = 1630, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = slotMachine1, StartTime = 1830, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = rideTheBus, StartTime = 2100, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = slotMachine1, StartTime = 2300, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = casinoBalcony, StartTime = 030, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = slotMachine1, StartTime = 200, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = frontDesk, StartTime = 300, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = blackJack, StartTime = 330, FaceDestinationDirection = true });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Les Ordures Puantes", StartTime = 500, DurationMinutes = 660 });
                });
        }

        public DanielJDalby() : base(
            id: "daniel_j_dalby",
            firstName: "Daniel J.",
            lastName: "D'alby",
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
                Region = Region.Uptown;

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
                .Set<S1API.Entities.Appearances.CustomizationFields.Height>(1.07f)
                .Set<S1API.Entities.Appearances.CustomizationFields.Weight>(0.5f)
                .Set<S1API.Entities.Appearances.CustomizationFields.SkinColor>(new Color(0.615f, 0.498f, 0.392f))
                .Set<S1API.Entities.Appearances.CustomizationFields.EyeBallTint>(new Color(1.0f, 0.8f, 0.8f))
                .Set<S1API.Entities.Appearances.CustomizationFields.PupilDilation>(0.46f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyebrowScale>(1.25f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyebrowThickness>(1.6f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyebrowRestingHeight>(-0.38f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyebrowRestingAngle>(6.64f)
                .Set<S1API.Entities.Appearances.CustomizationFields.EyeLidRestingStateLeft>((0.274f, 0.316f))
                .Set<S1API.Entities.Appearances.CustomizationFields.EyeLidRestingStateRight>((0.274f, 0.316f))
                .Set<S1API.Entities.Appearances.CustomizationFields.HairColor>(new Color(0.075f, 0.075f, 0.075f))
                .Set<S1API.Entities.Appearances.CustomizationFields.HairStyle>("Avatar/Hair/Spiky/Spiky")
                .WithFaceLayer<S1API.Entities.Appearances.FaceLayerFields.Face>("Avatar/Layers/Face/Face_SmugPout", Color.black)
                .WithFaceLayer<S1API.Entities.Appearances.FaceLayerFields.FacialHair>("Avatar/Layers/Face/FacialHair_Swirl", Color.black)
                .WithFaceLayer<S1API.Entities.Appearances.FaceLayerFields.FacialHair>("Avatar/Layers/Face/FacialHair_Stubble", Color.black)
                .WithBodyLayer<S1API.Entities.Appearances.BodyLayerFields.Shirts>("Avatar/Layers/Top/Buttonup", new Color(1f, 1f, 1f))
                .WithBodyLayer<S1API.Entities.Appearances.BodyLayerFields.Pants>("Avatar/Layers/Bottom/Jeans", new Color(0.235f, 0.235f, 0.235f))
                .WithAccessoryLayer<S1API.Entities.Appearances.AccessoryFields.Feet>("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(0.33f, 0.24f, 0.14f))
                .WithAccessoryLayer<S1API.Entities.Appearances.AccessoryFields.Chest>("Avatar/Accessories/Chest/OpenVest/OpenVest", new Color(0.651f, 0.236f, 0.236f))
                .WithAccessoryLayer<S1API.Entities.Appearances.AccessoryFields.Neck>("Avatar/Accessories/Neck/GoldChain/GoldChain", new Color(0.651f, 0.236f, 0.236f))
                .WithAccessoryLayer<S1API.Entities.Appearances.AccessoryFields.Waist>("Avatar/Accessories/Waist/Belt/Belt", new Color(0.151f, 0.151f, 0.151f));
        }
    }
}


