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
    /// <summary>Rick Torres' sibling, age 17 — lives in Long House (main door). No connections.</summary>
    public sealed class SofiaTorres : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            Vector3 spawnPos = new Vector3(65.3f, 4.935f, -86.8f);

            builder.WithIdentity("sofia_torres", "Sofia", "Torres")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 1.0f;
                    av.Height = 0.99f;
                    av.Weight = 0.34f;
                    av.SkinColor = new Color(0.54f, 0.42f, 0.34f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.97f, 0.93f, 0.89f);
                    av.PupilDilation = 0.74f;
                    av.EyebrowScale = 0.94f;
                    av.EyebrowThickness = 0.80f;
                    av.EyebrowRestingHeight = 0.00f;
                    av.EyebrowRestingAngle = 1.85f;
                    av.LeftEye = (0.33f, 0.42f);
                    av.RightEye = (0.33f, 0.42f);
                    av.HairColor = new Color(0.08f, 0.06f, 0.05f);
                    av.HairPath = "Avatar/Hair/SidePartBob/SidePartBob";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/Tucked T-Shirt", new Color(0.48f, 0.42f, 0.52f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.20f, 0.22f, 0.28f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.86f, 0.86f, 0.88f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 200f, maxWeekly: 500f)
                        .WithOrdersPerWeek(1, 3)
                        .WithPreferredOrderDay(Day.Thursday)
                        .WithOrderTime(1700)
                        .WithStandards(CustomerStandard.Low)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.0f, maxAt100: 3.4f)
                        .WithCallPoliceChance(0.14f)
                        .WithDependence(baseAddiction: 0.06f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.40f), (DrugType.Methamphetamine, 0.08f), (DrugType.Shrooms, 0.15f), (DrugType.Cocaine, 0.18f)
                        })
                        .WithPreferredProperties(Property.Euphoric, Property.Sneaky);
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
                    plan.Add(new StayInBuildingSpec { BuildingName = "Long House", StartTime = 0018, DurationMinutes = 1421 });
                });
        }

        public SofiaTorres() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.40f;
                Region = Region.Suburbia;
                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"SofiaTorres OnCreated failed: {ex.Message}");
            }
        }
    }
}
