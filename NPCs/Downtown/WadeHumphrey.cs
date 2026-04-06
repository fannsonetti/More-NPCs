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
    public sealed class WadeHumphrey : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            Vector3 spawnPos = new Vector3(8.2f, 0.28f, -4.1f);

            builder.WithIdentity("wade_humphrey", "Wade", "Humphrey")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 1.06f;
                    av.Weight = 0.52f;
                    av.SkinColor = new Color(0.48f, 0.38f, 0.32f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.94f, 0.88f, 0.84f);
                    av.PupilDilation = 0.55f;
                    av.EyebrowScale = 1.1f;
                    av.EyebrowThickness = 1.05f;
                    av.EyebrowRestingHeight = -0.12f;
                    av.EyebrowRestingAngle = 0.32f;
                    av.LeftEye = (0.30f, 0.40f);
                    av.RightEye = (0.30f, 0.40f);
                    av.HairColor = new Color(0.2f, 0.15f, 0.1f);
                    av.HairPath = "Avatar/Hair/Receding/Receding";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/RolledButtonUp", new Color(0.36f, 0.40f, 0.46f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.20f, 0.21f, 0.25f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/DressShoes/DressShoes", new Color(0.12f, 0.12f, 0.13f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(500f, 1000f)
                        .WithOrdersPerWeek(1, 3)
                        .WithPreferredOrderDay(Day.Wednesday)
                        .WithOrderTime(1830)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(0f, 1f)
                        .WithCallPoliceChance(0.17f)
                        .WithDependence(0.08f, 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.14f), (DrugType.Methamphetamine, 0.28f), (DrugType.Shrooms, -0.20f), (DrugType.Cocaine, 0.32f)
                        })
                        .WithPreferredProperties(Property.Sneaky, Property.Euphoric, Property.Focused);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .WithConnectionsById("philip_wentworth", "calder_wren")
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach);
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new StayInBuildingSpec { BuildingName = "BodyShop Office", StartTime = 0905, DurationMinutes = 120 });
                    plan.Add(new SitSpec { SeatSetPath = "Map/Hyland Point/Region_Downtown/TownCenter/OutdoorBench (1)", StartTime = 1115, DurationMinutes = 294 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "The Crimson Canary", StartTime = 1410, DurationMinutes = 149 });
                    plan.UseATM(1805);
                    plan.Add(new StayInBuildingSpec { BuildingName = "Apartment Building 2", StartTime = 2035, DurationMinutes = 794 });
                });
        }

        public WadeHumphrey() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.49f;
                Region = Region.Downtown;
                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"WadeHumphrey OnCreated failed: {ex.Message}");
            }
        }
    }
}
