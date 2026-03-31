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
    public sealed class MarisEldridge : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var cafe = Building.Get<Cafe>();
            var supermarket = Building.Get<Supermarket>();

            Vector3 spawnPos = new Vector3(-0.15f, 1.05f, 70.95f);
            Vector3 townCenter = new Vector3(69.7895f, 1.065f, 15.4409f); // walk waypoint only, not a building

            builder.WithIdentity("maris_eldridge", "Maris", "Eldridge")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 1.0f;
                    av.Height = 0.98f;
                    av.Weight = 0.36f;
                    av.SkinColor = new Color(0.56f, 0.46f, 0.38f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.98f, 0.92f, 0.90f);
                    av.PupilDilation = 0.72f;
                    av.EyebrowScale = 1.02f;
                    av.EyebrowThickness = 0.78f;
                    av.EyebrowRestingHeight = -0.04f;
                    av.EyebrowRestingAngle = 2.35f;
                    av.LeftEye = (0.34f, 0.48f);
                    av.RightEye = (0.34f, 0.48f);

                    av.HairPath = "Avatar/Hair/MessyBob/MessyBob";
                    av.HairColor = new Color(0.32f, 0.18f, 0.40f);

                    av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", Color.black);
                    // TiredEyes tints from layer color; use opaque black so it doesn’t pick up hair tint (purple read as bruised).
                    av.WithFaceLayer("Avatar/Layers/Face/TiredEyes", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Bottom/FemaleUnderwear", new Color(0.72f, 0.67f, 0.64f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.20f, 0.22f, 0.28f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.15f, 0.15f, 0.17f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/CollarJacket/CollarJacket", new Color(0.22f, 0.24f, 0.28f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(500f, 900f)
                        .WithOrdersPerWeek(1, 4)
                        .WithPreferredOrderDay(Day.Wednesday)
                        .WithOrderTime(1845)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(0f, 1f)
                        .WithCallPoliceChance(0.18f)
                        .WithDependence(0.09f, 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.10f),
                            (DrugType.Methamphetamine, -0.28f),
                            (DrugType.Shrooms, 0.44f),
                            (DrugType.Cocaine, -0.16f)
                        })
                        .WithPreferredProperties(Property.Euphoric, Property.Glowie, Property.Focused);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("damon_rusk", "calder_wren");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();

                    plan.Add(new StayInBuildingSpec { BuildingName = "Apartment Building 2", StartTime = 0010, DurationMinutes = 559 });
                    plan.StayInBuilding(cafe, 0930, 149);
                    plan.Add(new WalkToSpec { Destination = townCenter, StartTime = 1200, FaceDestinationDirection = true });
                    plan.StayInBuilding(supermarket, 1245, 179);
                    plan.Add(new StayInBuildingSpec { BuildingName = "The Crimson Canary", StartTime = 1715, DurationMinutes = 134 });
                    plan.UseVendingMachine(2005);
                    plan.Add(new StayInBuildingSpec { BuildingName = "Apartment Building 2", StartTime = 2045, DurationMinutes = 204 });
                });
        }

        public MarisEldridge() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 0.46f;
                Region = Region.Downtown;

                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"MarisEldridge OnCreated failed: {ex.Message}");
            }
        }
    }
}
