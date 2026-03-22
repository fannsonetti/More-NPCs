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
    /// Trisha Morrow - Female Westville regular who lives in Room 3. Connected to Shirley Watts.
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
                    av.EyeBallTint = new Color(0.96f, 0.90f, 0.86f);
                    av.PupilDilation = 0.66f;
                    av.EyebrowScale = 0.92f;
                    av.EyebrowThickness = 0.78f;
                    av.EyebrowRestingHeight = -0.08f;
                    av.EyebrowRestingAngle = 1.9f;
                    av.LeftEye = (0.27f, 0.39f);
                    av.RightEye = (0.27f, 0.39f);
                    av.HairColor = new Color(0.24f, 0.19f, 0.14f);
                    av.HairPath = "Avatar/Hair/SidePartBob/SidePartBob";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_NeutralPout", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/Freckles", new Color(0.42f, 0.28f, 0.22f));
                    av.WithBodyLayer("Avatar/Layers/Top/V-Neck", new Color(0.30f, 0.35f, 0.45f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.24f, 0.25f, 0.28f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Flats/Flats", new Color(0.17f, 0.18f, 0.22f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/CollarJacket/CollarJacket", new Color(0.19f, 0.24f, 0.31f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/RectangleFrameGlasses/RectangleFrameGlasses", new Color(0.15f, 0.15f, 0.17f));
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
                        .WithCallPoliceChance(0.02f)
                        .WithDependence(0.28f, 1f)
                        .WithAffinities(new[] { (DrugType.Marijuana, 0.22f), (DrugType.Methamphetamine, 0.94f), (DrugType.Shrooms, -0.35f), (DrugType.Cocaine, -0.18f) })
                        .WithPreferredProperties(Property.Calming, Property.Euphoric, Property.ThoughtProvoking);
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
                    plan.StayInBuilding(room3, 0, 554);
                    plan.StayInBuilding(cornerStore, 930, 89);
                    plan.Add(new WalkToSpec { Destination = busStop, StartTime = 1145, FaceDestinationDirection = true });
                    plan.StayInBuilding(thePissHut, 1310, 99);
                    plan.StayInBuilding(arcade, 1450, 109);
                    plan.UseVendingMachine(1645);
                    plan.StayInBuilding(room3, 1810, 124);
                    plan.StayInBuilding(room3, 2110, 629);
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
