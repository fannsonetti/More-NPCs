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
    public sealed class SharonWebster : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var websterHouse = Building.Get<WebsterHouse>();
            var westGasMart = Building.Get<WestGasMart>();
            Vector3 spawnPos = new Vector3(-108.2f, -2.835f, 60.8f);
            Vector3 busStop = new Vector3(-13.0495f, 1.065f, 95.5169f);

            builder.WithIdentity("sharon_webster", "Sharon", "Webster")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.92f;
                    av.Height = 0.99f;
                    av.Weight = 0.40f;
                    av.SkinColor = new Color(0.58f, 0.47f, 0.40f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.95f, 0.89f, 0.85f);
                    av.PupilDilation = 0.63f;
                    av.EyebrowScale = 0.94f;
                    av.EyebrowThickness = 0.84f;
                    av.EyebrowRestingHeight = -0.12f;
                    av.EyebrowRestingAngle = 1.7f;
                    av.LeftEye = (0.29f, 0.40f);
                    av.RightEye = (0.29f, 0.40f);
                    av.HairColor = new Color(0.48f, 0.47f, 0.45f);
                    av.HairPath = "Avatar/Hair/lowbun/LowBun";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Neutral", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/OldPersonWrinkles", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/Buttonup", new Color(0.48f, 0.44f, 0.40f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.28f, 0.30f, 0.34f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Flats/Flats", new Color(0.21f, 0.21f, 0.23f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/CollarJacket/CollarJacket", new Color(0.26f, 0.30f, 0.34f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(350f, 650f)
                        .WithOrdersPerWeek(1, 3)
                        .WithPreferredOrderDay(Day.Tuesday)
                        .WithOrderTime(1830)
                        .WithStandards(CustomerStandard.Low)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(0f, 1f)
                        .WithCallPoliceChance(0.15f)
                        .WithDependence(0.18f, 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.22f), (DrugType.Methamphetamine, -0.16f), (DrugType.Shrooms, 0.10f), (DrugType.Cocaine, 0.31f)
                        })
                        .WithPreferredProperties(Property.CalorieDense, Property.Paranoia, Property.AntiGravity);
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
                    plan.StayInBuilding(websterHouse, 0706, 164);
                    plan.Add(new SitSpec { SeatSetPath = "Map/Hyland Point/Region_Westville/Slums Park/OutdoorBench", StartTime = 1021, DurationMinutes = 89 });
                    plan.StayInBuilding(westGasMart, 1151, 124);
                    plan.UseVendingMachine(1441);
                    plan.StayInBuilding(websterHouse, 1636, 799);
                });
        }

        public SharonWebster() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.47f;
                Region = Region.Westville;
                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"SharonWebster OnCreated failed: {ex.Message}");
            }
        }
    }
}

