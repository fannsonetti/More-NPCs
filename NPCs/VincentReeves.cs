using MelonLoader;
using S1API.Economy;
using S1API.Entities;
using S1API.Entities.Schedule;
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
    /// Vincent Reeves - Northtown customer, FannsoNetti gang. White/blue scheme, male. Main: North Industrial, secondary: North Warehouse. Variation: Nightclub, Slop Shop.
    /// </summary>
    public sealed class VincentReeves : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var northIndustrial = Building.Get<NorthIndustrialBuilding>();
            var northWarehouse = Building.Get<NorthWarehouse>();
            var nightclub = Building.Get<Nightclub>();
            var slopShop = Building.Get<SlopShop>();
            var chineseRestaurant = Building.Get<ChineseRestaurant>();
            var northApartments = Building.Get<NorthApartments>();
            Vector3 spawnPos = new Vector3(-41.7551f, -2.9354f, 171.8678f);
            builder.WithIdentity("vincent_reeves", "Vincent", "Reeves")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 1.02f;
                    av.Weight = 0.42f;
                    av.SkinColor = new Color(0.52f, 0.42f, 0.35f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(1.0f, 0.8f, 0.8f);
                    av.PupilDilation = 0.75f;
                    av.EyebrowScale = 1.1f;
                    av.EyebrowThickness = 1.2f;
                    av.EyebrowRestingHeight = -0.28f;
                    av.EyebrowRestingAngle = 2.2f;
                    av.LeftEye = (0.28f, 0.38f);
                    av.RightEye = (0.28f, 0.38f);
                    av.HairColor = new Color(0.11f, 0.09f, 0.07f);
                    av.HairPath = "Avatar/Hair/Spiky/Spiky";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Neutral", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Stubble", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/RolledButtonUp", new Color(0.95f, 0.95f, 0.95f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.178f, 0.217f, 0.406f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(1.0f, 1.0f, 1.0f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/Belt/Belt", new Color(0.151f, 0.151f, 0.151f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/CollarJacket/CollarJacket", new Color(0.178f, 0.217f, 0.406f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 400f, maxWeekly: 800f)
                        .WithOrdersPerWeek(2, 4)
                        .WithPreferredOrderDay(Day.Saturday)
                        .WithOrderTime(2200)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.08f)
                        .WithDependence(baseAddiction: 0.25f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, -0.17f), (DrugType.Methamphetamine, 0.52f), (DrugType.Shrooms, -0.41f), (DrugType.Cocaine, 0.68f)
                        })
                        .WithPreferredProperties(Property.Euphoric, Property.Glowie, Property.Cyclopean);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("fannsonetti");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(slopShop, 823, 95);
                    plan.StayInBuilding(northIndustrial, 945, 135);
                    plan.StayInBuilding(chineseRestaurant, 1112, 89);
                    plan.StayInBuilding(northWarehouse, 1233, 148);
                    plan.StayInBuilding(nightclub, 1412, 112);
                    plan.StayInBuilding(northIndustrial, 1555, 122);
                    plan.StayInBuilding(chineseRestaurant, 1708, 89);
                    plan.StayInBuilding(northApartments, 1828, 611);
                });
        }

        public VincentReeves() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 5f;
                Region = S1API.Map.Region.Northtown;

                Schedule.Enable();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"VincentReeves OnCreated failed: {ex.Message}");
                MelonLogger.Error($"StackTrace: {ex.StackTrace}");
            }
        }
    }
}
