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
    public sealed class JuniperLyre : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var overpass = Building.Get<SouthOverpassBuilding>();
            var pillville = Building.Get<Pillville>();
            var cafe = Building.Get<Cafe>();

            Vector3 spawnPos = new Vector3(134.1605f, 6.0623f, 114.3804f);
            // Open-world walk point (downtown town center area) — not a Building / StayInBuilding.
            Vector3 townCenter = new Vector3(69.7895f, 1.065f, 15.4409f);

            builder.WithIdentity("juniper_lyre", "Juniper", "Lyre")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.86f;
                    av.Height = 0.99f;
                    av.Weight = 0.42f;
                    av.SkinColor = new Color(0.62f, 0.50f, 0.41f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.96f, 0.90f, 0.88f);
                    av.PupilDilation = 0.62f;
                    av.EyebrowScale = 0.95f;
                    av.EyebrowThickness = 0.82f;
                    av.EyebrowRestingHeight = -0.08f;
                    av.EyebrowRestingAngle = 1.90f;
                    av.LeftEye = (0.32f, 0.46f);
                    av.RightEye = (0.32f, 0.46f);

                    av.HairPath = "Avatar/Hair/MidFringe/MidFringe";
                    av.HairColor = new Color(0.5714f, 0.2964f, 0.0411f, 1f);

                    av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", Color.black);

                    av.WithBodyLayer("Avatar/Layers/Top/RolledButtonUp", new Color(0.42f, 0.22f, 0.18f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.18f, 0.22f, 0.30f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.14f, 0.15f, 0.17f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/Cap/Cap", new Color(0.26f, 0.24f, 0.22f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(400f, 800f)
                        .WithOrdersPerWeek(1, 3)
                        .WithPreferredOrderDay(Day.Thursday)
                        .WithOrderTime(2130)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(0f, 1f)
                        .WithCallPoliceChance(0.14f)
                        .WithDependence(0.08f, 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.12f),
                            (DrugType.Methamphetamine, -0.38f),
                            (DrugType.Shrooms, 0.78f),
                            (DrugType.Cocaine, -0.22f)
                        })
                        .WithPreferredProperties(Property.Glowie, Property.Calming, Property.Euphoric);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("lucy_pennington", "damon_rusk");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();

                    // After overpass sleep (~07:30): morning downtown, commute to Pillville shift, back downtown, night under overpass.
                    plan.UseVendingMachine(0740);
                    plan.Add(new WalkToSpec { Destination = townCenter, StartTime = 0810, FaceDestinationDirection = true });
                    plan.StayInBuilding(pillville, 0900, 479);
                    plan.Add(new WalkToSpec { Destination = townCenter, StartTime = 1730, FaceDestinationDirection = true });
                    plan.StayInBuilding(cafe, 1745, 224);
                    plan.UseVendingMachine(2130);
                    plan.StayInBuilding(overpass, 2230, 539);
                });
        }

        public JuniperLyre() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 0.42f;
                Region = Region.Downtown;

                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"JuniperLyre OnCreated failed: {ex.Message}");
            }
        }
    }
}
