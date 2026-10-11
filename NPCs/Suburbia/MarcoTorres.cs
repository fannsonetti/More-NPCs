using MelonLoader;
using S1API.Economy;
using S1API.Entities;
using S1API.Entities.Schedule;
using S1API.GameTime;
using S1API.Map;
using S1API.Products;
using S1API.Properties;
using UnityEngine;

namespace MoreNPCs.NPCs
{
    /// <summary>Rick Torres' father — lives in Long House (main door). No connections.</summary>
    public sealed class MarcoTorres : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            Vector3 spawnPos = new Vector3(65.5f, 4.935f, -87.2f);

            builder.WithIdentity("marco_torres", "Marco", "Torres")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 1.02f;
                    av.Weight = 0.52f;
                    av.SkinColor = new Color(0.50f, 0.38f, 0.30f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.96f, 0.90f, 0.86f);
                    av.PupilDilation = 0.58f;
                    av.EyebrowScale = 1.10f;
                    av.EyebrowThickness = 1.05f;
                    av.EyebrowRestingHeight = -0.08f;
                    av.EyebrowRestingAngle = 0.40f;
                    av.LeftEye = (0.30f, 0.40f);
                    av.RightEye = (0.30f, 0.40f);
                    av.HairColor = new Color(0.12f, 0.09f, 0.07f);
                    av.HairPath = "Avatar/Hair/Receding/Receding";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Neutral", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Stubble", new Color(0.10f, 0.08f, 0.06f));
                    av.WithBodyLayer("Avatar/Layers/Top/Buttonup", new Color(0.28f, 0.32f, 0.38f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.18f, 0.20f, 0.24f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.14f, 0.14f, 0.16f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/Belt/Belt", new Color(0.12f, 0.12f, 0.12f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 600f, maxWeekly: 1100f)
                        .WithOrdersPerWeek(1, 2)
                        .WithPreferredOrderDay(Day.Saturday)
                        .WithOrderTime(1900)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.0f, maxAt100: 3.5f)
                        .WithCallPoliceChance(0.16f)
                        .WithDependence(baseAddiction: 0.08f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.28f), (DrugType.Methamphetamine, 0.10f), (DrugType.Shrooms, 0.12f), (DrugType.Cocaine, 0.34f)
                        })
                        .WithPreferredProperties(Property.Sedating, Property.Munchies);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(true)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new StayInBuildingSpec { BuildingName = "Long House", StartTime = 0005, DurationMinutes = 1434 });
                });
        }

        public MarcoTorres() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.48f;
                Region = Region.Suburbia;
                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"MarcoTorres OnCreated failed: {ex.Message}");
            }
        }
    }
}
