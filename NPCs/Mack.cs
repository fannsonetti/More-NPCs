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
            builder.WithIdentity("mack", "Mack", "")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 1.1f;
                    av.Weight = 1.0f;
                    av.SkinColor = new Color(0.784f, 0.654f, 0.545f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(1.0f, 0.8f, 0.8f);
                    av.PupilDilation = 0.75f;
                    av.EyebrowScale = 1.23f;
                    av.EyebrowThickness = 1.8f;
                    av.EyebrowRestingHeight = -5.48f;
                    av.EyebrowRestingAngle = 10f;
                    av.LeftEye = (0.235f, 0.235f);
                    av.RightEye = (0.235f, 0.235f);
                    av.HairColor = new Color(0.311f, 0.22f, 0.154f);
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Agitated", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/OldPersonWrinkles", new Color(0.957f, 0.474f, 0.938f));
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Stubble", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/T-Shirt", new Color(0.178f, 0.217f, 0.406f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.235f, 0.235f, 0.235f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(0.33f, 0.24f, 0.14f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/Beanie/Beanie", new Color(0.235f, 0.235f, 0.235f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/Blazer/Blazer", new Color(0.33f, 0.24f, 0.14f));
                })
                .WithSpawnPosition(spawnPos)
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

        public Mack() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 15f;
                Region = Region.Docks;

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


