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
    public sealed class DamonRusk : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var cafe = Building.Get<Cafe>();
            var supermarket = Building.Get<Supermarket>();
            var slopShop = Building.Get<SlopShop>();

            Vector3 spawnPos = new Vector3(122.4035f, 6.0615f, 105.2812f);
            Vector3 townCenter = new Vector3(69.7895f, 1.065f, 15.4409f);

            builder.WithIdentity("damon_rusk", "Damon", "Rusk")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 1.02f;
                    av.Weight = 0.52f;
                    av.SkinColor = new Color(0.28f, 0.21f, 0.18f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.88f, 0.82f, 0.78f);
                    av.PupilDilation = 0.66f;
                    av.EyebrowScale = 1.02f;
                    av.EyebrowThickness = 1.18f;
                    av.EyebrowRestingHeight = -0.16f;
                    av.EyebrowRestingAngle = 1.25f;
                    av.LeftEye = (0.30f, 0.40f);
                    av.RightEye = (0.30f, 0.40f);

                    av.HairPath = "Avatar/Hair/Shoulderlength/ShoulderLength";
                    av.HairColor = new Color(0.06f, 0.06f, 0.07f);

                    av.WithFaceLayer("Avatar/Layers/Face/Face_Neutral", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Stubble", new Color(0.05f, 0.05f, 0.05f));
                    av.WithFaceLayer("Avatar/Layers/Face/TiredEyes", new Color(0f, 0f, 0f, 0.42f));

                    av.WithBodyLayer("Avatar/Layers/Top/Buttonup", new Color(0.22f, 0.24f, 0.28f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.18f, 0.19f, 0.22f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/OpenVest/OpenVest", new Color(0.20f, 0.20f, 0.22f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/DressShoes/DressShoes", new Color(0.10f, 0.10f, 0.11f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(500f, 800f)
                        .WithOrdersPerWeek(1, 4)
                        .WithPreferredOrderDay(Day.Sunday)
                        .WithOrderTime(2030)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(0f, 1f)
                        .WithCallPoliceChance(0.16f)
                        .WithDependence(0.10f, 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.18f),
                            (DrugType.Methamphetamine, -0.22f),
                            (DrugType.Shrooms, 0.28f),
                            (DrugType.Cocaine, -0.10f)
                        })
                        .WithPreferredProperties(Property.Smelly, Property.Foggy, Property.Sneaky);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("juniper_lyre");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();

                    plan.Add(new StayInBuildingSpec { BuildingName = "Apartment Building 2", StartTime = 0020, DurationMinutes = 439 });
                    plan.StayInBuilding(cafe, 0905, 134);
                    plan.Add(new WalkToSpec { Destination = townCenter, StartTime = 1120, FaceDestinationDirection = true });
                    plan.StayInBuilding(supermarket, 1245, 164);
                    plan.UseATM(1505);
                    plan.StayInBuilding(slopShop, 1610, 119);
                    plan.Add(new StayInBuildingSpec { BuildingName = "Tall Tower", StartTime = 1810, DurationMinutes = 149 });
                    plan.Add(new WalkToSpec { Destination = townCenter, StartTime = 2040, FaceDestinationDirection = true });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Apartment Building 2", StartTime = 2135, DurationMinutes = 764 });
                });
        }

        public DamonRusk() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 0.58f;
                Region = Region.Downtown;

                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"DamonRusk OnCreated failed: {ex.Message}");
            }
        }
    }
}
