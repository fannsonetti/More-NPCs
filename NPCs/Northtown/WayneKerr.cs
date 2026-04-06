using MelonLoader;
using S1API.Economy;
using S1API.Entities;
using S1API.Entities.Schedule;
using S1API.Entities.NPCs.Northtown;
using S1API.GameTime;
using S1API.Map;
using S1API.Map.Buildings;
using S1API.Products;
using S1API.Properties;
using UnityEngine;

namespace MoreNPCs.NPCs
{
    public sealed class WayneKerr : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var petersRoom = Building.Get<PetersRoom>();
            var motelOffice = Building.Get<MotelOffice>();
            var budsBar = Building.Get<BudsBar>();
            Vector3 spawnPos = new Vector3(-49.5478f, -4.035f, 168.5777f);
            Vector3 posA = new Vector3(-76.0633f, -1.535f, 44.6816f);
            Vector3 posB = new Vector3(-49.5478f, -4.035f, 168.5777f);
            builder.WithIdentity("wayne_kerr", "Wayne", "Kerr")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 1.0f;
                    av.Weight = 0.30f;
                    av.SkinColor = new Color(0.784f, 0.654f, 0.545f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = Color.white;
                    av.PupilDilation = 0.475f;
                    av.EyebrowScale = 1.15f;
                    av.EyebrowThickness = 1.1f;
                    av.EyebrowRestingHeight = -0.4f;
                    av.EyebrowRestingAngle = 0.0f;
                    av.LeftEye = (0.34f, 0.45f);
                    av.RightEye = (0.34f, 0.45f);
                    av.HairColor = new Color(0.198f, 0.118f, 0.062f);
                    av.HairPath = "Avatar/Hair/Spiky/Spiky";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SmugPout", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Stubble", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/ButtonUp", new Color(0.36f, 0.30f, 0.24f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.151f, 0.151f, 0.151f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(0.34f, 0.28f, 0.22f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/Belt/Belt", new Color(0.151f, 0.151f, 0.151f));
                    av.WithAccessoryLayer("Avatar/Accessories/Hands/Polex/Polex", new Color(0.151f, 0.151f, 0.151f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 400f, maxWeekly: 700f)
                        .WithOrdersPerWeek(2, 4)
                        .WithPreferredOrderDay(Day.Tuesday)
                        .WithOrderTime(0715)
                        .WithStandards(CustomerStandard.High)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.29f)
                        .WithDependence(baseAddiction: 0.25f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, -0.82f), (DrugType.Methamphetamine, 0.0f), (DrugType.Shrooms, 0.73f), (DrugType.Cocaine, 0.61f)
                        })
                        .WithPreferredProperties(Property.Zombifying, Property.ThoughtProvoking, Property.Sedating);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("peter_file", "ludwig_meyer");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.UseVendingMachine(0833);
                    plan.Add(new WalkToSpec { Destination = posA, StartTime = 0853, FaceDestinationDirection = true });
                    plan.StayInBuilding(petersRoom, 1035, 111);
                    plan.LocationDialogue(posA, 1307);
                    plan.UseVendingMachine(1334);
                    plan.StayInBuilding(petersRoom, 1359, 118);
                    plan.UseATM(1558);
                    plan.StayInBuilding(budsBar, 1657, 156);
                    plan.UseVendingMachine(1934);
                    plan.Add(new WalkToSpec { Destination = posB, StartTime = 2028, FaceDestinationDirection = false });
                    plan.StayInBuilding(petersRoom, 2157, 450);
                });
        }

        public WayneKerr() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 0.63f;
                Region = Region.Northtown;

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



