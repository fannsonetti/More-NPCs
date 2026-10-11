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
    /// <summary>Rick Torres' mother — lives in Long House (main door). No connections.</summary>
    public sealed class LuciaTorres : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            Vector3 spawnPos = new Vector3(65.7f, 4.935f, -87.0f);

            builder.WithIdentity("lucia_torres", "Lucia", "Torres")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 1.0f;
                    av.Height = 0.98f;
                    av.Weight = 0.40f;
                    av.SkinColor = new Color(0.58f, 0.44f, 0.36f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.97f, 0.92f, 0.88f);
                    av.PupilDilation = 0.66f;
                    av.EyebrowScale = 0.96f;
                    av.EyebrowThickness = 0.78f;
                    av.EyebrowRestingHeight = -0.02f;
                    av.EyebrowRestingAngle = 2.10f;
                    av.LeftEye = (0.32f, 0.41f);
                    av.RightEye = (0.32f, 0.41f);
                    av.HairColor = new Color(0.10f, 0.07f, 0.06f);
                    av.HairPath = "Avatar/Hair/Bun/Bun";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/RolledButtonUp", new Color(0.42f, 0.36f, 0.44f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/FemaleUnderwear", new Color(0.94f, 0.92f, 0.90f));
                    av.WithAccessoryLayer("Avatar/Accessories/Bottom/MediumSkirt/MediumSkirt", new Color(0.22f, 0.24f, 0.30f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Flats/Flats", new Color(0.14f, 0.12f, 0.12f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 500f, maxWeekly: 900f)
                        .WithOrdersPerWeek(1, 2)
                        .WithPreferredOrderDay(Day.Friday)
                        .WithOrderTime(1600)
                        .WithStandards(CustomerStandard.High)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.2f, maxAt100: 3.8f)
                        .WithCallPoliceChance(0.22f)
                        .WithDependence(baseAddiction: 0.04f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.22f), (DrugType.Methamphetamine, -0.10f), (DrugType.Shrooms, 0.18f), (DrugType.Cocaine, 0.20f)
                        })
                        .WithPreferredProperties(Property.Refreshing, Property.Sedating);
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
                    plan.Add(new StayInBuildingSpec { BuildingName = "Long House", StartTime = 0012, DurationMinutes = 1427 });
                });
        }

        public LuciaTorres() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.36f;
                Region = Region.Suburbia;
                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"LuciaTorres OnCreated failed: {ex.Message}");
            }
        }
    }
}
