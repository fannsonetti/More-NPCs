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
    /// <summary>Olive Crank — sewer-side docks; clashing dock-rat colors (name only), chef hat, reads unwell.</summary>
    public sealed class OliveCrank : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var docksIndustrial = Building.Get<DocksIndustrialBuilding>();
            var fishWarehouse = Building.Get<FishWarehouse>();
            var randysBait = Building.Get<RandysBaitTackle>();
            var hylandBank = Building.Get<HylandBank>();
            Vector3 spawnPos = new Vector3(-86.2f, -1.485f, -38.4f);

            builder.WithIdentity("olive_crank", "Olive", "Crank")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 1.08f;
                    av.Height = 0.98f;
                    av.Weight = 0.44f;
                    var skin = new Color(0.6f, 0.49f, 0.42f);
                    av.SkinColor = skin;
                    av.LeftEyeLidColor = skin;
                    av.RightEyeLidColor = skin;
                    av.EyeBallTint = new Color(0.9f, 0.84f, 0.8f);
                    av.PupilDilation = 0.94f;
                    av.EyebrowScale = 1.12f;
                    av.EyebrowThickness = 0.72f;
                    av.EyebrowRestingHeight = -0.38f;
                    av.EyebrowRestingAngle = 5.2f;
                    av.LeftEye = (0.31f, 0.37f);
                    av.RightEye = (0.31f, 0.37f);
                    av.HairColor = new Color(0.14f, 0.11f, 0.09f);
                    av.HairPath = "Avatar/Hair/MessyBob/MessyBob";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Agitated", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/TiredEyes", new Color(0.06f, 0.05f, 0.05f, 0.55f));
                    av.WithFaceLayer("Avatar/Layers/Face/OldPersonWrinkles", new Color(0.2f, 0.18f, 0.16f, 0.35f));
                    av.WithBodyLayer("Avatar/Layers/Top/Tucked T-Shirt", new Color(0.4f, 0.3f, 0.36f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.44f, 0.28f, 0.22f));
                    av.WithBodyLayer("Avatar/Layers/Accessories/FingerlessGloves", new Color(0.26f, 0.24f, 0.22f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/OpenVest/OpenVest", new Color(0.2f, 0.21f, 0.24f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(0.1f, 0.1f, 0.11f));
                    av.WithAccessoryLayer("Avatar/Accessories/Neck/GoldChain/GoldChain", new Color(0.52f, 0.52f, 0.56f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/ChefHat/ChefHat", new Color(0.9f, 0.88f, 0.86f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 640f, maxWeekly: 920f)
                        .WithOrdersPerWeek(1, 4)
                        .WithPreferredOrderDay(Day.Thursday)
                        .WithOrderTime(2030)
                        .WithStandards(CustomerStandard.Low)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.16f)
                        .WithDependence(baseAddiction: 0.19f, dependenceMultiplier: 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.08f), (DrugType.Methamphetamine, 0.62f), (DrugType.Shrooms, -0.15f), (DrugType.Cocaine, 0.28f)
                        })
                        .WithPreferredProperties(Property.Paranoia, Property.Energizing, Property.Sneaky);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("kaela_thorn", "sable_reed");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(fishWarehouse, 0702, 96);
                    plan.StayInBuilding(docksIndustrial, 0836, 118);
                    plan.Add(new SitSpec { SeatSetPath = "Map/Hyland Point/Region_Suburbia/Residential park/OutdoorBench", StartTime = 1014, DurationMinutes = 154 });
                    plan.UseATM(1208);
                    plan.StayInBuilding(randysBait, 1248, 92);
                    plan.StayInBuilding(hylandBank, 1420, 88);
                    plan.StayInBuilding(fishWarehouse, 1548, 134);
                    plan.UseVendingMachine(1750);
                    plan.StayInBuilding(docksIndustrial, 1820, 142);
                    plan.StayInBuilding(randysBait, 1922, 112);
                    plan.StayInBuilding(docksIndustrial, 2034, 627);
                });
        }

        public OliveCrank() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.58f;
                Region = Region.Docks;
                Schedule.Enable();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"OliveCrank OnCreated failed: {ex.Message}");
            }
        }
    }
}
