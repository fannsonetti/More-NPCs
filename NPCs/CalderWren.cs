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
    public sealed class CalderWren : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var cafe = Building.Get<Cafe>();
            var supermarket = Building.Get<Supermarket>();
            var slopShop = Building.Get<SlopShop>();

            Vector3 spawnPos = new Vector3(134.1605f, 6.0623f, 114.3804f);
            Vector3 townCenter = new Vector3(69.7895f, 1.065f, 15.4409f);

            builder.WithIdentity("calder_wren", "Calder", "Wren")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 1.00f;
                    av.Weight = 0.44f;
                    av.SkinColor = new Color(0.52f, 0.42f, 0.35f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.98f, 0.94f, 0.92f);
                    av.PupilDilation = 0.52f;

                    av.EyebrowScale = 1.14f;
                    av.EyebrowThickness = 1.52f;
                    av.EyebrowRestingHeight = -0.14f;
                    av.EyebrowRestingAngle = 0.85f;
                    av.LeftEye = (0.30f, 0.40f);
                    av.RightEye = (0.30f, 0.40f);

                    av.HairPath = "Avatar/Hair/Franklin/Franklin";
                    av.HairColor = new Color(0.22f, 0.18f, 0.16f);

                    av.WithFaceLayer("Avatar/Layers/Face/Face_Neutral", Color.black);
                    av.WithAccessoryLayer("Avatar/Accessories/FacialHair/Chevron/Chevron", av.HairColor);

                    // Young downtown — same-age peer look; not paired with Maris’s older “mentor” vibe
                    av.WithBodyLayer("Avatar/Layers/Top/RolledButtonUp", new Color(0.30f, 0.34f, 0.38f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.22f, 0.24f, 0.30f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/OpenVest/OpenVest", new Color(0.26f, 0.22f, 0.20f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.14f, 0.14f, 0.16f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(500f, 900f)
                        .WithOrdersPerWeek(1, 3)
                        .WithPreferredOrderDay(Day.Monday)
                        .WithOrderTime(2015)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(0f, 1f)
                        .WithCallPoliceChance(0.24f)
                        .WithDependence(0.06f, 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, -0.12f),
                            (DrugType.Methamphetamine, -0.44f),
                            (DrugType.Shrooms, 0.82f),
                            (DrugType.Cocaine, 0.10f)
                        })
                        .WithPreferredProperties(Property.Focused, Property.Calming, Property.Refreshing);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("philip_wentworth", "maris_eldridge");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();

                    plan.Add(new StayInBuildingSpec { BuildingName = "HAM Legal", StartTime = 0910, DurationMinutes = 119 });
                    plan.StayInBuilding(cafe, 1210, 94);
                    plan.UseATM(1405);
                    plan.StayInBuilding(supermarket, 1505, 119);
                    plan.Add(new WalkToSpec { Destination = townCenter, StartTime = 1745, FaceDestinationDirection = true });
                    plan.StayInBuilding(slopShop, 1845, 149);
                    plan.Add(new StayInBuildingSpec { BuildingName = "Apartment Building 2", StartTime = 2115, DurationMinutes = 714 });
                });
        }

        public CalderWren() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 0.37f;
                Region = Region.Downtown;

                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"CalderWren OnCreated failed: {ex.Message}");
            }
        }
    }
}
