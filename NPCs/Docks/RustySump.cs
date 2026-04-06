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
    /// Rusty Sump — docks weirdo / scavenger (not hazmat fugitive, not Mike’s homeless sewer act).
    /// Unlocks through the extra dock customers tied to Jane / Mack / Diesel / Mike / Anna.
    /// </summary>
    public sealed class RustySump : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            Vector3 pit = new Vector3(44.4508f, -8.035f, 36.1825f);
            Vector3 sewerBalcony = new Vector3(77.1413f, -4.535f, 34.6874f);
            Vector3 sewerWarehouse = new Vector3(74.0413f, -4.535f, 28.6874f);
            Vector3 underMotelChill = new Vector3(-52.25f, -6.535f, 92.5f);
            Vector3 sewerStorageChill = new Vector3(36.7f, -8.035f, 76.7f);
            Vector3 crossroad = new Vector3(97.4504f, 1.065f, 7.0207f);
            Vector3 sewerCrossSection = new Vector3(83.0061f, -5.785f, 5.8099f);
            Vector3 spawnPos = new Vector3(-10.2f, 1.065f, 65.9f);

            builder.WithIdentity("rusty_sump", "Rusty", "Sump")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0f;
                    av.Height = 1.02f;
                    av.Weight = 0.48f;
                    av.SkinColor = new Color(0.62f, 0.52f, 0.44f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.95f, 0.9f, 0.82f);
                    av.PupilDilation = 0.62f;
                    av.EyebrowScale = 1.18f;
                    av.EyebrowThickness = 0.72f;
                    av.EyebrowRestingHeight = -0.28f;
                    av.EyebrowRestingAngle = 6.5f;
                    av.LeftEye = (0.29f, 0.41f);
                    av.RightEye = (0.29f, 0.41f);
                    av.HairColor = new Color(0.55f, 0.22f, 0.12f);
                    av.HairPath = "Avatar/Hair/Monk/Monk";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_OpenMouthSmile", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/TiredEyes", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/FlannelButtonUp", new Color(0.72f, 0.38f, 0.62f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.2f, 0.26f, 0.34f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/RectangleFrameGlasses/RectangleFrameGlasses", new Color(0.22f, 0.78f, 0.36f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sandals/Sandals", new Color(0.42f, 0.28f, 0.18f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 280f, maxWeekly: 480f)
                        .WithOrdersPerWeek(1, 3)
                        .WithPreferredOrderDay(Day.Wednesday)
                        .WithOrderTime(1545)
                        .WithStandards(CustomerStandard.VeryLow)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.08f)
                        .WithDependence(baseAddiction: 0.05f, dependenceMultiplier: 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.35f), (DrugType.Methamphetamine, -0.2f), (DrugType.Shrooms, 0.55f), (DrugType.Cocaine, -0.35f)
                        })
                        .WithPreferredProperties(Property.Munchies, Property.ThoughtProvoking, Property.Sneaky);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("brack_silt", "nadia_rim", "manhole_mike");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new WalkToSpec { Destination = crossroad, StartTime = 0755, FaceDestinationDirection = false });
                    plan.Add(new WalkToSpec { Destination = pit, StartTime = 0952, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = sewerBalcony, StartTime = 1218, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = sewerCrossSection, StartTime = 1433, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = underMotelChill, StartTime = 1647, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 105, 0) * Vector3.forward });
                    plan.Add(new WalkToSpec { Destination = sewerWarehouse, StartTime = 1904, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = sewerBalcony, StartTime = 2126, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = pit, StartTime = 2341, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = sewerStorageChill, StartTime = 0214, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 210, 0) * Vector3.forward });
                });
        }

        public RustySump() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 0.42f;
                Region = Region.Docks;

                Schedule.Enable();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"RustySump OnCreated failed: {ex.Message}");
                MelonLogger.Error($"StackTrace: {ex.StackTrace}");
            }
        }
    }
}
