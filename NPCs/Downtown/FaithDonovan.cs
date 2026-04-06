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
    public sealed class FaithDonovan : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var cafe = Building.Get<Cafe>();
            Vector3 spawnPos = new Vector3(65.1f, 1.065f, 22.3f);

            builder.WithIdentity("faith_donovan", "Faith", "Donovan")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.92f;
                    av.Height = 0.97f;
                    av.Weight = 0.34f;
                    av.SkinColor = new Color(0.64f, 0.52f, 0.44f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.96f, 0.90f, 0.88f);
                    av.PupilDilation = 0.7f;
                    av.EyebrowScale = 0.93f;
                    av.EyebrowThickness = 0.76f;
                    av.EyebrowRestingHeight = -0.05f;
                    av.EyebrowRestingAngle = 2.25f;
                    av.LeftEye = (0.32f, 0.44f);
                    av.RightEye = (0.32f, 0.44f);
                    av.HairColor = new Color(0.32f, 0.20f, 0.14f);
                    av.HairPath = "Avatar/Hair/HighBun/HighBun";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Neutral", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/TiredEyes", new Color(0f, 0f, 0f, 0.12f));
                    av.WithBodyLayer("Avatar/Layers/Top/FlannelButtonUp", new Color(0.38f, 0.20f, 0.18f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.22f, 0.28f, 0.42f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(0.10f, 0.10f, 0.11f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(400f, 800f)
                        .WithOrdersPerWeek(1, 3)
                        .WithPreferredOrderDay(Day.Friday)
                        .WithOrderTime(2100)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(0f, 1f)
                        .WithCallPoliceChance(0.14f)
                        .WithDependence(0.07f, 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.40f), (DrugType.Methamphetamine, -0.24f), (DrugType.Shrooms, 0.30f), (DrugType.Cocaine, 0.08f)
                        })
                        .WithPreferredProperties(Property.Munchies, Property.Refreshing, Property.Glowie);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .WithConnectionsById("tessa_ward")
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach);
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new StayInBuildingSpec { BuildingName = "Les Ordures Puantes", StartTime = 1015, DurationMinutes = 119 });
                    plan.Add(new SitSpec { SeatSetPath = "@Businesses/Taco Ticklers/Fast Food Booth (3)/fast food booth/Seat", StartTime = 1255, DurationMinutes = 154 });
                    plan.StayInBuilding(cafe, 1410, 134);
                    plan.Add(new StayInBuildingSpec { BuildingName = "Slop Shop", StartTime = 1735, DurationMinutes = 164 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Apartment Building 2", StartTime = 2100, DurationMinutes = 729 });
                });
        }

        public FaithDonovan() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.38f;
                Region = Region.Downtown;
                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"FaithDonovan OnCreated failed: {ex.Message}");
            }
        }
    }
}
