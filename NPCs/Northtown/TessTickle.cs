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
    /// <summary>
    /// Tess Tickle - flirty Northtown barfly who knows Peter File and Wayne Kerr.
    /// Connected to Peter File and Wayne Kerr.
    /// </summary>
    public sealed class TessTickle : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var motelOffice = Building.Get<MotelOffice>();
            var budsBar = Building.Get<BudsBar>();
            var northWarehouse = Building.Get<NorthWarehouse>();
            var chineseRestaurant = Building.Get<ChineseRestaurant>();
            Vector3 spawnPos = new Vector3(-76.0633f, -1.535f, 44.6816f);
            Vector3 northWalk = new Vector3(-49.5478f, -4.035f, 168.5777f);

            builder.WithIdentity("tess_tickle", "Tess", "Tickle")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.86f;
                    av.Height = 1.01f;
                    av.Weight = 0.37f;
                    av.SkinColor = new Color(0.71f, 0.58f, 0.49f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.99f, 0.92f, 0.88f);
                    av.PupilDilation = 0.73f;
                    av.EyebrowScale = 0.96f;
                    av.EyebrowThickness = 0.86f;
                    av.EyebrowRestingHeight = -0.04f;
                    av.EyebrowRestingAngle = 2.8f;
                    av.LeftEye = (0.31f, 0.42f);
                    av.RightEye = (0.31f, 0.42f);
                    av.HairColor = new Color(0.29f, 0.18f, 0.12f);
                    av.HairPath = "Avatar/Hair/HighBun/HighBun";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/V-Neck", new Color(0.46f, 0.34f, 0.29f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.21f, 0.23f, 0.34f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/DressShoes/DressShoes", new Color(0.15f, 0.15f, 0.17f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/CollarJacket/CollarJacket", new Color(0.24f, 0.26f, 0.31f));
                    av.WithAccessoryLayer("Avatar/Accessories/Neck/GoldChain/GoldChain", new Color(0.80f, 0.69f, 0.34f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 400f, maxWeekly: 700f)
                        .WithOrdersPerWeek(2, 4)
                        .WithPreferredOrderDay(Day.Friday)
                        .WithOrderTime(2100)
                        .WithStandards(CustomerStandard.Low)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.15f)
                        .WithDependence(baseAddiction: 0.16f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.12f), (DrugType.Methamphetamine, 0.09f), (DrugType.Shrooms, 0.57f), (DrugType.Cocaine, 0.34f)
                        })
                        .WithPreferredProperties(Property.Euphoric, Property.Glowie, Property.Refreshing);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("peter_file", "wayne_kerr");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.UseVendingMachine(0927);
                    plan.StayInBuilding(chineseRestaurant, 1022, 111);
                    plan.Add(new WalkToSpec { Destination = northWalk, StartTime = 1247, FaceDestinationDirection = true });
                    plan.StayInBuilding(motelOffice, 1507, 91);
                    plan.UseATM(1647);
                    plan.StayInBuilding(budsBar, 1747, 196);
                    plan.StayInBuilding(northWarehouse, 2222, 229);
                });
        }

        public TessTickle() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 0.56f;
                Region = Region.Northtown;

                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"TessTickle OnCreated failed: {ex.Message}");
            }
        }
    }
}

