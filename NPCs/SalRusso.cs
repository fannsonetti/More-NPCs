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
    /// Sal Russo - Docks dockworker. Heavier build, longer hair. Single connection: mack (above-ground docks).
    /// </summary>
    public sealed class SalRusso : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var docksIndustrial = Building.Get<DocksIndustrialBuilding>();
            var fishWarehouse = Building.Get<FishWarehouse>();
            var randysBait = Building.Get<RandysBaitTackle>();
            var hylandBank = Building.Get<HylandBank>();
            Vector3 spawnPos = new Vector3(-65.5f, -1.535f, -32f);
            builder.WithIdentity("sal_russo", "Sal", "Russo")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 1.02f;
                    av.Weight = 0.85f;
                    av.SkinColor = new Color(0.61f, 0.49f, 0.40f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(1.0f, 0.88f, 0.85f);
                    av.PupilDilation = 0.72f;
                    av.EyebrowScale = 1.2f;
                    av.EyebrowThickness = 1.7f;
                    av.EyebrowRestingHeight = -0.25f;
                    av.EyebrowRestingAngle = 2.2f;
                    av.LeftEye = (0.24f, 0.31f);
                    av.RightEye = (0.24f, 0.31f);
                    av.HairColor = new Color(0.19f, 0.14f, 0.10f);
                    av.HairPath = "Avatar/Hair/Shoulderlength/ShoulderLength";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Neutral", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Stubble", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/FlannelButtonUp", new Color(0.22f, 0.28f, 0.38f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.24f, 0.22f, 0.20f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.45f, 0.35f, 0.28f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/Belt/Belt", new Color(0.32f, 0.26f, 0.20f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 700f, maxWeekly: 1000f)
                        .WithOrdersPerWeek(1, 4)
                        .WithPreferredOrderDay(Day.Tuesday)
                        .WithOrderTime(1930)
                        .WithStandards(CustomerStandard.Low)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.08f)
                        .WithDependence(baseAddiction: 0.15f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.22f), (DrugType.Methamphetamine, 0.31f), (DrugType.Shrooms, -0.41f), (DrugType.Cocaine, -0.18f)
                        })
                        .WithPreferredProperties(Property.Calming, Property.Munchies, Property.Paranoia);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("marco_boon", "sherman_giles");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(docksIndustrial, 705, 113);
                    plan.UseATM(855);
                    plan.StayInBuilding(fishWarehouse, 915, 124);
                    plan.StayInBuilding(randysBait, 1075, 87);
                    plan.UseVendingMachine(1202);
                    plan.StayInBuilding(docksIndustrial, 1232, 148);
                    plan.StayInBuilding(fishWarehouse, 1417, 118);
                    plan.StayInBuilding(hylandBank, 1572, 95);
                    plan.StayInBuilding(docksIndustrial, 1704, 635);
                });
        }

        public SalRusso() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 5f;
                Region = S1API.Map.Region.Docks;

                Schedule.Enable();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"SalRusso OnCreated failed: {ex.Message}");
                MelonLogger.Error($"StackTrace: {ex.StackTrace}");
            }
        }
    }
}
