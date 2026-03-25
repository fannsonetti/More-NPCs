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
    public sealed class MarleneHaskins : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var westGasMart = Building.Get<WestGasMart>();
            var arcade = Building.Get<Arcade>();
            Vector3 spawnPos = new Vector3(-83.1f, 1.065f, 56.0f);
            Vector3 oldCourt = new Vector3(-92.6f, 1.065f, 61.3f);

            builder.WithIdentity("marlene_haskins", "Marlene", "Haskins")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.88f;
                    av.Height = 0.98f;
                    av.Weight = 0.46f;
                    av.SkinColor = new Color(0.74f, 0.63f, 0.55f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.95f, 0.88f, 0.84f);
                    av.PupilDilation = 0.66f;
                    av.EyebrowScale = 0.98f;
                    av.EyebrowThickness = 0.90f;
                    av.EyebrowRestingHeight = -0.16f;
                    av.EyebrowRestingAngle = 1.7f;
                    av.LeftEye = (0.28f, 0.40f);
                    av.RightEye = (0.28f, 0.40f);
                    av.HairColor = new Color(0.46f, 0.45f, 0.44f);
                    av.HairPath = "Avatar/Hair/MidFringe/MidFringe";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/OldPersonWrinkles", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/RolledButtonUp", new Color(0.58f, 0.68f, 0.82f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.31f, 0.33f, 0.39f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/SmallRoundGlasses/SmallRoundGlasses", new Color(0.16f, 0.16f, 0.17f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Flats/Flats", new Color(0.22f, 0.22f, 0.24f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(450f, 650f)
                        .WithOrdersPerWeek(1, 3)
                        .WithPreferredOrderDay(Day.Friday)
                        .WithOrderTime(1810)
                        .WithStandards(CustomerStandard.Low)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(0f, 1f)
                        .WithCallPoliceChance(0.15f)
                        .WithDependence(0.10f, 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.18f), (DrugType.Methamphetamine, -0.26f), (DrugType.Shrooms, 0.34f), (DrugType.Cocaine, 0.09f)
                        })
                        .WithPreferredProperties(Property.Glowie, Property.Focused, Property.ThoughtProvoking);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("earl_haskins");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(westGasMart, 800, 94);
                    plan.Add(new WalkToSpec { Destination = oldCourt, StartTime = 955, FaceDestinationDirection = true });
                    plan.StayInBuilding(arcade, 1130, 114);
                    plan.UseATM(1500);
                    plan.Add(new StayInBuildingSpec { BuildingName = "Room 6", StartTime = 1830, DurationMinutes = 780 });
                });
        }

        public MarleneHaskins() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.44f;
                Region = Region.Westville;
                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"MarleneHaskins OnCreated failed: {ex.Message}");
            }
        }
    }
}
