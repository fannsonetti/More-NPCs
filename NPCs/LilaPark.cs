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
    public sealed class LilaPark : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var cornerStore = Building.Get<CornerStore>();
            var sauerkrautSupreme = Building.Get<SauerkrautSupreme>();
            Vector3 spawnPos = new Vector3(-96.6140f, -2.8350f, 58.3390f);
            Vector3 busStop = new Vector3(-13.0495f, 1.065f, 95.5169f);

            builder.WithIdentity("lila_park", "Lila", "Park")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.90f;
                    av.Height = 0.97f;
                    av.Weight = 0.34f;
                    av.SkinColor = new Color(0.66f, 0.53f, 0.44f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.97f, 0.90f, 0.86f);
                    av.PupilDilation = 0.70f;
                    av.EyebrowScale = 0.92f;
                    av.EyebrowThickness = 0.80f;
                    av.EyebrowRestingHeight = -0.10f;
                    av.EyebrowRestingAngle = 1.9f;
                    av.LeftEye = (0.30f, 0.41f);
                    av.RightEye = (0.30f, 0.41f);
                    av.HairColor = new Color(0.26f, 0.18f, 0.12f);
                    av.HairPath = "Avatar/Hair/SidePartBob/SidePartBob";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/ButtonUp", new Color(0.53f, 0.45f, 0.39f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.17f, 0.18f, 0.22f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.20f, 0.20f, 0.22f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/OpenVest/OpenVest", new Color(0.20f, 0.20f, 0.22f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(450f, 700f)
                        .WithOrdersPerWeek(1, 4)
                        .WithPreferredOrderDay(Day.Saturday)
                        .WithOrderTime(1820)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(0f, 1f)
                        .WithCallPoliceChance(0.19f)
                        .WithDependence(0.20f, 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.30f), (DrugType.Methamphetamine, -0.22f), (DrugType.Shrooms, 0.48f), (DrugType.Cocaine, 0.07f)
                        })
                        .WithPreferredProperties(Property.Sneaky, Property.Spicy, Property.BrightEyed);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("nora_kessler", "maxine_junefield");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(cornerStore, 0839, 109);
                    plan.Add(new SitSpec { SeatSetPath = "Map/Hyland Point/Region_Westville/OutdoorBench (1)", StartTime = 1039, DurationMinutes = 104 });
                    plan.StayInBuilding(sauerkrautSupreme, 1224, 124);
                    plan.UseATM(1454);
                    plan.UseVendingMachine(1644);
                    plan.Add(new StayInBuildingSpec { BuildingName = "Room 2", StartTime = 1854, DurationMinutes = 720 });
                });
        }

        public LilaPark() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.41f;
                Region = Region.Westville;
                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"LilaPark OnCreated failed: {ex.Message}");
            }
        }
    }
}


