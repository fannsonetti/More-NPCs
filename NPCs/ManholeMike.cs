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
    public sealed class ManholeMike : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            Vector3 sewerBalcony = new Vector3(77.1413f, -4.535f, 34.6874f);
            Vector3 crossroad = new Vector3(97.4504f, 1.065f, 7.0207f);
            Vector3 pit = new Vector3(44.4508f, -8.035f, 36.1825f);
            Vector3 sewerWarehouse = new Vector3(74.0413f, -4.535f, 28.6874f);
            Vector3 sewerOffice = new Vector3(48.6662f, -8.035f, 71.6929f);
            // var building = Buildings.GetAll().First();
            builder.WithIdentity("manhole_mike", "Manhole Mike", "")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 1.0f;
                    av.Weight = 0.4f;
                    av.SkinColor = new Color(0.874f, 0.741f, 0.631f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(1.0f, 0.825f, 0.825f);
                    av.PupilDilation = 0.75f;
                    av.EyebrowScale = 1.15f;
                    av.EyebrowThickness = 0.89f;
                    av.EyebrowRestingHeight = -0.47f;
                    av.EyebrowRestingAngle = 5.84f;
                    av.LeftEye = (0.2f, 0.5f);
                    av.RightEye = (0.22f, 0.48f);
                    av.HairColor = new Color(0.32f, 0.32f, 0.32f);
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Agitated", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Goatee", Color.white);
                    av.WithFaceLayer("Avatar/Layers/Face/OldPersonWrinkles", new Color(0.957f, 0.474f, 0.938f));
                    av.WithBodyLayer("Avatar/Layers/Top/UpperBodyTattoos", Color.white);
                    av.WithBodyLayer("Avatar/Layers/Top/Nipples", new Color(0.481f, 0.331f, 0.225f));
                    av.WithBodyLayer("Avatar/Layers/Top/ChestHair1", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Bottom/MaleUnderwear", new Color(0.613f, 0.493f, 0.344f));
                })
                .WithSpawnPosition(sewerOffice)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 600f, maxWeekly: 800f)
                        .WithOrdersPerWeek(1, 3)
                        .WithPreferredOrderDay(Day.Monday)
                        .WithOrderTime(2200)
                        .WithStandards(CustomerStandard.VeryLow)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.0f)
                        .WithDependence(baseAddiction: 0.0f, dependenceMultiplier: 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, -0.73f), (DrugType.Methamphetamine, 0.92f), (DrugType.Shrooms, 0.05f), (DrugType.Cocaine, -0.26f)
                        })
                        .WithPreferredProperties(Property.Paranoia, Property.Laxative, Property.Munchies);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("cranky_frank", "anna_chesterfield","mack");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new WalkToSpec { Destination = pit, StartTime = 0857, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = sewerBalcony, StartTime = 1127, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = sewerOffice, StartTime = 1403, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = sewerWarehouse, StartTime = 1556, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = sewerOffice, StartTime = 1804, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = sewerBalcony, StartTime = 2026, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = crossroad, StartTime = 2304, FaceDestinationDirection = false });
                    plan.Add(new WalkToSpec { Destination = sewerWarehouse, StartTime = 0057, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = sewerBalcony, StartTime = 0303, FaceDestinationDirection = true });
                });
        }

        public ManholeMike() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 5f;
                Region = S1API.Map.Region.Docks;

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


