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
    /// Trisha Morrow - Westville methhead connected to Shirley Watts.
    /// </summary>
    public sealed class Trisha : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var room3 = Building.Get<Room3>();
            var cornerStore = Building.Get<CornerStore>();
            var thePissHut = Building.Get<ThePissHut>();
            var arcade = Building.Get<Arcade>();
            Vector3 spawnPos = new Vector3(-113.1828f, -2.835f, 61.2241f);
            Vector3 busStop = new Vector3(-13.0495f, 1.065f, 95.5169f);

            builder.WithIdentity("trisha_morrow", "Trisha", "Morrow")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 1.0f;
                    av.Height = 0.99f;
                    av.Weight = 0.36f;
                    av.SkinColor = new Color(0.62f, 0.51f, 0.42f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.9018f, 0.74f, 0.74f);
                    av.PupilDilation = 0.28f;
                    av.EyebrowScale = 0.92f;
                    av.EyebrowThickness = 0.78f;
                    av.EyebrowRestingHeight = -0.08f;
                    av.EyebrowRestingAngle = 1.9f;
                    av.LeftEye = (0.42f, 0.05f);
                    av.RightEye = (0.42f, 0.05f);
                    av.HairColor = new Color(0.24f, 0.19f, 0.14f);
                    av.HairPath = "Avatar/Hair/SidePartBob/SidePartBob";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Agitated", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/TiredEyes", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/EyeShadow", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/FaceTattoos", new Color(0.12f, 0.12f, 0.14f));
                    av.WithFaceLayer("Avatar/Layers/Face/Freckles", new Color(0.42f, 0.28f, 0.22f));
                    av.WithBodyLayer("Avatar/Layers/Top/T-Shirt", new Color(0.31f, 0.30f, 0.33f));
                    av.WithBodyLayer("Avatar/Layers/Top/UpperBodyTattoos", new Color(0.10f, 0.09f, 0.08f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/FemaleUnderwear", new Color(0.72f, 0.67f, 0.64f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/OpenVest/OpenVest", new Color(0.18f, 0.19f, 0.21f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.15f, 0.15f, 0.17f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(200f, 500f)
                        .WithOrdersPerWeek(1, 3)
                        .WithPreferredOrderDay(Day.Wednesday)
                        .WithOrderTime(2030)
                        .WithStandards(CustomerStandard.Low)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(0f, 1f)
                        .WithCallPoliceChance(0.16f)
                        .WithDependence(0.28f, 1f)
                        .WithAffinities(new[] { (DrugType.Marijuana, 0.22f), (DrugType.Methamphetamine, 0.94f), (DrugType.Shrooms, -0.35f), (DrugType.Cocaine, -0.18f) })
                        .WithPreferredProperties(Property.Energizing, Property.Smelly, Property.Paranoia);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("shirley_watts");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(cornerStore, 0940, 89);
                    plan.Add(new SitSpec { SeatSetPath = "Map/Hyland Point/Region_Westville/OutdoorBench (1)", StartTime = 1155, DurationMinutes = 124 });
                    plan.StayInBuilding(thePissHut, 1320, 99);
                    plan.StayInBuilding(arcade, 1500, 109);
                    plan.UseVendingMachine(1655);
                    plan.StayInBuilding(room3, 1820, 809);
                });
        }

        public Trisha() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.68f;
                Region = Region.Westville;
                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"Trisha OnCreated failed: {ex.Message}");
            }
        }
    }
}


