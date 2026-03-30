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
                    av.Height = 0.98f;
                    av.Weight = 0.4f;
                    av.SkinColor = new Color(0.63f, 0.52f, 0.43f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(1.0f, 0.95f, 0.9f);
                    av.PupilDilation = 0.4f;
                    av.EyebrowScale = 1.15f;
                    av.EyebrowThickness = 1.23f;
                    av.EyebrowRestingHeight = -0.44f;
                    av.EyebrowRestingAngle = -5.2f;
                    av.LeftEye = (0.15f, 0.24f);
                    av.RightEye = (0.15f, 0.24f);
                    av.HairColor = new Color(0.18f, 0.14f, 0.10f);
                    av.HairPath = "Avatar/Hair/Spiky/Spiky";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Neutral", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Stubble", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/TiredEyes", new Color(0f, 0f, 0f, 0.55f));
                    av.WithBodyLayer("Avatar/Layers/Top/Buttonup", new Color(0.2f, 0.2f, 0.24f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.16f, 0.16f, 0.18f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.15f, 0.15f, 0.15f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/OpenVest/OpenVest", new Color(0.15f, 0.15f, 0.15f));
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

