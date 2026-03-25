using MelonLoader;
using S1API.Economy;
using S1API.Entities;
using S1API.Entities.Schedule;
using S1API.GameTime;
using S1API.Map;
using S1API.Products;
using S1API.Properties;
using UnityEngine;

namespace MoreNPCs.NPCs
{
    public sealed class BrentHalver : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            Vector3 spawnPos = new Vector3(-113.1828f, -2.835f, 61.2241f);
            Vector3 busStop = new Vector3(-13.0495f, 1.065f, 95.5169f);

            builder.WithIdentity("brent_halver", "Brent", "Halver")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.14f;
                    av.Height = 1.03f;
                    av.Weight = 0.50f;
                    av.SkinColor = new Color(0.70f, 0.58f, 0.49f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.98f, 0.90f, 0.86f);
                    av.PupilDilation = 0.70f;
                    av.EyebrowScale = 1.10f;
                    av.EyebrowThickness = 1.02f;
                    av.EyebrowRestingHeight = -0.20f;
                    av.EyebrowRestingAngle = 1.4f;
                    av.LeftEye = (0.29f, 0.39f);
                    av.RightEye = (0.29f, 0.39f);
                    av.HairColor = new Color(0.23f, 0.18f, 0.13f);
                    av.HairPath = "Avatar/Hair/BowlCut/BowlCut";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_NeutralPout", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/Tucked T-Shirt", new Color(0.41f, 0.37f, 0.29f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jorts", new Color(0.27f, 0.30f, 0.39f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sandals/Sandals", new Color(0.34f, 0.28f, 0.22f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/CollarJacket/CollarJacket", new Color(0.22f, 0.24f, 0.26f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/Cap/Cap", new Color(0.28f, 0.31f, 0.26f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(400f, 600f)
                        .WithOrdersPerWeek(2, 4)
                        .WithPreferredOrderDay(Day.Wednesday)
                        .WithOrderTime(2000)
                        .WithStandards(CustomerStandard.Low)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(0f, 1f)
                        .WithCallPoliceChance(0.14f)
                        .WithDependence(0.13f, 1f)
                        .WithAffinities(new[] { (DrugType.Marijuana, 0.52f), (DrugType.Methamphetamine, -0.08f), (DrugType.Shrooms, 0.10f), (DrugType.Cocaine, -0.25f) })
                        .WithPreferredProperties(Property.Munchies, Property.Calming, Property.Energizing);
                })
                .WithRelationshipDefaults(r => r.WithDelta(2.0f).SetUnlocked(false).SetUnlockType(NPCRelationship.UnlockType.DirectApproach).WithConnectionsById("george_greene", "charles_rowland"))
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new StayInBuildingSpec { BuildingName = "The Piss Hut", StartTime = 845, DurationMinutes = 99 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Corner Store", StartTime = 1045, DurationMinutes = 104 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Sauerkraut Supreme", StartTime = 1220, DurationMinutes = 119 });
                    plan.Add(new SitSpec { SeatSetPath = "Map/Hyland Point/Region_Westville/Corner Store/OutdoorBench", StartTime = 1400, DurationMinutes = 119 });
                    plan.UseATM(1620);
                    plan.Add(new StayInBuildingSpec { BuildingName = "Tool Shed", StartTime = 1920, DurationMinutes = 700 }); // until 7am, no 4am kickout
                });
        }

        public BrentHalver() : base() { }

        protected override void OnCreated()
        {
            try { base.OnCreated(); Appearance.Build(); Aggressiveness = 0.38f; Region = Region.Westville; Schedule.Enable(); }
            catch (System.Exception ex) { MelonLogger.Error($"BrentHalver OnCreated failed: {ex.Message}"); }
        }
    }
}


