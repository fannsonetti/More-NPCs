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
    /// Max Pennyson - Grandpa Ben-inspired character from Downtown (looks based on him, parody name). Eats at Slop Shop 90+ min at dinner, walks forest before. South Overpass Building after dinner until wake 7:02. Connections: bruce_norton, philip_wentworth.
    /// </summary>
    public sealed class MaxPennyson : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var slopShop = Building.Get<SlopShop>();
            var southOverpass = Building.Get<SouthOverpassBuilding>();
            Vector3 forestStraight = new Vector3(136.5495f, 0.7765f, -20.7365f);
            Vector3 forestRock = new Vector3(138.6413f, 1.2155f, -39.32f);
            Vector3 forestBarn = new Vector3(164.8612f, 2.8479f, -29.5627f);
            Vector3 forestMansion = new Vector3(164.8612f, 2.8479f, -29.5627f);
            Vector3 spawnPos = forestStraight;
            builder.WithIdentity("max_pennyson", "Max", "Pennyson")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 1.05f;
                    av.Weight = 0.88f;
                    av.SkinColor = new Color(0.78f, 0.65f, 0.55f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(1.0f, 0.9f, 0.88f);
                    av.PupilDilation = 0.65f;
                    av.EyebrowScale = 1.2f;
                    av.EyebrowThickness = 1.1f;
                    av.EyebrowRestingHeight = -0.12f;
                    av.EyebrowRestingAngle = 2.8f;
                    av.LeftEye = (0.26f, 0.44f);
                    av.RightEye = (0.26f, 0.44f);
                    av.HairColor = new Color(0.75f, 0.72f, 0.68f);
                    av.HairPath = "Avatar/Hair/ShortBuzz/ShortBuzz";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/OldPersonWrinkles", new Color32(255, 0, 255, 8));
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Goatee", new Color(0.45f, 0.42f, 0.38f));
                    av.WithBodyLayer("Avatar/Layers/Top/Buttonup", new Color(0.78f, 0.45f, 0.38f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.2f, 0.28f, 0.48f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.42f, 0.32f, 0.24f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/Belt/Belt", new Color(0.28f, 0.20f, 0.14f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 600f, maxWeekly: 900f)
                        .WithOrdersPerWeek(1, 4)
                        .WithPreferredOrderDay(Day.Saturday)
                        .WithOrderTime(1900)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.18f)
                        .WithDependence(baseAddiction: 0.2f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.45f), (DrugType.Methamphetamine, -0.52f), (DrugType.Shrooms, 0.62f), (DrugType.Cocaine, -0.28f)
                        })
                        .WithPreferredProperties(Property.Munchies, Property.Calming, Property.ThoughtProvoking);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("bruce_norton", "philip_wentworth");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new WalkToSpec { Destination = forestStraight, StartTime = 702, FaceDestinationDirection = true }); // wake 7:02, leave South Overpass
                    plan.Add(new WalkToSpec { Destination = forestRock, StartTime = 858, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = forestBarn, StartTime = 1004, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = forestMansion, StartTime = 1156, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = forestStraight, StartTime = 1312, FaceDestinationDirection = true });
                    plan.StayInBuilding(slopShop, 1717, 90); // dinner 90 min at Slop Shop
                    plan.StayInBuilding(southOverpass, 1908, 602); // South Overpass after dinner until wake 7:02
                });
        }

        public MaxPennyson() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 3f;
                Region = S1API.Map.Region.Downtown;

                Schedule.Enable();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"MaxPennyson OnCreated failed: {ex.Message}");
                MelonLogger.Error($"StackTrace: {ex.StackTrace}");
            }
        }
    }
}
