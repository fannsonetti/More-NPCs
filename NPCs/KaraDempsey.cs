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
    /// Kara Dempsey - Westville local tied to Rory and Sarah.
    /// </summary>
    public sealed class KaraDempsey : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var cornerStore = Building.Get<CornerStore>();
            var arcade = Building.Get<Arcade>();
            var thePissHut = Building.Get<ThePissHut>();
            Vector3 spawnPos = new Vector3(-118.3270f, -2.8350f, 66.4780f);
            Vector3 courtyard = new Vector3(-87.7440f, 1.0650f, 57.2130f);

            builder.WithIdentity("kara_dempsey", "Kara", "Dempsey")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.84f;
                    av.Height = 0.97f;
                    av.Weight = 0.33f;
                    av.SkinColor = new Color(0.63f, 0.51f, 0.42f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.98f, 0.90f, 0.86f);
                    av.PupilDilation = 0.63f;
                    av.EyebrowScale = 0.88f;
                    av.EyebrowThickness = 0.76f;
                    av.EyebrowRestingHeight = -0.04f;
                    av.EyebrowRestingAngle = 1.55f;
                    av.LeftEye = (0.27f, 0.41f);
                    av.RightEye = (0.27f, 0.41f);
                    av.HairColor = new Color(0.18f, 0.14f, 0.11f);
                    av.HairPath = "Avatar/Hair/DoubleTopKnot/DoubleTopKnot";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/TiredEyes", new Color(0.10f, 0.09f, 0.09f));
                    av.WithFaceLayer("Avatar/Layers/Face/Freckles", new Color(0.40f, 0.30f, 0.24f));
                    av.WithBodyLayer("Avatar/Layers/Top/RolledButtonUp", new Color(0.30f, 0.37f, 0.45f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jorts", new Color(0.28f, 0.29f, 0.34f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sandals/Sandals", new Color(0.24f, 0.22f, 0.20f));
                    av.WithAccessoryLayer("Avatar/Accessories/Neck/GoldChain/GoldChain", new Color(0.70f, 0.61f, 0.33f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/SmallRoundGlasses/SmallRoundGlasses", new Color(0.16f, 0.16f, 0.18f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(400f, 600f)
                        .WithOrdersPerWeek(1, 3)
                        .WithPreferredOrderDay(Day.Wednesday)
                        .WithOrderTime(1940)
                        .WithStandards(CustomerStandard.Low)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(0f, 1f)
                        .WithCallPoliceChance(0.14f)
                        .WithDependence(0.16f, 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.44f), (DrugType.Methamphetamine, 0.06f), (DrugType.Shrooms, 0.18f), (DrugType.Cocaine, -0.12f)
                        })
                        .WithPreferredProperties(Property.Calming, Property.Refreshing, Property.Foggy);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("rory_dempsey", "sarah_greene");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(arcade, 0705, 149);
                    plan.Add(new WalkToSpec { Destination = courtyard, StartTime = 1050, FaceDestinationDirection = true });
                    plan.StayInBuilding(cornerStore, 1220, 89);
                    plan.UseVendingMachine(1405);
                    plan.StayInBuilding(thePissHut, 1640, 89);
                    plan.Add(new StayInBuildingSpec { BuildingName = "Room 5", StartTime = 1840, DurationMinutes = 744 });
                });
        }

        public KaraDempsey() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.43f;
                Region = Region.Westville;
                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"KaraDempsey OnCreated failed: {ex.Message}");
            }
        }
    }
}
