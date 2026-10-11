using MelonLoader;
using S1API.Economy;
using S1API.Entities;
using S1API.Entities.Schedule;
using S1API.GameTime;
using S1API.Map;
using S1API.Map.Buildings;
using S1API.Products;
using S1API.Properties;
using UnityEngine;

namespace MoreNPCs.NPCs
{
    /// <summary>Wilkinson sibling — connection: Skyler Wilkinson.</summary>
    public sealed class AveryWilkinson : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var cafe = Building.Get<Cafe>();
            var supermarket = Building.Get<Supermarket>();
            Vector3 spawnPos = new Vector3(69.2f, 5.5412f, -119.2f);

            builder.WithIdentity("avery_wilkinson", "Avery", "Wilkinson")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.35f;
                    av.Height = 1.00f;
                    av.Weight = 0.36f;
                    av.SkinColor = new Color(0.78f, 0.65f, 0.54f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(1f, 1f, 1f);
                    av.PupilDilation = 0.72f;
                    av.EyebrowScale = 0.90f;
                    av.EyebrowThickness = 1.05f;
                    av.EyebrowRestingHeight = -0.02f;
                    av.EyebrowRestingAngle = 1.80f;
                    av.LeftEye = (0.36f, 0.20f);
                    av.RightEye = (0.36f, 0.20f);
                    av.HairColor = new Color(0.70f, 0.52f, 0.24f);
                    av.HairPath = "Avatar/Hair/MidFringe/MidFringe";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/Tucked T-Shirt", new Color(0.32f, 0.40f, 0.48f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.22f, 0.24f, 0.30f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.20f, 0.20f, 0.22f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 650f, maxWeekly: 1100f)
                        .WithOrdersPerWeek(1, 3)
                        .WithPreferredOrderDay(Day.Wednesday)
                        .WithOrderTime(1800)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.3f, maxAt100: 3.9f)
                        .WithCallPoliceChance(0.24f)
                        .WithDependence(baseAddiction: 0.05f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.35f), (DrugType.Methamphetamine, 0.15f), (DrugType.Shrooms, -0.20f), (DrugType.Cocaine, 0.55f)
                        })
                        .WithPreferredProperties(Property.Glowie, Property.Focused);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(true)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("skyler_wilkinson");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new StayInBuildingSpec { BuildingName = "Wilkinson House", StartTime = 2205, DurationMinutes = 545 });
                    plan.StayInBuilding(supermarket, 0830, 70);
                    plan.StayInBuilding(cafe, 1105, 95);
                    plan.Add(new StayInBuildingSpec { BuildingName = "Small Tower", StartTime = 1310, DurationMinutes = 210 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Wilkinson House", StartTime = 1700, DurationMinutes = 300 });
                });
        }

        public AveryWilkinson() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.50f;
                Region = Region.Suburbia;
                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"AveryWilkinson OnCreated failed: {ex.Message}");
            }
        }
    }
}
