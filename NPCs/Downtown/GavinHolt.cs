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
    /// <summary>
    /// Gavin Holt - Downtown professional (classy outfit). Schedule uses placeholder buildings; update when provided.
    /// </summary>
    public sealed class GavinHolt : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            Vector3 spawnPos = new Vector3(134.1605f, 6.0623f, 114.3804f);

            builder.WithIdentity("gavin_holt", "Gavin", "Holt")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.10f;
                    av.Height = 1.02f;
                    av.Weight = 0.46f;
                    av.SkinColor = new Color(0.58f, 0.48f, 0.40f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.98f, 0.89f, 0.84f);
                    av.PupilDilation = 0.71f;
                    av.EyebrowScale = 1.10f;
                    av.EyebrowThickness = 1.20f;
                    av.EyebrowRestingHeight = -0.24f;
                    av.EyebrowRestingAngle = 2.8f;
                    av.LeftEye = (0.28f, 0.38f);
                    av.RightEye = (0.28f, 0.38f);
                    av.HairColor = new Color(0.13f, 0.10f, 0.08f);
                    av.HairPath = "Avatar/Hair/Receding/Receding";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SmugPout", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/FlannelButtonUp", new Color(0.28f, 0.24f, 0.20f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.20f, 0.20f, 0.22f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(0.12f, 0.12f, 0.12f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/OpenVest/OpenVest", new Color(0.22f, 0.22f, 0.24f));
                    av.WithAccessoryLayer("Avatar/Accessories/Hands/Polex/Polex", new Color(0.63f, 0.63f, 0.65f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(400f, 600f)
                        .WithOrdersPerWeek(1, 3)
                        .WithPreferredOrderDay(Day.Sunday)
                        .WithOrderTime(2145)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(0f, 1f)
                        .WithCallPoliceChance(0.20f)
                        .WithDependence(0.21f, 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.10f), (DrugType.Methamphetamine, 0.63f), (DrugType.Shrooms, -0.20f), (DrugType.Cocaine, -0.26f)
                        })
                        .WithPreferredProperties(Property.Energizing, Property.Smelly, Property.Sneaky);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .WithConnectionsById("bruce_norton", "jeff_gilmore")
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach);
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new StayInBuildingSpec { BuildingName = "HAM Legal", StartTime = 0921, DurationMinutes = 119 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Cafe", StartTime = 1221, DurationMinutes = 94 });
                    plan.UseATM(1416);
                    plan.Add(new StayInBuildingSpec { BuildingName = "Supermarket", StartTime = 1651, DurationMinutes = 89 });
                    plan.Add(new SitSpec { SeatSetPath = "Map/Hyland Point/Region_Downtown/Diner/Round Outdoor Set/Outdoor chair (2)", StartTime = 1921, DurationMinutes = 224 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Nightclub", StartTime = 2146, DurationMinutes = 299 });
                });
        }

        public GavinHolt() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.54f;
                Region = Region.Downtown;
                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"GavinHolt OnCreated failed: {ex.Message}");
            }
        }
    }
}

