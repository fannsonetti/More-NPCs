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
    /// <summary>Carl's wife — Carl's House anchor with errands while Carl works nights.</summary>
    public sealed class MarcyBundy : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var supermarket = Building.Get<Supermarket>();
            var cafe = Building.Get<Cafe>();
            Vector3 park = new Vector3(70.2f, 4.8614f, -92.1f);
            Vector3 spawnPos = new Vector3(63.8f, 4.935f, -89.0f);

            builder.WithIdentity("marcy_bundy", "Marcy", "Bundy")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 1.0f;
                    av.Height = 0.99f;
                    av.Weight = 0.38f;
                    av.SkinColor = new Color(0.34f, 0.24f, 0.17f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.92f, 0.88f, 0.84f);
                    av.PupilDilation = 0.70f;
                    av.EyebrowScale = 0.94f;
                    av.EyebrowThickness = 0.72f;
                    av.EyebrowRestingHeight = -0.04f;
                    av.EyebrowRestingAngle = 2.22f;
                    av.LeftEye = (0.33f, 0.40f);
                    av.RightEye = (0.33f, 0.40f);
                    av.HairColor = new Color(0.08f, 0.06f, 0.06f);
                    av.HairPath = "Avatar/Hair/Bun/Bun";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Neutral", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/RolledButtonUp", new Color(0.34f, 0.38f, 0.48f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/FemaleUnderwear", new Color(0.94f, 0.92f, 0.90f));
                    av.WithAccessoryLayer("Avatar/Accessories/Bottom/MediumSkirt/MediumSkirt", new Color(0.26f, 0.28f, 0.32f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Flats/Flats", new Color(0.12f, 0.12f, 0.14f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 800f, maxWeekly: 1300f)
                        .WithOrdersPerWeek(1, 3)
                        .WithPreferredOrderDay(Day.Tuesday)
                        .WithOrderTime(1500)
                        .WithStandards(CustomerStandard.High)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.1f, maxAt100: 3.7f)
                        .WithCallPoliceChance(0.20f)
                        .WithDependence(baseAddiction: 0.07f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.18f), (DrugType.Methamphetamine, -0.12f), (DrugType.Shrooms, 0.24f), (DrugType.Cocaine, 0.31f)
                        })
                        .WithPreferredProperties(Property.Refreshing, Property.Glowie, Property.Munchies);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("carl_bundy", "trey_bundy");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new StayInBuildingSpec { BuildingName = "Carl's House", StartTime = 0618, DurationMinutes = 312 });
                    plan.StayInBuilding(supermarket, 0931, 67);
                    plan.StayInBuilding(cafe, 1039, 71);
                    plan.Add(new WalkToSpec { Destination = park, StartTime = 1151, FaceDestinationDirection = true });
                    plan.Add(new SitSpec { SeatSetPath = "Map/Hyland Point/Region_Suburbia/Residential park/OutdoorBench (1)", StartTime = 1210, DurationMinutes = 84 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Carl's House", StartTime = 1335, DurationMinutes = 268 });
                    plan.StayInBuilding(supermarket, 1644, 51);
                    plan.Add(new StayInBuildingSpec { BuildingName = "Carl's House", StartTime = 1736, DurationMinutes = 307 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Carl's House", StartTime = 2244, DurationMinutes = 473 });
                });
        }

        public MarcyBundy() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.42f;
                Region = Region.Suburbia;
                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"MarcyBundy OnCreated failed: {ex.Message}");
            }
        }
    }
}
