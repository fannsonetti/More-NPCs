using System;
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
    /// Dorothy Samwell - elderly Northtown resident. Lives above Dan's Hardware,
    /// visits Bud's Bar and Sauerkraut Supreme during the day, relaxes on the outdoor chair.
    /// </summary>
    public sealed class DorothySamwell : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var dansHardwareUpstairs = Building.Get<DansHardwareUpstairs>();
            var budsBar = Building.Get<BudsBar>();
            var sauerkrautSupreme = Building.Get<SauerkrautSupreme>();
            Vector3 outdoorChair = GetOutdoorChairPosition();
            Vector3 spawnPos = outdoorChair;

            builder.WithIdentity("dorothy_samwell", "Dorothy", "Samwell")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 1.0f;
                    av.Height = 0.92f;
                    av.Weight = 0.4f;
                    av.SkinColor = new Color(0.78f, 0.65f, 0.55f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.95f, 0.9f, 0.9f);
                    av.PupilDilation = 0.7f;
                    av.EyebrowScale = 0.95f;
                    av.EyebrowThickness = 0.85f;
                    av.EyebrowRestingHeight = 0.35f;
                    av.EyebrowRestingAngle = 1.2f;
                    av.LeftEye = (0.28f, 0.38f);
                    av.RightEye = (0.28f, 0.38f);
                    av.HairColor = new Color(0.95f, 0.95f, 0.95f);
                    av.HairPath = "Avatar/Hair/lowbun/LowBun";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Neutral", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/OldPersonWrinkles", new Color32(255, 0, 255, 5));
                    av.WithBodyLayer("Avatar/Layers/Top/Tucked T-Shirt", new Color(0.6f, 0.5f, 0.65f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.2f, 0.22f, 0.28f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Flats/Flats", new Color(0.85f, 0.8f, 0.75f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/Blazer/Blazer", new Color(0.45f, 0.35f, 0.5f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 200f, maxWeekly: 400f)
                        .WithOrdersPerWeek(0, 1)
                        .WithPreferredOrderDay(Day.Sunday)
                        .WithOrderTime(1400)
                        .WithStandards(CustomerStandard.Low)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.14f)
                        .WithDependence(baseAddiction: 0f, dependenceMultiplier: 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.2f), (DrugType.Methamphetamine, -0.5f), (DrugType.Shrooms, 0.1f), (DrugType.Cocaine, -0.6f)
                        })
                        .WithPreferredProperties(Property.LongFaced, Property.Refreshing, Property.Focused);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("beth_penn", "peggy_myers", "iris_samwell");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(dansHardwareUpstairs, 0006, 479);
                    plan.StayInBuilding(budsBar, 0806, 239);
                    plan.StayInBuilding(sauerkrautSupreme, 1206, 239);
                    plan.Add(new WalkToSpec { Destination = outdoorChair, StartTime = 1606, FaceDestinationDirection = true });
                    plan.StayInBuilding(dansHardwareUpstairs, 2006, 279);
                });
        }

        private static Vector3 GetOutdoorChairPosition()
        {
            try
            {
                var t = GameObject.Find("Map/Hyland Point/Region_Northtown/Hardware Store/Outdoor chair")?.transform;
                if (t != null) return t.position;
            }
            catch { }
            return new Vector3(-58f, -2.5f, 154f);
        }

        public DorothySamwell() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.39f;
                Region = Region.Northtown;
                Schedule.Enable();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"DorothySamwell OnCreated failed: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
