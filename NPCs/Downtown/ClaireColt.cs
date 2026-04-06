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
    /// <summary>Harold Colt's daughter (17). Home at Holt House; unlock requires rapport with vanilla <c>harold_colt</c>.</summary>
    public sealed class ClaireColt : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var cafe = Building.Get<Cafe>();
            var supermarket = Building.Get<Supermarket>();
            var slopShop = Building.Get<SlopShop>();
            Vector3 townBench = new Vector3(69.7895f, 1.065f, 15.4409f);
            Vector3 spawnPos = new Vector3(131.2f, 6.0623f, 113.6f);

            builder.WithIdentity("claire_colt", "Claire", "Colt")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 1.0f;
                    av.Height = 0.96f;
                    av.Weight = 0.34f;
                    av.SkinColor = new Color(0.5441f, 0.3947f, 0.2736f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.94f, 0.90f, 0.86f);
                    av.PupilDilation = 0.73f;
                    av.EyebrowScale = 0.93f;
                    av.EyebrowThickness = 0.80f;
                    av.EyebrowRestingHeight = -0.03f;
                    av.EyebrowRestingAngle = 2.28f;
                    av.LeftEye = (0.32f, 0.40f);
                    av.RightEye = (0.32f, 0.40f);
                    av.HairColor = new Color(0.18f, 0.12f, 0.09f);
                    av.HairPath = "Avatar/Hair/Shoulderlength/ShoulderLength";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/RolledButtonUp", new Color(0.40f, 0.44f, 0.52f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/FemaleUnderwear", new Color(0.92f, 0.90f, 0.88f));
                    av.WithAccessoryLayer("Avatar/Accessories/Bottom/MediumSkirt/MediumSkirt", new Color(0.24f, 0.26f, 0.32f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Flats/Flats", new Color(0.14f, 0.14f, 0.16f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 400f, maxWeekly: 900f)
                        .WithOrdersPerWeek(1, 3)
                        .WithPreferredOrderDay(Day.Thursday)
                        .WithOrderTime(1615)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.0f, maxAt100: 3.5f)
                        .WithCallPoliceChance(0.15f)
                        .WithDependence(baseAddiction: 0.08f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.38f), (DrugType.Methamphetamine, 0.12f), (DrugType.Shrooms, 0.06f), (DrugType.Cocaine, 0.28f)
                        })
                        .WithPreferredProperties(Property.Refreshing, Property.Munchies, Property.Sneaky);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("harold_colt");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.UseVendingMachine(0712);
                    plan.StayInBuilding(cafe, 0730, 66);
                    plan.StayInBuilding(supermarket, 0837, 59);
                    plan.Add(new WalkToSpec { Destination = townBench, StartTime = 1049, FaceDestinationDirection = true });
                    plan.Add(new SitSpec { SeatSetPath = "Map/Hyland Point/Region_Downtown/TownCenter/OutdoorBench", StartTime = 1104, DurationMinutes = 96 });
                    plan.StayInBuilding(slopShop, 1241, 81);
                    plan.Add(new StayInBuildingSpec { BuildingName = "Holt House", StartTime = 1403, DurationMinutes = 287 });
                    plan.StayInBuilding(supermarket, 1851, 54);
                    plan.StayInBuilding(cafe, 1946, 71);
                    plan.Add(new StayInBuildingSpec { BuildingName = "Holt House", StartTime = 2058, DurationMinutes = 142 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Holt House", StartTime = 2321, DurationMinutes = 470 });
                });
        }

        public ClaireColt() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.40f;
                Region = Region.Downtown;
                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"ClaireColt OnCreated failed: {ex.Message}");
            }
        }
    }
}
