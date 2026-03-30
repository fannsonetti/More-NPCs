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
                    av.SkinColor = new Color(0.52f, 0.41f, 0.34f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.96f, 0.88f, 0.83f);
                    av.PupilDilation = 0.68f;
                    av.EyebrowScale = 1.08f;
                    av.EyebrowThickness = 0.94f;
                    av.EyebrowRestingHeight = -0.24f;
                    av.EyebrowRestingAngle = 2.6f;
                    av.LeftEye = (0.24f, 0.36f);
                    av.RightEye = (0.30f, 0.36f);
                    av.HairColor = new Color(0.18f, 0.14f, 0.10f);
                    av.HairPath = "Avatar/Hair/FringePonyTail/FringePonyTail";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/Buttonup", new Color(0.30f, 0.52f, 0.36f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.22f, 0.24f, 0.30f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.92f, 0.92f, 0.92f));
                    av.WithAccessoryLayer("Avatar/Accessories/Hands/Polex/Polex", new Color(0.76f, 0.64f, 0.28f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/Belt/Belt", new Color(0.14f, 0.14f, 0.14f));
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
                        .WithCallPoliceChance(0.19f)
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
                    plan.StayInBuilding(arcade, 1007, 119);
                    plan.Add(new SitSpec { SeatSetPath = "Map/Hyland Point/Region_Westville/OutdoorBench", StartTime = 1242, DurationMinutes = 129 });
                    plan.StayInBuilding(thePissHut, 1412, 89);
                    plan.UseATM(1547);
                    plan.Add(new StayInBuildingSpec { BuildingName = "George and Molly's House", StartTime = 1712, DurationMinutes = 984 });
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


