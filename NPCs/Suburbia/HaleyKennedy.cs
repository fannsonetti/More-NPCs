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
    /// <summary>Kennedy child (10) — blonde family look, park and errands after school.</summary>
    public sealed class HaleyKennedy : NPC
    {
        public override bool IsPhysical => true;

        private static readonly Color KidHair = new Color(0.612f, 0.51f, 0.29f);
        private static readonly Color KidSkin = new Color(0.81f, 0.648f, 0.496f);

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var supermarket = Building.Get<Supermarket>();
            var cafe = Building.Get<Cafe>();
            Vector3 park = new Vector3(71.4f, 4.8614f, -92.6f);
            Vector3 spawnPos = new Vector3(72.5f, 4.935f, -95.6f);

            builder.WithIdentity("haley_kennedy", "Haley", "Kennedy")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.95f;
                    av.Height = 0.80f;
                    av.Weight = 0.30f;
                    av.SkinColor = KidSkin;
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.96f, 0.92f, 0.88f);
                    av.PupilDilation = 0.78f;
                    av.EyebrowScale = 0.90f;
                    av.EyebrowThickness = 0.82f;
                    av.EyebrowRestingHeight = 0.02f;
                    av.EyebrowRestingAngle = 2.42f;
                    av.LeftEye = (0.34f, 0.42f);
                    av.RightEye = (0.34f, 0.42f);
                    av.HairColor = KidHair;
                    av.HairPath = "Avatar/Hair/HighBun/HighBun";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/Freckles", new Color(0f, 0f, 0f, 0.35f));
                    av.WithBodyLayer("Avatar/Layers/Top/Tucked T-Shirt", new Color(0.72f, 0.38f, 0.42f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/FemaleUnderwear", new Color(0.94f, 0.92f, 0.90f));
                    av.WithAccessoryLayer("Avatar/Accessories/Bottom/MediumSkirt/MediumSkirt", new Color(0.32f, 0.36f, 0.44f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sandals/Sandals", new Color(0.42f, 0.32f, 0.24f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 50f, maxWeekly: 250f)
                        .WithOrdersPerWeek(2, 4)
                        .WithPreferredOrderDay(Day.Tuesday)
                        .WithOrderTime(1400)
                        .WithStandards(CustomerStandard.VeryLow)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.11f)
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
                        .WithConnectionsById("tyler_kennedy", "emma_kennedy");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.UseVendingMachine(0703);
                    plan.Add(new WalkToSpec { Destination = park, StartTime = 0718, FaceDestinationDirection = true });
                    plan.Add(new SitSpec { SeatSetPath = "Map/Hyland Point/Region_Suburbia/Residential park/OutdoorBench (1)", StartTime = 0733, DurationMinutes = 72 });
                    plan.StayInBuilding(supermarket, 0846, 62);
                    plan.StayInBuilding(cafe, 0949, 68);
                    plan.Add(new WalkToSpec { Destination = park, StartTime = 1058, FaceDestinationDirection = true });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Kennedy House", StartTime = 1110, DurationMinutes = 481 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Kennedy House", StartTime = 1912, DurationMinutes = 710 });
                });
        }

        public HaleyKennedy() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.28f;
                Region = Region.Suburbia;
                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"HaleyKennedy OnCreated failed: {ex.Message}");
            }
        }
    }
}
