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
    public sealed class EleanorBriggs : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            Vector3 hospital1 = new Vector3(105.9473f, 1.815f, 56.774f);
            Vector3 hospital2 = new Vector3(100.1469f, 1.065f, 55.4907f);
            Vector3 hospital3 = new Vector3(100.0152f, 1.815f, 56.8694f);
            Vector3 spawnPos = new Vector3(-11.0538f, 1.065f, 66.753f);
            builder.WithIdentity("eleanor_briggs", "Eleanor", "Briggs")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 1.0f;
                    av.Height = 0.93f;
                    av.Weight = 1.0f;
                    av.SkinColor = new Color(0.784f, 0.655f, 0.545f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(1.0f, 1.0f, 1.0f);
                    av.PupilDilation = 0.75f;
                    av.EyebrowScale = 0.907f;
                    av.EyebrowThickness = 0.981f;
                    av.EyebrowRestingHeight = 0.419f;
                    av.EyebrowRestingAngle = 1.806f;
                    av.LeftEye = (0.258f, 0.306f);
                    av.RightEye = (0.258f, 0.306f);
                    av.HairColor = new Color(0.519f, 0.519f, 0.519f);
                    av.HairPath = "Avatar/Hair/lowbun/LowBun";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Neutral", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/OldPersonWrinkles", new Color32(255, 0, 255, 5));
                    av.WithBodyLayer("Avatar/Layers/Top/T-Shirt", new Color(0.651f, 0.236f, 0.236f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.235f, 0.235f, 0.235f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.151f, 0.151f, 0.151f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/CollarJacket/CollarJacket", new Color(0.613f, 0.493f, 0.344f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 400f, maxWeekly: 800f)
                        .WithOrdersPerWeek(1, 5)
                        .WithPreferredOrderDay(Day.Saturday)
                        .WithOrderTime(0800)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.35f)
                        .WithDependence(baseAddiction: 0.0f, dependenceMultiplier: 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.53f), (DrugType.Methamphetamine, 0.92f), (DrugType.Cocaine, -0.24f)
                        })
                        .WithPreferredProperties(Property.Jennerising, Property.Gingeritis, Property.TropicThunder);
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
                    plan.Add(new StayInBuildingSpec { BuildingName = "HAM Legal", StartTime = 1130, DurationMinutes = 90 });
                    plan.Add(new WalkToSpec { Destination = hospital1, StartTime = 1300, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = hospital3, StartTime = 1400, FaceDestinationDirection = true });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Hyland Medical", StartTime = 1500, DurationMinutes = 120 });
                    plan.Add(new WalkToSpec { Destination = hospital2, StartTime = 1700, FaceDestinationDirection = true });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Hyland Medical", StartTime = 1830, DurationMinutes = 90 });
                    plan.Add(new WalkToSpec { Destination = hospital3, StartTime = 2000, FaceDestinationDirection = true });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Apartment Buiding", StartTime = 2100, DurationMinutes = 870 });
                });
        }

        public EleanorBriggs() : base()
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


