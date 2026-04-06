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
    /// <summary>Midnight shift at The Crimson Canary — brown apron, laid-back; mornings at Carl's House.</summary>
    public sealed class CarlBundy : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var supermarket = Building.Get<Supermarket>();
            var cafe = Building.Get<Cafe>();
            Vector3 spawnPos = new Vector3(64.2f, 4.935f, -88.5f);

            builder.WithIdentity("carl_bundy", "Carl", "Bundy")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 1.02f;
                    av.Weight = 0.44f;
                    av.SkinColor = new Color(0.7857f, 0.6154f, 0.4774f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.93f, 0.88f, 0.84f);
                    av.PupilDilation = 0.68f;
                    av.EyebrowScale = 1.02f;
                    av.EyebrowThickness = 0.52f;
                    av.EyebrowRestingHeight = -0.06f;
                    av.EyebrowRestingAngle = 0.88f;
                    av.LeftEye = (0.29f, 0.38f);
                    av.RightEye = (0.29f, 0.38f);
                    av.HairColor = new Color(0.34f, 0.22f, 0.14f);
                    av.HairPath = "Avatar/Hair/Franklin/Franklin";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/T-Shirt", new Color(0.42f, 0.40f, 0.38f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/Apron/Apron", new Color(0.38f, 0.26f, 0.16f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.24f, 0.26f, 0.30f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.20f, 0.20f, 0.22f));
                    av.WithAccessoryLayer("Avatar/Accessories/Neck/GoldChain/GoldChain", new Color(0.65f, 0.55f, 0.32f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 750f, maxWeekly: 1200f)
                        .WithOrdersPerWeek(1, 3)
                        .WithPreferredOrderDay(Day.Friday)
                        .WithOrderTime(1130)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.0f, maxAt100: 3.6f)
                        .WithCallPoliceChance(0.16f)
                        .WithDependence(baseAddiction: 0.09f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.36f), (DrugType.Methamphetamine, 0.14f), (DrugType.Shrooms, 0.08f), (DrugType.Cocaine, 0.22f)
                        })
                        .WithPreferredProperties(Property.Munchies, Property.Sedating, Property.Sneaky);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("marcy_bundy", "trey_bundy");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new StayInBuildingSpec { BuildingName = "Carl's House", StartTime = 0650, DurationMinutes = 380 });
                    plan.StayInBuilding(supermarket, 1251, 48);
                    plan.StayInBuilding(cafe, 1340, 55);
                    plan.Add(new StayInBuildingSpec { BuildingName = "Carl's House", StartTime = 1436, DurationMinutes = 259 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "The Crimson Canary", StartTime = 1915, DurationMinutes = 694 });
                });
        }

        public CarlBundy() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.39f;
                Region = Region.Suburbia;
                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"CarlBundy OnCreated failed: {ex.Message}");
            }
        }
    }
}
