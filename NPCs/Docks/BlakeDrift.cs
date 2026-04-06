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
    /// Blake Drift — Gasmart-strip regular with the same wrong-energy lane as the freezer dealer, but sweaty, anxious, and not the “frozen” look.
    /// </summary>
    public sealed class BlakeDrift : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var docksIndustrial = Building.Get<DocksIndustrialBuilding>();
            var fishWarehouse = Building.Get<FishWarehouse>();
            var randysBait = Building.Get<RandysBaitTackle>();
            Vector3 spawnPos = new Vector3(12.35f, 0.3515f, -4.85f);
            Vector3 underParkingGarage = new Vector3(1.25f, -4.035f, 77.25f);
            Vector3 sewerStorageEntrance = new Vector3(36.7f, -8.035f, 76.7f);
            Vector3 underMotel = new Vector3(-52.25f, -6.535f, 92.5f);

            builder.WithIdentity("blake_drift", "Blake", "Drift")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 1.03f;
                    av.Weight = 0.62f;
                    var skin = new Color(0.58f, 0.47f, 0.40f);
                    av.SkinColor = skin;
                    av.LeftEyeLidColor = skin;
                    av.RightEyeLidColor = skin;
                    av.EyeBallTint = new Color(0.96f, 0.90f, 0.86f);
                    av.PupilDilation = 0.74f;
                    av.EyebrowScale = 1.18f;
                    av.EyebrowThickness = 1.12f;
                    av.EyebrowRestingHeight = -0.38f;
                    av.EyebrowRestingAngle = 3.4f;
                    av.LeftEye = (0.20f, 0.28f);
                    av.RightEye = (0.20f, 0.28f);
                    av.HairColor = new Color(0.20f, 0.17f, 0.15f);
                    av.HairPath = "Avatar/Hair/BuzzCut/BuzzCut";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_NeutralPout", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/TiredEyes", new Color(0.08f, 0.08f, 0.08f, 0.38f));
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Stubble", new Color(0.16f, 0.14f, 0.12f));
                    av.WithBodyLayer("Avatar/Layers/Top/RolledButtonUp", new Color(0.38f, 0.32f, 0.28f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jorts", new Color(0.2f, 0.22f, 0.28f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sandals/Sandals", new Color(0.35f, 0.32f, 0.28f));
                    av.WithAccessoryLayer("Avatar/Accessories/Neck/GoldChain/GoldChain", new Color(0.58f, 0.48f, 0.2f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 620f, maxWeekly: 920f)
                        .WithOrdersPerWeek(1, 4)
                        .WithPreferredOrderDay(Day.Friday)
                        .WithOrderTime(2115)
                        .WithStandards(CustomerStandard.Low)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.15f)
                        .WithDependence(baseAddiction: 0.14f, dependenceMultiplier: 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.18f), (DrugType.Methamphetamine, 0.48f), (DrugType.Shrooms, -0.12f), (DrugType.Cocaine, 0.22f)
                        })
                        .WithPreferredProperties(Property.Paranoia, Property.Sneaky, Property.Energizing);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("billy_kramer", "maya_webb", "henry_mitchell", "dewey_koontz");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(randysBait, 0742, 94);
                    plan.Add(new WalkToSpec { Destination = underParkingGarage, StartTime = 0916, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 180, 0) * Vector3.forward });
                    plan.StayInBuilding(docksIndustrial, 1048, 118);
                    plan.Add(new WalkToSpec { Destination = sewerStorageEntrance, StartTime = 1226, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 210, 0) * Vector3.forward });
                    plan.StayInBuilding(fishWarehouse, 1424, 108);
                    plan.UseVendingMachine(1550);
                    plan.Add(new WalkToSpec { Destination = underMotel, StartTime = 1627, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 105, 0) * Vector3.forward });
                    plan.StayInBuilding(docksIndustrial, 1750, 112);
                    plan.StayInBuilding(randysBait, 1902, 759);
                });
        }

        public BlakeDrift() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.61f;
                Region = Region.Docks;
                Schedule.Enable();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"BlakeDrift OnCreated failed: {ex.Message}");
            }
        }
    }
}
