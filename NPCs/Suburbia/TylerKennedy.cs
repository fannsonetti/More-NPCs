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
    /// <summary>Kennedy teen (15) — blonde like parents, suburban school/errand loop.</summary>
    public sealed class TylerKennedy : NPC
    {
        public override bool IsPhysical => true;

        private static readonly Color KidHair = new Color(0.6057f, 0.5035f, 0.2807f);
        private static readonly Color KidSkin = new Color(0.807f, 0.646f, 0.494f);

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var supermarket = Building.Get<Supermarket>();
            var cafe = Building.Get<Cafe>();
            Vector3 park = new Vector3(71.0f, 4.8614f, -93.2f);
            Vector3 spawnPos = new Vector3(72.8f, 4.935f, -95.1f);

            builder.WithIdentity("tyler_kennedy", "Tyler", "Kennedy")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.15f;
                    av.Height = 0.97f;
                    av.Weight = 0.34f;
                    av.SkinColor = KidSkin;
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.95f, 0.91f, 0.87f);
                    av.PupilDilation = 0.72f;
                    av.EyebrowScale = 0.98f;
                    av.EyebrowThickness = 0.86f;
                    av.EyebrowRestingHeight = -0.05f;
                    av.EyebrowRestingAngle = 1.25f;
                    av.LeftEye = (0.30f, 0.40f);
                    av.RightEye = (0.30f, 0.40f);
                    av.HairColor = KidHair;
                    av.HairPath = "Avatar/Hair/MidFringe/MidFringe";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/Tucked T-Shirt", new Color(0.38f, 0.42f, 0.48f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/OpenVest/OpenVest", new Color(0.24f, 0.26f, 0.30f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.22f, 0.24f, 0.29f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.18f, 0.18f, 0.20f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 400f, maxWeekly: 800f)
                        .WithOrdersPerWeek(1, 3)
                        .WithPreferredOrderDay(Day.Wednesday)
                        .WithOrderTime(1545)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.0f, maxAt100: 3.5f)
                        .WithCallPoliceChance(0.14f)
                        .WithDependence(baseAddiction: 0.10f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.52f), (DrugType.Methamphetamine, 0.18f), (DrugType.Shrooms, 0.08f), (DrugType.Cocaine, 0.24f)
                        })
                        .WithPreferredProperties(Property.Munchies, Property.Euphoric, Property.Sneaky);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("haley_kennedy", "emma_kennedy");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new StayInBuildingSpec { BuildingName = "Kennedy House", StartTime = 2248, DurationMinutes = 487 });
                    plan.UseVendingMachine(0656);
                    plan.Add(new WalkToSpec { Destination = park, StartTime = 0712, FaceDestinationDirection = true });
                    plan.Add(new SitSpec { SeatSetPath = "Map/Hyland Point/Region_Suburbia/Residential park/OutdoorBench (2)", StartTime = 0728, DurationMinutes = 78 });
                    plan.StayInBuilding(supermarket, 0847, 68);
                    plan.Add(new StayInBuildingSpec { BuildingName = "Small Tower", StartTime = 0956, DurationMinutes = 164 });
                    plan.StayInBuilding(cafe, 1241, 74);
                    plan.Add(new WalkToSpec { Destination = park, StartTime = 1316, FaceDestinationDirection = true });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Kennedy House", StartTime = 1425, DurationMinutes = 503 });
                });
        }

        public TylerKennedy() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.41f;
                Region = Region.Suburbia;
                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"TylerKennedy OnCreated failed: {ex.Message}");
            }
        }
    }
}
