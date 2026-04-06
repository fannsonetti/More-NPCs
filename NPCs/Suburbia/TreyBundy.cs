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
    /// <summary>Carl and Marcy's son — mixed skin tone between both parents.</summary>
    public sealed class TreyBundy : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var supermarket = Building.Get<Supermarket>();
            Vector3 park = new Vector3(71.6f, 4.8614f, -91.5f);
            Vector3 spawnPos = new Vector3(64.0f, 4.935f, -88.2f);

            builder.WithIdentity("trey_bundy", "Trey", "Bundy")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.12f;
                    av.Height = 0.93f;
                    av.Weight = 0.32f;
                    av.SkinColor = new Color(0.54f, 0.41f, 0.31f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.94f, 0.90f, 0.86f);
                    av.PupilDilation = 0.76f;
                    av.EyebrowScale = 0.96f;
                    av.EyebrowThickness = 0.84f;
                    av.EyebrowRestingHeight = -0.02f;
                    av.EyebrowRestingAngle = 1.18f;
                    av.LeftEye = (0.32f, 0.42f);
                    av.RightEye = (0.32f, 0.42f);
                    av.HairColor = new Color(0.22f, 0.16f, 0.12f);
                    av.HairPath = "Avatar/Hair/CloseBuzzCut/CloseBuzzCut";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/Tucked T-Shirt", new Color(0.52f, 0.48f, 0.44f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/OpenVest/OpenVest", new Color(0.30f, 0.34f, 0.40f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.22f, 0.24f, 0.30f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.24f, 0.24f, 0.26f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 50f, maxWeekly: 250f)
                        .WithOrdersPerWeek(2, 4)
                        .WithPreferredOrderDay(Day.Thursday)
                        .WithOrderTime(1530)
                        .WithStandards(CustomerStandard.VeryLow)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.12f)
                        .WithDependence(baseAddiction: 0f, dependenceMultiplier: 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 1f), (DrugType.Methamphetamine, 1f), (DrugType.Shrooms, 1f), (DrugType.Cocaine, 1f)
                        })
                        .WithPreferredProperties();
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(3.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("carl_bundy", "marcy_bundy");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new StayInBuildingSpec { BuildingName = "Carl's House", StartTime = 2242, DurationMinutes = 478 });
                    plan.UseVendingMachine(0642);
                    plan.Add(new WalkToSpec { Destination = park, StartTime = 0655, FaceDestinationDirection = true });
                    plan.Add(new SitSpec { SeatSetPath = "Map/Hyland Point/Region_Suburbia/Residential park/OutdoorBench", StartTime = 0712, DurationMinutes = 74 });
                    plan.StayInBuilding(supermarket, 0827, 54);
                    plan.Add(new StayInBuildingSpec { BuildingName = "Carl's House", StartTime = 0922, DurationMinutes = 319 });
                    plan.Add(new WalkToSpec { Destination = park, StartTime = 1342, FaceDestinationDirection = true });
                    plan.StayInBuilding(supermarket, 1355, 46);
                    plan.Add(new StayInBuildingSpec { BuildingName = "Carl's House", StartTime = 1442, DurationMinutes = 480 });
                });
        }

        public TreyBundy() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.31f;
                Region = Region.Suburbia;
                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"TreyBundy OnCreated failed: {ex.Message}");
            }
        }
    }
}
