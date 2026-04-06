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

    public sealed class OrlandoCastillo : NPC

    {

        public override bool IsPhysical => true;



        protected override void ConfigurePrefab(NPCPrefabBuilder builder)

        {

            var cafe = Building.Get<Cafe>();

            var slop = Building.Get<SlopShop>();

            Vector3 spawnPos = new Vector3(71.2f, 1.065f, 18.4f);



            builder.WithIdentity("orlando_castillo", "Orlando", "Castillo")

                .WithAppearanceDefaults(av =>

                {

                    av.Gender = 0.0f;

                    av.Height = 1.01f;

                    av.Weight = 0.46f;

                    av.SkinColor = new Color(0.52f, 0.41f, 0.34f);

                    av.LeftEyeLidColor = av.SkinColor;

                    av.RightEyeLidColor = av.SkinColor;

                    av.EyeBallTint = new Color(0.96f, 0.90f, 0.86f);

                    av.PupilDilation = 0.58f;

                    av.EyebrowScale = 1.04f;

                    av.EyebrowThickness = 0.94f;

                    av.EyebrowRestingHeight = -0.1f;

                    av.EyebrowRestingAngle = 0.4f;

                    av.LeftEye = (0.32f, 0.42f);

                    av.RightEye = (0.32f, 0.42f);

                    av.HairColor = new Color(0.08f, 0.07f, 0.08f);

                    av.HairPath = "Avatar/Hair/Tony/Tony";

                    av.WithFaceLayer("Avatar/Layers/Face/Face_Neutral", Color.black);

                    av.WithBodyLayer("Avatar/Layers/Top/Tucked T-Shirt", new Color(0.38f, 0.38f, 0.40f));

                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.20f, 0.22f, 0.28f));

                    av.WithAccessoryLayer("Avatar/Accessories/Chest/OpenVest/OpenVest", new Color(0.24f, 0.22f, 0.20f));

                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.16f, 0.16f, 0.18f));

                })

                .WithSpawnPosition(spawnPos)

                .EnsureCustomer()

                .WithCustomerDefaults(cd =>

                {

                    cd.WithSpending(450f, 900f)

                        .WithOrdersPerWeek(1, 3)

                        .WithPreferredOrderDay(Day.Thursday)

                        .WithOrderTime(1945)

                        .WithStandards(CustomerStandard.Moderate)

                        .AllowDirectApproach(true)

                        .GuaranteeFirstSample(false)

                        .WithMutualRelationRequirement(0f, 1f)

                        .WithCallPoliceChance(0.18f)

                        .WithDependence(0.09f, 1f)

                        .WithAffinities(new[]

                        {

                            (DrugType.Marijuana, 0.36f), (DrugType.Methamphetamine, 0.06f), (DrugType.Shrooms, 0.24f), (DrugType.Cocaine, -0.12f)

                        })

                        .WithPreferredProperties(Property.Focused, Property.Foggy, Property.Refreshing);

                })

                .WithRelationshipDefaults(r =>

                {

                    r.WithDelta(2.0f)

                        .SetUnlocked(false)

                        .WithConnectionsById("milton_delaney", "jeff_gilmore")

                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach);

                })

                .WithSchedule(plan =>

                {

                    plan.EnsureDealSignal();

                    plan.Add(new StayInBuildingSpec { BuildingName = "Tall Tower", StartTime = 0855, DurationMinutes = 124 });

                    plan.Add(new SitSpec { SeatSetPath = "@Businesses/Taco Ticklers/Fast Food Booth (1)", StartTime = 1100, DurationMinutes = 109 });

                    plan.StayInBuilding(cafe, 1210, 134);

                    plan.StayInBuilding(slop, 1635, 149);

                    plan.Add(new StayInBuildingSpec { BuildingName = "Apartment Building 2", StartTime = 2110, DurationMinutes = 719 });

                });

        }



        public OrlandoCastillo() : base() { }



        protected override void OnCreated()

        {

            try

            {

                base.OnCreated();

                Appearance.Build();

                Aggressiveness = 0.44f;

                Region = Region.Downtown;

                Schedule.Enable();

            }

            catch (System.Exception ex)

            {

                MelonLogger.Error($"OrlandoCastillo OnCreated failed: {ex.Message}");

            }

        }

    }

}

