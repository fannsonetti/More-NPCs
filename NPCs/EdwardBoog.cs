using MelonLoader;
using S1API.Economy;
using S1API.Entities;
using S1API.Entities.NPCs.Downtown;
using S1API.Entities.NPCs.Uptown;
using S1API.Entities.Schedule;
using S1API.GameTime;
using S1API.Map;
using S1API.Money;
using S1API.Products;
using S1API.Properties;
using S1API.Properties.Tokens;
using S1API.Vehicles;
using UnityEngine;
using UnityEngine.UI;

namespace CustomNPCTest.NPCs
{
    /// <summary>
    /// An example S1API NPC that opts into a physical rig.
    /// Demonstrates movement and inventory usage.
    /// </summary>
    public sealed class EdwardBoog : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            Vector3 spawnPos = new Vector3(134.1605f, 6.0623f, 114.3804f);
            builder.WithIdentity("edward_boog", "Edward", "Boog")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 1f;
                    av.Weight = 0.5f;
                    av.SkinColor = new Color(0.874f, 0.741f, 0.631f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(1.0f, 0.8f, 0.8f);
                    av.PupilDilation = 0.46f;
                    av.EyebrowScale = 1.25f;
                    av.EyebrowThickness = 1.6f;
                    av.EyebrowRestingHeight = -0.38f;
                    av.EyebrowRestingAngle = 6.64f;
                    av.LeftEye = (0.24f, 0.16f);
                    av.RightEye = (0.24f, 0.16f);
                    av.HairColor = new Color(0.32f, 0.32f, 0.32f);
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Agitated", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/OldPersonWrinkles", new Color32(255, 0, 255, 5));
                    av.WithBodyLayer("Avatar/Layers/Top/Buttonup", new Color(1f, 1f, 1f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.235f, 0.235f, 0.235f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/DressShoes/DressShoes", new Color(0.151f, 0.151f, 0.151f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/CollarJacket/CollarJacket", new Color(0.613f, 0.493f, 0.344f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/PorkpieHat/PorkpieHat", new Color(0.481f, 0.331f, 0.225f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/SmallRoundGlasses/SmallRoundGlasses", new Color(1f, 0.756f, 0.212f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/Belt/Belt", new Color(0.151f, 0.151f, 0.151f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 2000f, maxWeekly: 8000f)
                        .WithOrdersPerWeek(1, 3)
                        .WithPreferredOrderDay(Day.Tuesday)
                        .WithOrderTime(2100)
                        .WithStandards(CustomerStandard.VeryHigh)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(1.0f)
                        .WithDependence(baseAddiction: 0.0f, dependenceMultiplier: 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, -0.77f), (DrugType.Methamphetamine, 0.21f), (DrugType.Cocaine, 0.74f)
                        })
                        .WithPreferredProperties(Property.Lethal, Property.Schizophrenic, Property.Explosive);
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
                    plan.Add(new StayInBuildingSpec { BuildingName = "Church", StartTime = 1130, DurationMinutes = 90 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Town hall", StartTime = 1300, DurationMinutes = 120 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Modern Mansion", StartTime = 1500, DurationMinutes = 180 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Slop Shop", StartTime = 1800, DurationMinutes = 180 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Modern Mansion", StartTime = 2100, DurationMinutes = 870 });
                });
        }

        public EdwardBoog() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 5f;
                Region = Region.Uptown;

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


