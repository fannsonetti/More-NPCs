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
    /// <summary>
    /// An example S1API NPC that opts into a physical rig.
    /// Demonstrates movement and inventory usage.
    /// </summary>
    public sealed class WayneKerr : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var petersRoom = Building.Get<PetersRoom>();
            var motelOffice = Building.Get<MotelOffice>();
            var budsBar = Building.Get<BudsBar>();
            Vector3 spawnPos = new Vector3(-71.2847f, -2.935f, 145.6754f);
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
                    av.WithBodyLayer("Avatar/Layers/Top/ButtonUp", new Color(0.481f, 0.331f, 0.225f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.151f, 0.151f, 0.151f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(0.481f, 0.331f, 0.225f));
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
                        .WithCallPoliceChance(0.25f)
                        .WithDependence(baseAddiction: 0.25f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, -0.82f), (DrugType.Methamphetamine, 0.0f), (DrugType.Shrooms, 0.73f), (DrugType.Cocaine, 0.61f)
                        })
                        .WithPreferredProperties(Property.Calming);
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
                    plan.UseVendingMachine(896);
                    plan.Add(new WalkToSpec { Destination = posA, StartTime = 922, FaceDestinationDirection = true });
                    plan.StayInBuilding(petersRoom, 1104, 111);
                    plan.LocationDialogue(posA, 1296);
                    plan.UseVendingMachine(1403);
                    plan.StayInBuilding(petersRoom, 1428, 118);
                    plan.UseATM(1627);
                    plan.StayInBuilding(budsBar, 1726, 156);
                    plan.UseVendingMachine(2003);
                    plan.Add(new WalkToSpec { Destination = posB, StartTime = 2057, FaceDestinationDirection = false });
                    plan.StayInBuilding(motelOffice, 2226, 450);
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

                Aggressiveness = 5f;
                Region = Region.Northtown;

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


