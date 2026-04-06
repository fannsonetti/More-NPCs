using MelonLoader;
using S1API.Economy;
using S1API.Entities;
using S1API.Entities.NPCs.Northtown;
using S1API.Entities.Schedule;
using S1API.GameTime;
using S1API.Map;
using S1API.Map.Buildings;
using S1API.Products;
using S1API.Properties;
using UnityEngine;

namespace MoreNPCs.NPCs
{
    public sealed class BenDover : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var motelOffice = Building.Get<MotelOffice>();
            var budsBar = Building.Get<BudsBar>();
            var northWarehouse = Building.Get<NorthWarehouse>();
            var chineseRestaurant = Building.Get<ChineseRestaurant>();
            Vector3 spawnPos = new Vector3(-56.2741f, -4.035f, 162.1295f);
            Vector3 northCorner = new Vector3(-74.2816f, -1.535f, 47.1063f);

            builder.WithIdentity("ben_dover", "Ben", "Dover")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.06f;
                    av.Height = 0.97f;
                    av.Weight = 0.36f;
                    av.SkinColor = new Color(0.32f, 0.24f, 0.20f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.90f, 0.86f, 0.82f);
                    av.PupilDilation = 0.54f;
                    av.EyebrowScale = 1.03f;
                    av.EyebrowThickness = 0.94f;
                    av.EyebrowRestingHeight = -0.18f;
                    av.EyebrowRestingAngle = 0.8f;
                    av.LeftEye = (0.31f, 0.39f);
                    av.RightEye = (0.31f, 0.39f);
                    av.HairColor = new Color(0.08f, 0.07f, 0.07f);
                    av.HairPath = "Avatar/Hair/Curtains/Curtains";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Neutral", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/Buttonup", new Color(0.36f, 0.32f, 0.28f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.22f, 0.24f, 0.26f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.14f, 0.14f, 0.16f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/Belt/Belt", new Color(0.15f, 0.15f, 0.15f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 350f, maxWeekly: 650f)
                        .WithOrdersPerWeek(2, 4)
                        .WithPreferredOrderDay(Day.Wednesday)
                        .WithOrderTime(1940)
                        .WithStandards(CustomerStandard.Low)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.0f, maxAt100: 3.5f)
                        .WithCallPoliceChance(0.16f)
                        .WithDependence(baseAddiction: 0.20f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.21f), (DrugType.Methamphetamine, 0.35f), (DrugType.Shrooms, -0.14f), (DrugType.Cocaine, 0.10f)
                        })
                        .WithPreferredProperties(Property.Jennerising, Property.Refreshing, Property.Smelly);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("tess_tickle", "wayne_kerr");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.UseVendingMachine(0926);
                    plan.StayInBuilding(chineseRestaurant, 1016, 99);
                    plan.Add(new WalkToSpec { Destination = northCorner, StartTime = 1216, FaceDestinationDirection = true });
                    plan.StayInBuilding(motelOffice, 1436, 99);
                    plan.UseATM(1701);
                    plan.StayInBuilding(budsBar, 1801, 194);
                    plan.StayInBuilding(northWarehouse, 2216, 289);
                });
        }

        public BenDover() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 0.58f;
                Region = Region.Northtown;

                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"BenDover OnCreated failed: {ex.Message}");
            }
        }
    }
}

