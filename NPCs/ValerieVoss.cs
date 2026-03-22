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
    /// Valerie Voss - Northtown customer. Connected to Vincent Reeves. White and blue color scheme.
    /// </summary>
    public sealed class ValerieVoss : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var northIndustrial = Building.Get<NorthIndustrialBuilding>();
            var budsBar = Building.Get<BudsBar>();
            var chineseRestaurant = Building.Get<ChineseRestaurant>();
            var northApartments = Building.Get<NorthApartments>();
            Vector3 waterfront = new Vector3(-49.5478f, -4.035f, 168.5777f);
            Vector3 spawnPos = new Vector3(-25.6521f, -3.0349f, 154.5471f);
            builder.WithIdentity("valerie_voss", "Valerie", "Voss")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 1.0f;
                    av.Height = 0.98f;
                    av.Weight = 0.32f;
                    av.SkinColor = new Color(0.684f, 0.554f, 0.445f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = Color.white;
                    av.PupilDilation = 0.75f;
                    av.EyebrowScale = 1.0f;
                    av.EyebrowThickness = 0.9f;
                    av.EyebrowRestingHeight = -0.219f;
                    av.EyebrowRestingAngle = 2.5f;
                    av.LeftEye = (0.30f, 0.39f);
                    av.RightEye = (0.30f, 0.39f);
                    av.HairColor = new Color(0.239f, 0.182f, 0.139f);
                    av.HairPath = "Avatar/Hair/Shoulderlength/ShoulderLength";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/FlannelButtonUp", new Color(0.45f, 0.32f, 0.28f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.22f, 0.22f, 0.24f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(0.18f, 0.18f, 0.18f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/Belt/Belt", new Color(0.32f, 0.24f, 0.16f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/OpenVest/OpenVest", new Color(0.15f, 0.12f, 0.10f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 400f, maxWeekly: 700f)
                        .WithOrdersPerWeek(2, 4)
                        .WithPreferredOrderDay(Day.Friday)
                        .WithOrderTime(1400)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.15f)
                        .WithDependence(baseAddiction: 0.25f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.47f), (DrugType.Methamphetamine, -0.23f), (DrugType.Shrooms, 0.61f), (DrugType.Cocaine, -0.51f)
                        })
                        .WithPreferredProperties(Property.Refreshing, Property.Calming, Property.Glowie);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("vincent_reeves");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new WalkToSpec { Destination = waterfront, StartTime = 720, FaceDestinationDirection = true });
                    plan.Add(new SitSpec { SeatSetPath = "Map/Hyland Point/Region_Northtown/Waterfront/OutdoorBench (1)", StartTime = 800, DurationMinutes = 46 });
                    plan.UseATM(847);
                    plan.StayInBuilding(chineseRestaurant, 913, 89);
                    plan.StayInBuilding(budsBar, 1045, 134);
                    plan.StayInBuilding(northIndustrial, 1220, 174);
                    plan.StayInBuilding(chineseRestaurant, 1435, 99);
                    plan.Add(new WalkToSpec { Destination = spawnPos, StartTime = 1688, FaceDestinationDirection = false });
                    plan.StayInBuilding(northApartments, 1750, 1049);
                });
        }

        public ValerieVoss() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 0.57f;
                Region = S1API.Map.Region.Northtown;

                Schedule.Enable();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"ValerieVoss OnCreated failed: {ex.Message}");
                MelonLogger.Error($"StackTrace: {ex.StackTrace}");
            }
        }
    }
}
