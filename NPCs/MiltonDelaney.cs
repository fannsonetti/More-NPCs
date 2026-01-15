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
            builder.WithIdentity("milton_delaney", "Milton", "Delaney")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 1f;
                    av.Weight = 0.7f;
                    av.SkinColor = new Color(0.874f, 0.741f, 0.631f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(1.0f, 1.0f, 1.0f);
                    av.PupilDilation = 0.75f;
                    av.EyebrowScale = 1.208f;
                    av.EyebrowThickness = 1.6f;
                    av.EyebrowRestingHeight = -0.335f;
                    av.EyebrowRestingAngle = 10f;
                    av.LeftEye = (0.296f, 0.164f);
                    av.RightEye = (0.296f, 0.164f);
                    av.HairColor = new Color(0.519f, 0.519f, 0.519f);
                    av.HairPath = "Avatar/Hair/Balding/Balding";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Agitated", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Stubble", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/OldPersonWrinkles", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/Tucked T-Shirt", new Color(0.34f, 0.38f, 0.42f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.29f, 0.28f, 0.20f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.151f, 0.151f, 0.151f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/Blazer/Blazer", new Color(0.33f, 0.24f, 0.14f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/Belt/Belt", new Color(0.151f, 0.151f, 0.151f));
                })
                .WithSpawnPosition(spawnPos)
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
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("jeff_gilmore", "eleanor_briggs");
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

        public MiltonDelaney() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 5f;
                Region = Region.Downtown;

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
