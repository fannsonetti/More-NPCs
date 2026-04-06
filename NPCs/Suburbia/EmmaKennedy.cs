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
    /// <summary>Kennedy child (6) — does not buy drugs; strongly dislikes all of them.</summary>
    public sealed class EmmaKennedy : NPC
    {
        public override bool IsPhysical => true;

        private static readonly Color KidHair = new Color(0.62f, 0.52f, 0.30f);
        private static readonly Color KidSkin = new Color(0.82f, 0.65f, 0.50f);

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var supermarket = Building.Get<Supermarket>();
            Vector3 park = new Vector3(70.8f, 4.8614f, -93.8f);
            Vector3 spawnPos = new Vector3(73.0f, 4.935f, -94.5f);

            builder.WithIdentity("emma_kennedy", "Emma", "Kennedy")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 1.0f;
                    av.Height = 0.72f;
                    av.Weight = 0.28f;
                    av.SkinColor = KidSkin;
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.97f, 0.93f, 0.89f);
                    av.PupilDilation = 0.82f;
                    av.EyebrowScale = 0.88f;
                    av.EyebrowThickness = 0.80f;
                    av.EyebrowRestingHeight = 0.06f;
                    av.EyebrowRestingAngle = 2.55f;
                    av.LeftEye = (0.38f, 0.44f);
                    av.RightEye = (0.38f, 0.44f);
                    av.HairColor = KidHair;
                    av.HairPath = "Avatar/Hair/Bun/Bun";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Neutral", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/Tucked T-Shirt", new Color(0.55f, 0.62f, 0.72f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/CollarJacket/CollarJacket", new Color(0.88f, 0.86f, 0.84f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jorts", new Color(0.30f, 0.38f, 0.48f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.88f, 0.86f, 0.84f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 0f, maxWeekly: 0f)
                        .WithOrdersPerWeek(0, 0)
                        .WithPreferredOrderDay(Day.Monday)
                        .WithOrderTime(1200)
                        .WithStandards(CustomerStandard.VeryHigh)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.48f)
                        .WithDependence(baseAddiction: 0f, dependenceMultiplier: 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, -0.92f), (DrugType.Methamphetamine, -0.92f), (DrugType.Shrooms, -0.92f), (DrugType.Cocaine, -0.92f)
                        })
                        .WithPreferredProperties();
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(3.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("tyler_kennedy", "haley_kennedy");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new StayInBuildingSpec { BuildingName = "Kennedy House", StartTime = 2150, DurationMinutes = 520 });
                    plan.UseVendingMachine(0612);
                    plan.Add(new WalkToSpec { Destination = park, StartTime = 0625, FaceDestinationDirection = true });
                    plan.Add(new SitSpec { SeatSetPath = "Map/Hyland Point/Region_Suburbia/Residential park/OutdoorBench", StartTime = 0640, DurationMinutes = 68 });
                    plan.StayInBuilding(supermarket, 0749, 48);
                    plan.Add(new StayInBuildingSpec { BuildingName = "Kennedy House", StartTime = 0838, DurationMinutes = 312 });
                    plan.Add(new WalkToSpec { Destination = park, StartTime = 1231, FaceDestinationDirection = true });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Kennedy House", StartTime = 1305, DurationMinutes = 525 });
                });
        }

        public EmmaKennedy() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.22f;
                Region = Region.Suburbia;
                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"EmmaKennedy OnCreated failed: {ex.Message}");
            }
        }
    }
}
