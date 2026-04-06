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

    public sealed class TessaWard : NPC

    {

        public override bool IsPhysical => true;



        protected override void ConfigurePrefab(NPCPrefabBuilder builder)

        {

            var supermarket = Building.Get<Supermarket>();
            var cafe = Building.Get<Cafe>();
            Vector3 plaza = new Vector3(69.7895f, 1.065f, 15.4409f);

            Vector3 spawnPos = new Vector3(22.4f, 1.065f, 54.3f);



            builder.WithIdentity("tessa_ward", "Tessa", "Ward")

                .WithAppearanceDefaults(av =>

                {

                    av.Gender = 0.88f;

                    av.Height = 0.98f;

                    av.Weight = 0.36f;

                    av.SkinColor = new Color(0.56f, 0.46f, 0.38f);

                    av.LeftEyeLidColor = av.SkinColor;

                    av.RightEyeLidColor = av.SkinColor;

                    av.EyeBallTint = new Color(0.98f, 0.92f, 0.90f);

                    av.PupilDilation = 0.72f;

                    av.EyebrowScale = 0.95f;

                    av.EyebrowThickness = 0.8f;

                    av.EyebrowRestingHeight = -0.06f;

                    av.EyebrowRestingAngle = 2.1f;

                    av.LeftEye = (0.33f, 0.45f);

                    av.RightEye = (0.33f, 0.45f);

                    av.HairColor = new Color(0.18f, 0.14f, 0.12f);

                    av.HairPath = "Avatar/Hair/Shoulderlength/ShoulderLength";

                    av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", Color.black);

                    av.WithBodyLayer("Avatar/Layers/Top/V-Neck", new Color(0.36f, 0.34f, 0.38f));

                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.22f, 0.24f, 0.30f));

                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Flats/Flats", new Color(0.18f, 0.18f, 0.20f));

                })

                .WithSpawnPosition(spawnPos)

                .EnsureCustomer()

                .WithCustomerDefaults(cd =>

                {

                    cd.WithSpending(500f, 800f)

                        .WithOrdersPerWeek(1, 4)

                        .WithPreferredOrderDay(Day.Saturday)

                        .WithOrderTime(2015)

                        .WithStandards(CustomerStandard.Moderate)

                        .AllowDirectApproach(true)

                        .GuaranteeFirstSample(false)

                        .WithMutualRelationRequirement(0f, 1f)

                        .WithCallPoliceChance(0.15f)

                        .WithDependence(0.06f, 1f)

                        .WithAffinities(new[]

                        {

                            (DrugType.Marijuana, 0.22f), (DrugType.Methamphetamine, -0.18f), (DrugType.Shrooms, 0.44f), (DrugType.Cocaine, 0.10f)

                        })

                        .WithPreferredProperties(Property.Glowie, Property.Euphoric, Property.Sneaky);

                })

                .WithRelationshipDefaults(r =>

                {

                    r.WithDelta(2.0f)

                        .SetUnlocked(false)

                        .WithConnectionsById("faith_donovan", "calder_wren")

                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach);

                })

                .WithSchedule(plan =>

                {

                    plan.EnsureDealSignal();

                    plan.Add(new StayInBuildingSpec { BuildingName = "Tall Tower", StartTime = 0930, DurationMinutes = 104 });

                    plan.Add(new WalkToSpec { Destination = plaza, StartTime = 1036, FaceDestinationDirection = true });

                    plan.UseVendingMachine(1055);

                    plan.StayInBuilding(cafe, 1105, 129);

                    plan.Add(new WalkToSpec { Destination = plaza, StartTime = 1236, FaceDestinationDirection = true });

                    plan.StayInBuilding(supermarket, 1245, 164);

                    plan.Add(new StayInBuildingSpec { BuildingName = "Apartment Building 2", StartTime = 2115, DurationMinutes = 714 });

                });

        }



        public TessaWard() : base() { }



        protected override void OnCreated()

        {

            try

            {

                base.OnCreated();

                Appearance.Build();

                Aggressiveness = 0.36f;

                Region = Region.Downtown;

                Schedule.Enable();

            }

            catch (System.Exception ex)

            {

                MelonLogger.Error($"TessaWard OnCreated failed: {ex.Message}");

            }

        }

    }

}

