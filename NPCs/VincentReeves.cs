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
    /// Vincent Reeves - Northtown customer. Connected to Jian Ming.
    /// </summary>
    public sealed class VincentReeves : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var northWarehouse = Building.Get<NorthWarehouse>();
            var chineseRestaurant = Building.Get<ChineseRestaurant>();
            var budsBar = Building.Get<BudsBar>();
            Vector3 spawnPos = new Vector3(-41.7551f, -2.9354f, 171.8678f);
            builder.WithIdentity("vincent_reeves", "Vincent", "Reeves")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 1.00f;
                    av.Weight = 0.46f;
                    av.SkinColor = new Color(0.30f, 0.23f, 0.19f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.92f, 0.86f, 0.82f);
                    av.PupilDilation = 0.48f;
                    av.EyebrowScale = 1.08f;
                    av.EyebrowThickness = 1.12f;
                    av.EyebrowRestingHeight = -0.28f;
                    av.EyebrowRestingAngle = 1.4f;
                    av.LeftEye = (0.28f, 0.36f);
                    av.RightEye = (0.28f, 0.36f);
                    av.HairColor = new Color(0.10f, 0.09f, 0.09f);
                    av.HairPath = "Avatar/Hair/CloseBuzzCut/CloseBuzzCut";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Neutral", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Stubble", new Color(0.06f, 0.05f, 0.05f));
                    av.WithBodyLayer("Avatar/Layers/Top/Tucked T-Shirt", new Color(0.38f, 0.32f, 0.28f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.20f, 0.22f, 0.28f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/CollarJacket/CollarJacket", new Color(0.24f, 0.22f, 0.20f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(0.14f, 0.14f, 0.15f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/FlatCap/FlatCap", new Color(0.22f, 0.20f, 0.18f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(400f, 700f)
                        .WithOrdersPerWeek(2, 4)
                        .WithPreferredOrderDay(Day.Saturday)
                        .WithOrderTime(2000)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(2.5f, 4.0f)
                        .WithCallPoliceChance(0.21f)
                        .WithDependence(0.25f, 1f)
                        .WithAffinities(new[] { (DrugType.Marijuana, -0.2f), (DrugType.Methamphetamine, 0.5f), (DrugType.Shrooms, -0.3f), (DrugType.Cocaine, 0.5f) })
                        .WithPreferredProperties(Property.Euphoric, Property.Glowie, Property.Energizing);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("jian_ming");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(northWarehouse, 0808, 299);
                    plan.StayInBuilding(chineseRestaurant, 1108, 219);
                    plan.StayInBuilding(budsBar, 1328, 919);
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

                Aggressiveness = 0.61f;
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

