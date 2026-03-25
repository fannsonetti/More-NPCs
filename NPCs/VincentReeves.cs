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
                    av.Height = 1.0f;
                    av.Weight = 0.4f;
                    av.SkinColor = new Color(0.55f, 0.45f, 0.38f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = Color.white;
                    av.HairColor = new Color(0.12f, 0.09f, 0.07f);
                    av.HairPath = "Avatar/Hair/Spiky/Spiky";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Neutral", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/Buttonup", new Color(0.2f, 0.2f, 0.24f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.16f, 0.16f, 0.18f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/DressShoes/DressShoes", new Color(0.15f, 0.15f, 0.15f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/Belt/Belt", new Color(0.28f, 0.22f, 0.16f));
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
                    plan.StayInBuilding(northWarehouse, 800, 299);
                    plan.StayInBuilding(chineseRestaurant, 1100, 219);
                    plan.StayInBuilding(budsBar, 1320, 919);
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

