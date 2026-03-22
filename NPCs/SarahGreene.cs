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
    /// Sarah Greene - Westville resident in George and Molly's House. Orders once per week Sundays at 12:45. Moderate standards, likes green.
    /// </summary>
    public sealed class SarahGreene : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var arcade = Building.Get<Arcade>();
            var thePissHut = Building.Get<ThePissHut>();
            Vector3 spawnPos = new Vector3(-142f, -2.8f, 72f);
            Vector3 busStop = new Vector3(-13.0495f, 1.065f, 95.5169f);

            builder.WithIdentity("sarah_greene", "Sarah", "Greene")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 1.0f;
                    av.Height = 0.98f;
                    av.Weight = 0.38f;
                    av.SkinColor = new Color(0.62f, 0.52f, 0.42f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.98f, 0.90f, 0.85f);
                    av.PupilDilation = 0.68f;
                    av.EyebrowScale = 1.02f;
                    av.EyebrowThickness = 0.88f;
                    av.EyebrowRestingHeight = -0.16f;
                    av.EyebrowRestingAngle = 1.8f;
                    av.LeftEye = (0.28f, 0.38f);
                    av.RightEye = (0.28f, 0.38f);
                    av.HairColor = new Color(0.20f, 0.15f, 0.11f);
                    av.HairPath = "Avatar/Hair/HighBun/HighBun";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/RolledButtonUp", new Color(0.28f, 0.38f, 0.27f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jorts", new Color(0.27f, 0.30f, 0.36f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sandals/Sandals", new Color(0.21f, 0.34f, 0.25f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/Blazer/Blazer", new Color(0.20f, 0.30f, 0.24f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/SmallRoundGlasses/SmallRoundGlasses", new Color(0.15f, 0.15f, 0.17f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(800f, 2000f)
                        .WithOrdersPerWeek(1, 1)
                        .WithPreferredOrderDay(Day.Sunday)
                        .WithOrderTime(1245)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(0f, 1f)
                        .WithCallPoliceChance(0.06f)
                        .WithDependence(0.10f, 1f)
                        .WithAffinities(new[] { (DrugType.Marijuana, 0.45f), (DrugType.Methamphetamine, 0.12f), (DrugType.Shrooms, 0.22f), (DrugType.Cocaine, 0.18f) })
                        .WithPreferredProperties(Property.Refreshing, Property.Focused, Property.Glowie);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("george_greene");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new StayInBuildingSpec { BuildingName = "George and Molly's House", StartTime = 0, DurationMinutes = 599 });
                    plan.StayInBuilding(arcade, 1030, 119);
                    plan.Add(new WalkToSpec { Destination = busStop, StartTime = 1305, FaceDestinationDirection = true });
                    plan.StayInBuilding(thePissHut, 1435, 89);
                    plan.UseATM(1610);
                    plan.Add(new StayInBuildingSpec { BuildingName = "George and Molly's House", StartTime = 1735, DurationMinutes = 204 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "George and Molly's House", StartTime = 2145, DurationMinutes = 734 });
                });
        }

        public SarahGreene() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.38f;
                Region = Region.Westville;
                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"SarahGreene OnCreated failed: {ex.Message}");
            }
        }
    }
}
