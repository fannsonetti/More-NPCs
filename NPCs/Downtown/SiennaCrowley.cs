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
    /// Sienna Crowley - Downtown professional (classy outfit). Schedule uses placeholder buildings; update when provided.
    /// </summary>
    public sealed class SiennaCrowley : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var northOverpass = Building.Get<SouthOverpassBuilding>();

            Vector3 spawnPos = new Vector3(134.1605f, 6.0623f, 114.3804f);

            builder.WithIdentity("sienna_crowley", "Sienna", "Crowley")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.92f;
                    av.Height = 1.01f;
                    av.Weight = 0.35f;
                    av.SkinColor = new Color(0.72f, 0.59f, 0.49f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.99f, 0.91f, 0.88f);
                    av.PupilDilation = 0.77f;
                    av.EyebrowScale = 0.96f;
                    av.EyebrowThickness = 0.82f;
                    av.EyebrowRestingHeight = -0.03f;
                    av.EyebrowRestingAngle = 2.1f;
                    av.LeftEye = (0.30f, 0.42f);
                    av.RightEye = (0.30f, 0.42f);
                    av.HairColor = new Color(0.38f, 0.24f, 0.18f);
                    av.HairPath = "Avatar/Hair/HighBun/HighBun";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/FlannelButtonUp", new Color(0.46f, 0.39f, 0.33f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.25f, 0.26f, 0.30f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/DressShoes/DressShoes", new Color(0.18f, 0.18f, 0.20f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/Blazer/Blazer", new Color(0.27f, 0.29f, 0.33f));
                    av.WithAccessoryLayer("Avatar/Accessories/Neck/GoldChain/GoldChain", new Color(0.76f, 0.67f, 0.34f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(500f, 700f)
                        .WithOrdersPerWeek(1, 4)
                        .WithPreferredOrderDay(Day.Friday)
                        .WithOrderTime(1900)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(0f, 1f)
                        .WithCallPoliceChance(0.20f)
                        .WithDependence(0.11f, 1f)
                        .WithAffinities(new[] { (DrugType.Marijuana, 0.18f), (DrugType.Methamphetamine, 0.24f), (DrugType.Shrooms, 0.34f), (DrugType.Cocaine, -0.21f) })
                        .WithPreferredProperties(Property.Euphoric, Property.Refreshing, Property.Sedating);
                })
                .WithRelationshipDefaults(r => r.WithDelta(2.0f).SetUnlocked(false).WithConnectionsById("lucy_pennington", "jennifer_rivera").SetUnlockType(NPCRelationship.UnlockType.DirectApproach))
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new StayInBuildingSpec { BuildingName = "Small Tower", StartTime = 0851, DurationMinutes = 109 });
                    plan.Add(new SitSpec { SeatSetPath = "@Businesses/Taco Ticklers/Fast Food Booth (3)/fast food booth/Seat (1)", StartTime = 1141, DurationMinutes = 309 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Cafe", StartTime = 1451, DurationMinutes = 99 });
                    plan.UseVendingMachine(1721);
                    plan.StayInBuilding(northOverpass, 2001, 494);
                });
        }

        public SiennaCrowley() : base() { }

        protected override void OnCreated()
        {
            try { base.OnCreated(); Appearance.Build(); Aggressiveness = 0.50f; Region = Region.Downtown; Schedule.Enable(); }
            catch (System.Exception ex) { MelonLogger.Error($"SiennaCrowley OnCreated failed: {ex.Message}"); }
        }
    }
}

