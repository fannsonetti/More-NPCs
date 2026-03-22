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
    /// Marla Hale - Northtown regular who bounces between Chinese Restaurant, Bud's Bar, and Kyle and Austin's place.
    /// Connected to Kyle Cooley and Nico Marlowe.
    /// </summary>
    public sealed class MarlaHale : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var chineseRestaurant = Building.Get<ChineseRestaurant>();
            var budsBar = Building.Get<BudsBar>();
            var northIndustrial = Building.Get<NorthIndustrialBuilding>();
            Vector3 spawnPos = new Vector3(-38.442f, -2.935f, 170.228f);
            Vector3 northStreet = new Vector3(-71.2847f, -2.935f, 145.6754f);

            builder.WithIdentity("marla_hale", "Marla", "Hale")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.88f;
                    av.Height = 0.97f;
                    av.Weight = 0.34f;
                    av.SkinColor = new Color(0.68f, 0.56f, 0.47f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.98f, 0.90f, 0.86f);
                    av.PupilDilation = 0.72f;
                    av.EyebrowScale = 0.94f;
                    av.EyebrowThickness = 0.82f;
                    av.EyebrowRestingHeight = -0.08f;
                    av.EyebrowRestingAngle = 1.4f;
                    av.LeftEye = (0.28f, 0.40f);
                    av.RightEye = (0.28f, 0.40f);
                    av.HairColor = new Color(0.42f, 0.30f, 0.22f);
                    av.HairPath = "Avatar/Hair/MessyBob/MessyBob";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/RolledButtonUp", new Color(0.39f, 0.47f, 0.41f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.29f, 0.31f, 0.27f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Flats/Flats", new Color(0.20f, 0.20f, 0.22f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/CollarJacket/CollarJacket", new Color(0.24f, 0.34f, 0.36f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/RectangleFrameGlasses/RectangleFrameGlasses", new Color(0.16f, 0.16f, 0.18f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 300f, maxWeekly: 600f)
                        .WithOrdersPerWeek(2, 4)
                        .WithPreferredOrderDay(Day.Thursday)
                        .WithOrderTime(1945)
                        .WithStandards(CustomerStandard.VeryLow)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.08f)
                        .WithDependence(baseAddiction: 0.18f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.18f), (DrugType.Methamphetamine, 0.71f), (DrugType.Shrooms, -0.22f), (DrugType.Cocaine, 0.24f)
                        })
                        .WithPreferredProperties(Property.Energizing, Property.Euphoric, Property.Foggy);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("kyle_cooley", "nico_marlowe");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(northIndustrial, 0, 430); // overnight at North Industrial Building
                    plan.StayInBuilding(chineseRestaurant, 830, 99);
                    plan.UseATM(1005);
                    plan.StayInBuilding(budsBar, 1100, 164);
                    plan.Add(new WalkToSpec { Destination = northStreet, StartTime = 1340, FaceDestinationDirection = true });
                    plan.UseVendingMachine(1545);
                    plan.StayInBuilding(northIndustrial, 1615, 109);
                    plan.StayInBuilding(northIndustrial, 2140, 299); // overnight at North Industrial Building
                });
        }

        public MarlaHale() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 0.48f;
                Region = Region.Northtown;

                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"MarlaHale OnCreated failed: {ex.Message}");
            }
        }
    }
}
