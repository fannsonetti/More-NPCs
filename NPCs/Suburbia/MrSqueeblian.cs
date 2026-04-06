using MelonLoader;
using S1API.Economy;
using S1API.Entities;
using S1API.Entities.Schedule;
using S1API.GameTime;
using S1API.Products;
using S1API.Properties;
using UnityEngine;

namespace MoreNPCs.NPCs
{
    public sealed class MrSqueeblian : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            Vector3 residentialBench = new Vector3(71.2f, 4.8614f, -93.4f);
            Vector3 longHouseWalk = new Vector3(64.0f, 4.935f, -86.0f);
            Vector3 spawnPos = new Vector3(67.8f, 4.935f, -91.6f);

            builder.WithIdentity("mr_squeeblian", "Mr. Squeeblian", "")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.08f;
                    av.Height = 0.99f;
                    av.Weight = 0.38f;
                    av.SkinColor = new Color(0.62f, 0.48f, 0.40f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.96f, 0.94f, 0.90f);
                    av.PupilDilation = 0.68f;
                    av.EyebrowScale = 1.12f;
                    av.EyebrowThickness = 0.88f;
                    av.EyebrowRestingHeight = -0.02f;
                    av.EyebrowRestingAngle = 0.95f;
                    av.LeftEye = (0.30f, 0.40f);
                    av.RightEye = (0.30f, 0.40f);
                    av.HairColor = new Color(0.18f, 0.14f, 0.22f);
                    av.HairPath = "Avatar/Hair/Tony/Tony";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_OpenMouthSmile", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/RolledButtonUp", new Color(0.42f, 0.52f, 0.38f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/TailoredPants", new Color(0.28f, 0.26f, 0.32f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/DressShoes/DressShoes", new Color(0.22f, 0.20f, 0.18f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/MushroomHat/MushroomHat", new Color(0.55f, 0.38f, 0.32f));
                    av.WithAccessoryLayer("Avatar/Accessories/Neck/GoldChain/GoldChain", new Color(0.78f, 0.70f, 0.38f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 500f, maxWeekly: 900f)
                        .WithOrdersPerWeek(1, 2)
                        .WithPreferredOrderDay(Day.Thursday)
                        .WithOrderTime(1100)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 1.8f, maxAt100: 3.2f)
                        .WithCallPoliceChance(0.22f)
                        .WithDependence(baseAddiction: 0f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.55f), (DrugType.Methamphetamine, -0.15f), (DrugType.Shrooms, 0.62f), (DrugType.Cocaine, 0.08f)
                        })
                        .WithPreferredProperties(Property.Munchies, Property.ThoughtProvoking, Property.Sneaky);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new WalkToSpec { Destination = residentialBench, StartTime = 0718, FaceDestinationDirection = true });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Supermarket", StartTime = 0842, DurationMinutes = 101 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Cafe", StartTime = 1043, DurationMinutes = 94 });
                    plan.Add(new WalkToSpec { Destination = longHouseWalk, StartTime = 1208, FaceDestinationDirection = true });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Small Tower", StartTime = 1321, DurationMinutes = 289 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Cafe", StartTime = 1811, DurationMinutes = 87 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Green House", StartTime = 1940, DurationMinutes = 655 });
                });
        }

        public MrSqueeblian() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.45f;
                Region = S1API.Map.Region.Suburbia;
                Schedule.Enable();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"MrSqueeblian OnCreated failed: {ex.Message}");
                MelonLogger.Error($"StackTrace: {ex.StackTrace}");
            }
        }
    }
}
