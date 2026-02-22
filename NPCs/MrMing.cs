using MelonLoader;
using S1API.Economy;
using S1API.Entities;
using S1API.Entities.Schedule;
using S1API.GameTime;
using S1API.Products;
using S1API.Properties;
using UnityEngine;

namespace MoreNPCs.NPCs
{
    /// <summary>
    /// Mr. Ming - Chinese elder. Empty schedule (add positions manually).
    /// </summary>
    public sealed class MrMing : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            Vector3 spawnPos = new Vector3(-54.3375f, -3.025f, 132.0033f);

            builder.WithIdentity("mr_ming", "Mr.", "Ming")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 0.95f;
                    av.Weight = 0.45f;
                    av.SkinColor = new Color(0.867f, 0.7369f, 0.5623f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(1.0f, 0.95f, 0.9f);
                    av.PupilDilation = 0.7f;
                    av.EyebrowScale = 1.1f;
                    av.EyebrowThickness = 1.2f;
                    av.EyebrowRestingHeight = -0.3f;
                    av.EyebrowRestingAngle = 2.5f;
                    av.LeftEye = (0.32f, 0.38f);
                    av.RightEye = (0.32f, 0.38f);
                    av.HairColor = new Color(0.24f, 0.21f, 0.18f); // Lightened dark brown
                    av.HairPath = "Avatar/Hair/Receding/Receding";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Neutral", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/OldPersonWrinkles", new Color(0.62f, 0.30f, 0.60f)); // ~20% less vibrant
                    av.WithBodyLayer("Avatar/Layers/Top/ButtonUp", new Color(0.56f, 0.12f, 0.10f)); // Red - lucky color (~20% less vibrant)
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.2f, 0.2f, 0.2f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/DressShoes/DressShoes", new Color(0.15f, 0.15f, 0.15f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/Belt/Belt", new Color(0.15f, 0.15f, 0.15f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/SmallRoundGlasses/SmallRoundGlasses", new Color(0.2f, 0.2f, 0.2f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 300f, maxWeekly: 700f)
                        .WithOrdersPerWeek(2, 4)
                        .WithPreferredOrderDay(Day.Friday)
                        .WithOrderTime(1200)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.1f)
                        .WithDependence(baseAddiction: 0.1f, dependenceMultiplier: 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.3f), (DrugType.Methamphetamine, 0.2f), (DrugType.Shrooms, 0.26f), (DrugType.Cocaine, 0.1f)
                        })
                        .WithPreferredProperties(Property.Calming, Property.Refreshing);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("ming");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new StayInBuildingSpec { BuildingName = "Chinese Restaurant", StartTime = 503, DurationMinutes = 1439 });
                });
        }

        public MrMing() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 3f;
                Region = S1API.Map.Region.Northtown;

                Schedule.Enable();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"MrMing OnCreated failed: {ex.Message}");
                MelonLogger.Error($"StackTrace: {ex.StackTrace}");
            }
        }
    }
}
