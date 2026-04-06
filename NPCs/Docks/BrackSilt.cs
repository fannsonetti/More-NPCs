using System;
using MelonLoader;
using S1API.Economy;
using S1API.Entities;
using S1API.Entities.Schedule;
using S1API.GameTime;
using S1API.Map;
using S1API.Products;
using S1API.Properties;
using UnityEngine;

namespace MoreNPCs.NPCs
{
    /// <summary>
    /// Brack Silt — stimmy sewer regular (likes hanging in the tunnels, not a “lives in the grate” archetype).
    /// </summary>
    public sealed class BrackSilt : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            Vector3 pit = new Vector3(44.4508f, -8.035f, 36.1825f);
            Vector3 sewerBalcony = new Vector3(77.1413f, -4.535f, 34.6874f);
            Vector3 sewerWarehouse = new Vector3(74.0413f, -4.535f, 28.6874f);
            Vector3 roundRoomChill = new Vector3(39.75f, -8.035f, 40.75f);
            Vector3 crossroad = new Vector3(97.4504f, 1.065f, 7.0207f);
            Vector3 sewerCrossSection = new Vector3(83.0061f, -5.785f, 5.8099f);
            Vector3 spawnPos = new Vector3(96.2f, 1.065f, 8.4f);

            builder.WithIdentity("brack_silt", "Brack", "Silt")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0f;
                    av.Height = 0.93f;
                    av.Weight = 0.29f;
                    av.SkinColor = new Color(0.58f, 0.5f, 0.45f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(1f, 0.78f, 0.78f);
                    av.PupilDilation = 0.88f;
                    av.EyebrowScale = 0.92f;
                    av.EyebrowThickness = 0.55f;
                    av.EyebrowRestingHeight = -0.52f;
                    av.EyebrowRestingAngle = -3.4f;
                    av.LeftEye = (0.35f, 0.47f);
                    av.RightEye = (0.35f, 0.47f);
                    av.HairColor = new Color(0.12f, 0.11f, 0.1f);
                    av.HairPath = "Avatar/Hair/Spiky/Spiky";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Agitated", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Stubble", new Color(0.16f, 0.14f, 0.12f));
                    av.WithBodyLayer("Avatar/Layers/Top/Tucked T-Shirt", new Color(0.62f, 0.6f, 0.56f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.11f, 0.11f, 0.12f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/OpenVest/OpenVest", new Color(0.22f, 0.2f, 0.18f));
                    av.WithBodyLayer("Avatar/Layers/Accessories/FingerlessGloves", new Color(0.12f, 0.11f, 0.1f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(0.28f, 0.26f, 0.24f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 520f, maxWeekly: 780f)
                        .WithOrdersPerWeek(1, 5)
                        .WithPreferredOrderDay(Day.Saturday)
                        .WithOrderTime(0230)
                        .WithStandards(CustomerStandard.VeryLow)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.15f)
                        .WithDependence(baseAddiction: 0.45f, dependenceMultiplier: 1.15f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, -0.15f), (DrugType.Methamphetamine, 0.82f), (DrugType.Shrooms, -0.4f), (DrugType.Cocaine, 0.68f)
                        })
                        .WithPreferredProperties(Property.Energizing, Property.Paranoia, Property.Explosive);
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
                    plan.Add(new WalkToSpec { Destination = sewerCrossSection, StartTime = 0248, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = sewerWarehouse, StartTime = 0506, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = pit, StartTime = 0733, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = sewerBalcony, StartTime = 0951, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = crossroad, StartTime = 1218, FaceDestinationDirection = false });
                    plan.Add(new WalkToSpec { Destination = roundRoomChill, StartTime = 1442, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 180, 0) * Vector3.forward });
                    plan.Add(new WalkToSpec { Destination = sewerBalcony, StartTime = 1715, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = sewerWarehouse, StartTime = 1938, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = pit, StartTime = 2204, FaceDestinationDirection = true });
                });
        }

        public BrackSilt() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 0.78f;
                Region = Region.Docks;

                Schedule.Enable();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"BrackSilt OnCreated failed: {ex.Message}");
                MelonLogger.Error($"StackTrace: {ex.StackTrace}");
            }
        }
    }
}
