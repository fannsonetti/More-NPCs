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
    /// <summary>
    /// An example S1API NPC that opts into a physical rig.
    /// Demonstrates movement and inventory usage.
    /// </summary>
    public sealed class JasonReed : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            Vector3 tacoticklers = new Vector3(-28.9266f, 1.065f, 74.6178f);
            Vector3 outside = new Vector3(-36.3346f, 1.065f, 75.6414f);
            Vector3 spawnPos = new Vector3(-28.9266f, 1.065f, 74.6178f);
            // var building = Buildings.GetAll().First();
            builder.WithIdentity("jason_reed", "Jason", "Reed")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 0.95f;
                    av.Weight = 1.0f;
                    av.SkinColor = new Color(0.713f, 0.592f, 0.486f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(1.0f, 0.8f, 0.8f);
                    av.PupilDilation = 0.75f;
                    av.EyebrowScale = 1.39f;
                    av.EyebrowThickness = 0.5f;
                    av.EyebrowRestingHeight = -0.48f;
                    av.EyebrowRestingAngle = -4.64f;
                    av.LeftEye = (0.18f, 0.24f);
                    av.RightEye = (0.18f, 0.24f);
                    av.HairColor = new Color(0.55f, 0.41f, 0.31f);
                    av.HairPath = "Avatar/Hair/SidePartBob/SidePartBob";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_NeutralPout", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/Freckles", new Color(0.642f, 0.533f, 0.437f));
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Stubble", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/FastFood T-Shirt", new Color(1.0f, 0.9f, 0.7f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.23529411852359773f, 0.23529411852359773f, 0.23529411852359773f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.23529411852359773f, 0.23529411852359773f, 0.23529411852359773f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/Belt/Belt", new Color(0.23529411852359773f, 0.23529411852359773f, 0.23529411852359773f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 400f, maxWeekly: 800f)
                        .WithOrdersPerWeek(2, 4)
                        .WithPreferredOrderDay(Day.Monday)
                        .WithOrderTime(2020)
                        .WithStandards(CustomerStandard.VeryLow)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.33f)
                        .WithDependence(baseAddiction: 0.1f, dependenceMultiplier: 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.86f), (DrugType.Methamphetamine, 0.25f), (DrugType.Shrooms, 1f), (DrugType.Cocaine, -0.67f)
                        })
                        .WithPreferredProperties(Property.LongFaced, Property.CalorieDense, Property.Shrinking);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("kyle_cooley", "austin_steiner");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new WalkToSpec { Destination = tacoticklers, StartTime = 557, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 180, 0) * Vector3.forward});
                    plan.Add(new WalkToSpec { Destination = outside, StartTime = 1804, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 240, 0) * Vector3.forward});
                    plan.Add(new WalkToSpec { Destination = tacoticklers, StartTime = 2003, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 180, 0) * Vector3.forward });
                    plan.Add(new UseVendingMachineSpec { StartTime = 2157 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "North apartments", StartTime = 2226, DurationMinutes = 450 });
                });
        }

        public JasonReed() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 5f;
                Region = S1API.Map.Region.Northtown;

                // Customer.RequestProduct();

                Schedule.Enable();

            }
            catch (Exception ex)
            {
                MelonLogger.Error($"ExamplePhysicalNPC OnCreated failed: {ex.Message}");
                MelonLogger.Error($"StackTrace: {ex.StackTrace}");
            }
        }
    }
}


