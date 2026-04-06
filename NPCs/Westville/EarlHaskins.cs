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
    /// Earl Haskins - older rough-around-the-edges Westville customer connected to Dean Webster.
    /// </summary>
    public sealed class EarlHaskins : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var westGasMart = Building.Get<WestGasMart>();
            var arcade = Building.Get<Arcade>();
            Vector3 oldCourt = new Vector3(-92.6f, 1.065f, 61.3f);
            Vector3 busStop = new Vector3(-13.0495f, 1.065f, 95.5169f);
            Vector3 spawnPos = new Vector3(-84.5f, 1.065f, 54.2f);

            builder.WithIdentity("earl_haskins", "Earl", "Haskins")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.32f;
                    av.Height = 1.02f;
                    av.Weight = 1.23f;
                    av.SkinColor = new Color(0.80f, 0.70f, 0.62f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.96f, 0.87f, 0.83f);
                    av.PupilDilation = 0.62f;
                    av.EyebrowScale = 1.14f;
                    av.EyebrowThickness = 0.98f;
                    av.EyebrowRestingHeight = -0.34f;
                    av.EyebrowRestingAngle = 3.40f;
                    av.LeftEye = (0.23f, 0.41f);
                    av.RightEye = (0.23f, 0.41f);
                    av.HairColor = new Color(0.50f, 0.49f, 0.48f);
                    av.HairPath = "Avatar/Hair/Receding/Receding";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Agitated", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/OldPersonWrinkles", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/TiredEyes", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Goatee", new Color(0.64f, 0.64f, 0.64f));
                    av.WithBodyLayer("Avatar/Layers/Top/UpperBodyTattoos", new Color(0.14f, 0.14f, 0.14f));
                    av.WithBodyLayer("Avatar/Layers/Top/Nipples", new Color(0.48f, 0.33f, 0.24f));
                    av.WithBodyLayer("Avatar/Layers/Top/ChestHair1", new Color(0.10f, 0.10f, 0.10f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.20f, 0.22f, 0.26f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(0.18f, 0.18f, 0.18f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(500f, 700f)
                        .WithOrdersPerWeek(1, 3)
                        .WithPreferredOrderDay(Day.Monday)
                        .WithOrderTime(1740)
                        .WithStandards(CustomerStandard.Low)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(0.00f, 1.00f)
                        .WithCallPoliceChance(0.15f)
                        .WithDependence(0.08f, 1.00f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.24f),
                            (DrugType.Methamphetamine, 0.08f),
                            (DrugType.Shrooms, -0.18f),
                            (DrugType.Cocaine, 0.30f)
                        })
                        .WithPreferredProperties(Property.Energizing, Property.Sedating, Property.Smelly);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("dean_webster", "marlene_haskins");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(westGasMart, 0749, 104);
                    plan.Add(new WalkToSpec { Destination = oldCourt, StartTime = 0934, FaceDestinationDirection = true });
                    plan.StayInBuilding(arcade, 1154, 124);
                    plan.UseATM(1534);
                    plan.Add(new SitSpec { SeatSetPath = "Map/Hyland Point/Region_Westville/Encampment/OutdoorBench", StartTime = 1724, DurationMinutes = 139 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Room 6", StartTime = 1904, DurationMinutes = 764 });
                });
        }

        public EarlHaskins() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.54f;
                Region = Region.Westville;
                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"EarlHaskins OnCreated failed: {ex.Message}");
            }
        }
    }
}


