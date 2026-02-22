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
    /// An example S1API NPC that opts into a physical rig.
    /// Demonstrates movement and inventory usage.
    /// </summary>
    public sealed class Corkskrew : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var goblinHiding = Building.Get<GoblinHideBuilding> ();
            Vector3 sewerCrossSection = new Vector3(83.0061f, -5.785f, 5.8099f);
            Vector3 forestStraight = new Vector3(136.5495f, 0.7765f, -20.7365f);
            Vector3 forestRock = new Vector3(138.6413f, 1.2155f, -39.32f);
            Vector3 forestBarn = new Vector3(164.8612f, 2.8479f, -29.5627f);
            Vector3 forestMansion = new Vector3(164.8612f, 2.8479f, -29.5627f);
            builder.WithIdentity("corkskrew", "Corkskrew", "")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 1.0f;
                    av.Weight = 0.35f;
                    av.SkinColor = new Color(0.525f, 0.427f, 0.337f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(1.0f, 0.82f, 0.82f);
                    av.PupilDilation = 0.6f;
                    av.EyebrowScale = 1.23f;
                    av.EyebrowThickness = 1.48f;
                    av.EyebrowRestingHeight = -0.25f;
                    av.EyebrowRestingAngle = 5.67f;
                    av.LeftEye = (0.31f, 0.35f);
                    av.RightEye = (0.31f, 0.35f);
                    av.HairColor = new Color(0.075f, 0.075f, 0.075f);
                    av.HairPath = "Avatar/Hair/Closebuzzcut/CloseBuzzCut";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Agitated", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Goatee", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Tattoos/Face/Face_Sword", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/HazmatSuit", new Color(0.943f, 0.576f, 0.316f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(1.0f, 1.0f, 1.0f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(1.0f, 1.0f, 1.0f));
                })
                .WithSpawnPosition(sewerCrossSection)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 400f, maxWeekly: 600f)
                        .WithOrdersPerWeek(1, 4)
                        .WithPreferredOrderDay(Day.Thursday)
                        .WithOrderTime(2300)
                        .WithStandards(CustomerStandard.Low)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.15f)
                        .WithDependence(baseAddiction: 0.25f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.52f), (DrugType.Methamphetamine, 0.73f), (DrugType.Shrooms, -0.43f), (DrugType.Cocaine, 0.14f)
                        })
                        .WithPreferredProperties(Property.AntiGravity, Property.Spicy, Property.CalorieDense);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("trent_sherman", "keith_wagner");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new WalkToSpec { Destination = sewerCrossSection, StartTime = 627, FaceDestinationDirection = true });
                    plan.StayInBuilding(goblinHiding, 697, 826);
                    plan.Add(new WalkToSpec { Destination = forestStraight, StartTime = 2104, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = forestRock, StartTime = 2226, FaceDestinationDirection = true });
                    plan.UseVendingMachine(2356);
                    plan.Add(new WalkToSpec { Destination = forestBarn, StartTime = 057, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = forestMansion, StartTime = 303, FaceDestinationDirection = true });
                });
        }

        public Corkskrew() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 5f;
                Region = S1API.Map.Region.Westville;

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


