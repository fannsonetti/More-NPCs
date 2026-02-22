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
    /// <summary>
    /// Finn Murphy - Docks fisherman. Works at Fish Warehouse, Randy's Bait. Connections: kelly_reynolds, lisa_gardener.
    /// </summary>
    public sealed class FinnMurphy : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var fishWarehouse = Building.Get<FishWarehouse>();
            var randysBait = Building.Get<RandysBaitTackle>();
            var docksIndustrial = Building.Get<DocksIndustrialBuilding>();
            Vector3 warehousePier = new Vector3(-62.4702f, -1.5315f, 19.8399f);
            Vector3 spawnPos = new Vector3(-78.2f, -1.535f, -28.5f);
            builder.WithIdentity("finn_murphy", "Finn", "Murphy")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 0.98f;
                    av.Weight = 0.48f;
                    av.SkinColor = new Color(0.72f, 0.58f, 0.46f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(1.0f, 0.92f, 0.88f);
                    av.PupilDilation = 0.68f;
                    av.EyebrowScale = 1.0f;
                    av.EyebrowThickness = 0.95f;
                    av.EyebrowRestingHeight = -0.18f;
                    av.EyebrowRestingAngle = 1.6f;
                    av.LeftEye = (0.26f, 0.32f);
                    av.RightEye = (0.26f, 0.32f);
                    av.HairColor = new Color(0.12f, 0.09f, 0.06f);
                    av.HairPath = "Avatar/Hair/Spiky/Spiky";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/FlannelButtonUp", new Color(0.72f, 0.62f, 0.48f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.25f, 0.30f, 0.42f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.65f, 0.45f, 0.28f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/Belt/Belt", new Color(0.35f, 0.25f, 0.18f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/BucketHat/BucketHat", new Color(0.42f, 0.48f, 0.36f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 600f, maxWeekly: 900f)
                        .WithOrdersPerWeek(1, 4)
                        .WithPreferredOrderDay(Day.Saturday)
                        .WithOrderTime(1100)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.12f)
                        .WithDependence(baseAddiction: 0.2f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, -0.35f), (DrugType.Methamphetamine, 0.58f), (DrugType.Shrooms, 0.12f), (DrugType.Cocaine, 0.21f)
                        })
                        .WithPreferredProperties(Property.Euphoric, Property.Paranoia, Property.Glowie);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("kelly_reynolds", "lisa_gardener");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(fishWarehouse, 630, 139);
                    plan.StayInBuilding(randysBait, 805, 92);
                    plan.Add(new WalkToSpec { Destination = warehousePier, StartTime = 934, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 180, 0) * Vector3.forward });
                    plan.UseATM(1155);
                    plan.StayInBuilding(docksIndustrial, 1227, 148);
                    plan.UseVendingMachine(1412);
                    plan.StayInBuilding(fishWarehouse, 1442, 198);
                    plan.StayInBuilding(randysBait, 1677, 93);
                    plan.Add(new WalkToSpec { Destination = warehousePier, StartTime = 1807, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 180, 0) * Vector3.forward });
                    plan.StayInBuilding(docksIndustrial, 2045, 583);
                });
        }

        public FinnMurphy() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 4f;
                Region = S1API.Map.Region.Docks;

                Schedule.Enable();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"FinnMurphy OnCreated failed: {ex.Message}");
                MelonLogger.Error($"StackTrace: {ex.StackTrace}");
            }
        }
    }
}
