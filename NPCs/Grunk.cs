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
    /// Grunk - sewer-dwelling ogre. Home: Goblin Hide Building.
    /// </summary>
    public sealed class Grunk : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var goblinHiding = Building.Get<GoblinHideBuilding>();
            Vector3 pit = new Vector3(44.4508f, -8.035f, 36.1825f);
            Vector3 sewerBalcony = new Vector3(77.1413f, -4.535f, 34.6874f);
            Vector3 sewerWarehouse = new Vector3(74.0413f, -4.535f, 28.6874f);
            Vector3 crossroad = new Vector3(97.4504f, 1.065f, 7.0207f);
            Vector3 sewerCrossSection = new Vector3(83.0061f, -5.785f, 5.8099f);
            Vector3 spawnPos = sewerCrossSection;

            builder.WithIdentity("grunk", "Grunk", "")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 1.6f;
                    av.Weight = 1f;
                    av.SkinColor = new Color(0.478f, 0.4659f, 0.3983f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(1f, 1f, 1f);
                    av.PupilDilation = 0.6f;
                    av.EyebrowScale = 0f;
                    av.EyebrowThickness = 0f;
                    av.LeftEye = (0.3594f, 1f);
                    av.RightEye = (0.3594f, 1f);
                    av.HairPath = "";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Agitated", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/TiredEyes", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/OldPersonWrinkles", new Color(0.62f, 0.30f, 0.60f));
                    av.WithFaceLayer("Avatar/Layers/Face/Eyeshadow", new Color(0f, 0f, 0f, 1f));
                    av.WithBodyLayer("Avatar/Layers/Top/Nipples", new Color(0.5402f, 0.5124f, 0.5124f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/MaleUnderwear", new Color(0.3884f, 0.37f, 0.313f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 600f, maxWeekly: 900f)
                        .WithOrdersPerWeek(1, 4)
                        .WithPreferredOrderDay(Day.Wednesday)
                        .WithOrderTime(2000)
                        .WithStandards(CustomerStandard.VeryLow)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.0f)
                        .WithDependence(baseAddiction: 0.1f, dependenceMultiplier: 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, -0.68f), (DrugType.Methamphetamine, 0.86f), (DrugType.Shrooms, -0.02f), (DrugType.Cocaine, -0.85f)
                        })
                        .WithPreferredProperties(Property.Munchies, Property.Paranoia);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("manhole_mike");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new WalkToSpec { Destination = crossroad, StartTime = 0204, FaceDestinationDirection = false });
                    plan.Add(new WalkToSpec { Destination = sewerWarehouse, StartTime = 0327, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = pit, StartTime = 0533, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = sewerBalcony, StartTime = 0626, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = sewerWarehouse, StartTime = 1104, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = sewerBalcony, StartTime = 1256, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = crossroad, StartTime = 1504, FaceDestinationDirection = false });
                    plan.Add(new WalkToSpec { Destination = pit, StartTime = 1726, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = sewerWarehouse, StartTime = 1932, FaceDestinationDirection = true });
                    plan.StayInBuilding(goblinHiding, 2103, 300);
                });
        }

        public Grunk() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 8f;
                Region = Region.Docks;

                Schedule.Enable();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Grunk OnCreated failed: {ex.Message}");
                MelonLogger.Error($"StackTrace: {ex.StackTrace}");
            }
        }
    }
}
