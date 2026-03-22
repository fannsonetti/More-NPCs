using MelonLoader;
using S1API.Economy;
using S1API.Entities;
using S1API.Entities.Schedule;
using S1API.Entities.NPCs.Westville;
using S1API.GameTime;
using S1API.Map;
using S1API.Map.Buildings;
using S1API.Products;
using S1API.Properties;
using UnityEngine;

namespace MoreNPCs.NPCs
{
    /// <summary>
    /// Darla Crane - Westville methhead with tattoos. Connected to Dean Webster and Shirley Watts.
    /// </summary>
    public sealed class DarlaCrane : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var cornerStore = Building.Get<CornerStore>();
            var thePissHut = Building.Get<ThePissHut>();
            Vector3 spawnPos = new Vector3(-113.1828f, -2.835f, 61.2241f);
            Vector3 busStop = new Vector3(-13.0495f, 1.065f, 95.5169f);

            builder.WithIdentity("darla_crane", "Darla", "Crane")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 1.0f;
                    av.Height = 0.94f;
                    av.Weight = 0.32f;
                    av.SkinColor = new Color(0.56f, 0.46f, 0.38f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.96f, 0.86f, 0.78f);
                    av.PupilDilation = 0.92f;
                    av.EyebrowScale = 0.98f;
                    av.EyebrowThickness = 0.72f;
                    av.EyebrowRestingHeight = -0.28f;
                    av.EyebrowRestingAngle = 2.6f;
                    av.LeftEye = (0.30f, 0.20f);
                    av.RightEye = (0.30f, 0.20f);
                    av.HairColor = new Color(0.18f, 0.14f, 0.10f);
                    av.HairPath = "Avatar/Hair/LowBun/LowBun";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Agitated", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/T-Shirt", new Color(0.35f, 0.30f, 0.32f));
                    av.WithBodyLayer("Avatar/Layers/Top/UpperBodyTattoos", new Color(0.10f, 0.08f, 0.06f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.20f, 0.20f, 0.22f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.14f, 0.14f, 0.16f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/Belt/Belt", new Color(0.14f, 0.14f, 0.16f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(250f, 550f)
                        .WithOrdersPerWeek(1, 3)
                        .WithPreferredOrderDay(Day.Thursday)
                        .WithOrderTime(1945)
                        .WithStandards(CustomerStandard.Low)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(0f, 1f)
                        .WithCallPoliceChance(0.04f)
                        .WithDependence(0.32f, 1f)
                        .WithAffinities(new[] { (DrugType.Marijuana, 0.28f), (DrugType.Methamphetamine, 0.91f), (DrugType.Shrooms, -0.42f), (DrugType.Cocaine, -0.12f) })
                        .WithPreferredProperties(Property.Energizing, Property.Smelly, Property.Paranoia);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("dean_webster", "shirley_watts");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new StayInBuildingSpec { BuildingName = "Jessi's Room", StartTime = 0, DurationMinutes = 479 });
                    plan.StayInBuilding(cornerStore, 800, 84);
                    plan.Add(new WalkToSpec { Destination = busStop, StartTime = 1015, FaceDestinationDirection = true });
                    plan.StayInBuilding(thePissHut, 1115, 104);
                    plan.UseATM(1335);
                    plan.StayInBuilding(cornerStore, 1510, 79);
                    plan.Add(new StayInBuildingSpec { BuildingName = "Jessi's Room", StartTime = 1700, DurationMinutes = 179 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Jessi's Room", StartTime = 2015, DurationMinutes = 704 });
                });
        }

        public DarlaCrane() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.72f;
                Region = Region.Westville;
                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"DarlaCrane OnCreated failed: {ex.Message}");
            }
        }
    }
}
