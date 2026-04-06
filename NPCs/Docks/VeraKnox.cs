using System;
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
    /// <summary>Vera Knox — dock worker; ties to Maya Webb and Salvador Moreno.</summary>
    public sealed class VeraKnox : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var docksIndustrial = Building.Get<DocksIndustrialBuilding>();
            var fishWarehouse = Building.Get<FishWarehouse>();
            var randysBait = Building.Get<RandysBaitTackle>();
            Vector3 warehousePier = new Vector3(-62.4702f, -1.5315f, 19.8399f);
            Vector3 spawnPos = new Vector3(-73.8f, -1.535f, -31.5f);

            builder.WithIdentity("vera_knox", "Vera", "Knox")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.78f;
                    av.Height = 0.99f;
                    av.Weight = 0.43f;
                    var skin = new Color(0.6f, 0.5f, 0.42f);
                    av.SkinColor = skin;
                    av.LeftEyeLidColor = skin;
                    av.RightEyeLidColor = skin;
                    av.EyeBallTint = new Color(0.95f, 0.9f, 0.86f);
                    av.PupilDilation = 0.7f;
                    av.EyebrowScale = 0.96f;
                    av.EyebrowThickness = 0.8f;
                    av.EyebrowRestingHeight = -0.12f;
                    av.EyebrowRestingAngle = 1.6f;
                    av.LeftEye = (0.28f, 0.34f);
                    av.RightEye = (0.28f, 0.34f);
                    av.HairColor = new Color(0.16f, 0.14f, 0.12f);
                    av.HairPath = "Avatar/Hair/Shoulderlength/ShoulderLength";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_NeutralPout", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/FlannelButtonUp", new Color(0.32f, 0.3f, 0.26f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.22f, 0.24f, 0.26f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(0.14f, 0.14f, 0.15f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/Beanie/Beanie", new Color(0.2f, 0.22f, 0.24f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 650f, maxWeekly: 940f)
                        .WithOrdersPerWeek(1, 4)
                        .WithPreferredOrderDay(Day.Monday)
                        .WithOrderTime(1830)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.2f)
                        .WithDependence(baseAddiction: 0.18f, dependenceMultiplier: 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.15f), (DrugType.Methamphetamine, 0.28f), (DrugType.Shrooms, 0.08f), (DrugType.Cocaine, 0.35f)
                        })
                        .WithPreferredProperties(Property.Focused, Property.Sneaky, Property.Euphoric);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("maya_webb", "salvador_moreno");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(fishWarehouse, 0718, 88);
                    plan.Add(new SitSpec { SeatSetPath = "Map/Hyland Point/Region_Suburbia/Residential park/OutdoorBench (2)", StartTime = 0906, DurationMinutes = 124 });
                    plan.StayInBuilding(docksIndustrial, 1110, 118);
                    plan.UseATM(1308);
                    plan.StayInBuilding(randysBait, 1328, 88);
                    plan.StayInBuilding(fishWarehouse, 1456, 104);
                    plan.StayInBuilding(docksIndustrial, 1620, 116);
                    plan.UseVendingMachine(1756);
                    plan.StayInBuilding(fishWarehouse, 1828, 118);
                    plan.Add(new WalkToSpec { Destination = warehousePier, StartTime = 1953, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 180f, 0f) * Vector3.forward });
                    plan.StayInBuilding(docksIndustrial, 1955, 640);
                });
        }

        public VeraKnox() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.44f;
                Region = Region.Docks;
                Schedule.Enable();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"VeraKnox OnCreated failed: {ex.Message}");
            }
        }
    }
}
