using MelonLoader;
using S1API.Economy;
using S1API.Entities;
using S1API.Entities.NPCs.Northtown;
using S1API.GameTime;
using S1API.Map;
using S1API.Map.Buildings;
using S1API.Products;
using S1API.Properties;
using UnityEngine;

namespace MoreNPCs.NPCs
{
    /// <summary>
    /// Jian Ming - Chinese, last name Ming. White/blue scheme. Main: North Industrial, secondary: North Warehouse. Chinese Restaurant 90min at dinner. Connected to Ludwig Meyer, Mrs. Ming, Mr. Ming.
    /// </summary>
    public sealed class JianMing : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var northIndustrial = Building.Get<NorthIndustrialBuilding>();
            var northWarehouse = Building.Get<NorthWarehouse>();
            var chineseRestaurant = Building.Get<ChineseRestaurant>();
            Vector3 spawnPos = new Vector3(-37.1751f, -3.035f, 175.8911f);
            builder.WithIdentity("jian_ming", "Jian", "Ming")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 0.97f;
                    av.Weight = 0.38f;
                    av.SkinColor = new Color(0.867f, 0.7369f, 0.5623f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(1.0f, 0.95f, 0.9f);
                    av.PupilDilation = 0.7f;
                    av.EyebrowScale = 1.05f;
                    av.EyebrowThickness = 1.1f;
                    av.EyebrowRestingHeight = -0.25f;
                    av.EyebrowRestingAngle = 2.2f;
                    av.LeftEye = (0.20f, 0.28f);
                    av.RightEye = (0.20f, 0.28f);
                    av.HairColor = new Color(0.24f, 0.21f, 0.18f);
                    av.HairPath = "Avatar/Hair/Spiky/Spiky";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Neutral", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/RolledButtonUp", new Color(0.92f, 0.88f, 0.82f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.18f, 0.16f, 0.14f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sandals/Sandals", new Color(0.32f, 0.26f, 0.20f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/Belt/Belt", new Color(0.28f, 0.22f, 0.16f));
                    av.WithAccessoryLayer("Avatar/Accessories/Neck/GoldChain/GoldChain", new Color(0.85f, 0.72f, 0.35f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 300f, maxWeekly: 600f)
                        .WithOrdersPerWeek(2, 4)
                        .WithPreferredOrderDay(Day.Thursday)
                        .WithOrderTime(1600)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.12f)
                        .WithDependence(baseAddiction: 0.25f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.33f), (DrugType.Methamphetamine, -0.58f), (DrugType.Shrooms, 0.41f), (DrugType.Cocaine, -0.22f)
                        })
                        .WithPreferredProperties(Property.Calming, Property.Refreshing, Property.ThoughtProvoking);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("ludwig_meyer", "ming", "mr_ming", "fannsonetti");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(northIndustrial, 720, 232);
                    plan.StayInBuilding(chineseRestaurant, 953, 91);
                    plan.UseVendingMachine(1045);
                    plan.StayInBuilding(northIndustrial, 1105, 217);
                    plan.StayInBuilding(chineseRestaurant, 1323, 109);
                    plan.StayInBuilding(chineseRestaurant, 1433, 806); // overnight at Chinese Restaurant
                });
        }

        public JianMing() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 0.58f;
                Region = S1API.Map.Region.Northtown;

                Schedule.Enable();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"JianMing OnCreated failed: {ex.Message}");
                MelonLogger.Error($"StackTrace: {ex.StackTrace}");
            }
        }
    }
}
