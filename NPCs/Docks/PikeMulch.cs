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
    /// <summary>
    /// Pike Mulch — low-level docks connector between Mack, Anna, and Manhole Mike’s circles.
    /// </summary>
    public sealed class PikeMulch : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var fishWarehouse = Building.Get<FishWarehouse>();
            var hylandBank = Building.Get<HylandBank>();
            Vector3 spawnPos = new Vector3(-72f, -1.535f, -36f);
            Vector3 sewerStorageEntrance = new Vector3(36.7f, -8.035f, 76.7f);
            Vector3 underParkingGarage = new Vector3(1.25f, -4.035f, 77.25f);
            builder.WithIdentity("pike_mulch", "Pike", "Mulch")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0f;
                    av.Height = 1.07f;
                    av.Weight = 0.78f;
                    av.SkinColor = new Color(0.52f, 0.44f, 0.38f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(1f, 0.82f, 0.8f);
                    av.PupilDilation = 0.68f;
                    av.EyebrowScale = 1.15f;
                    av.EyebrowThickness = 1.25f;
                    av.EyebrowRestingHeight = -0.42f;
                    av.EyebrowRestingAngle = 4.2f;
                    av.LeftEye = (0.2f, 0.28f);
                    av.RightEye = (0.2f, 0.28f);
                    av.HairColor = new Color(0.18f, 0.16f, 0.14f);
                    av.HairPath = "Avatar/Hair/Mohawk/Mohawk";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Agitated", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Goatee", new Color(0.14f, 0.12f, 0.1f));
                    av.WithBodyLayer("Avatar/Layers/Top/Tucked T-Shirt", new Color(0.22f, 0.22f, 0.24f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.28f, 0.24f, 0.2f));
                    av.WithBodyLayer("Avatar/Layers/Accessories/FingerlessGloves", new Color(0.14f, 0.14f, 0.15f));
                    av.WithAccessoryLayer("Avatar/Accessories/Neck/GoldChain/GoldChain", new Color(0.65f, 0.52f, 0.2f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(0.2f, 0.18f, 0.16f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 580f, maxWeekly: 850f)
                        .WithOrdersPerWeek(1, 4)
                        .WithPreferredOrderDay(Day.Thursday)
                        .WithOrderTime(2110)
                        .WithStandards(CustomerStandard.Low)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.17f)
                        .WithDependence(baseAddiction: 0.1f, dependenceMultiplier: 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, -0.08f), (DrugType.Methamphetamine, 0.44f), (DrugType.Shrooms, -0.32f), (DrugType.Cocaine, 0.52f)
                        })
                        .WithPreferredProperties(Property.Focused, Property.Paranoia, Property.Sneaky);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("diesel", "mack");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(fishWarehouse, 0635, 108);
                    plan.Add(new WalkToSpec { Destination = sewerStorageEntrance, StartTime = 0843, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 210, 0) * Vector3.forward });
                    plan.StayInBuilding(hylandBank, 1018, 76);
                    plan.UseATM(1134);
                    plan.Add(new WalkToSpec { Destination = underParkingGarage, StartTime = 1200, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 180, 0) * Vector3.forward });
                    plan.StayInBuilding(fishWarehouse, 1356, 146);
                    plan.UseVendingMachine(1558);
                    plan.StayInBuilding(hylandBank, 1634, 102);
                    plan.StayInBuilding(fishWarehouse, 1814, 740);
                });
        }

        public PikeMulch() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.74f;
                Region = Region.Docks;
                Schedule.Enable();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"PikeMulch OnCreated failed: {ex.Message}");
            }
        }
    }
}
