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
    /// <summary>Sable Reed — dark, sable-toned look.</summary>
    public sealed class SableReed : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var docksIndustrial = Building.Get<DocksIndustrialBuilding>();
            var fishWarehouse = Building.Get<FishWarehouse>();
            var randysBait = Building.Get<RandysBaitTackle>();
            Vector3 spawnPos = new Vector3(-81.6f, -1.485f, -36.2f);
            Vector3 roundRoom = new Vector3(39.75f, -8.035f, 40.75f);
            Vector3 underParkingGarage = new Vector3(1.25f, -4.035f, 77.25f);
            Vector3 sewerStorageEntrance = new Vector3(36.7f, -8.035f, 76.7f);

            builder.WithIdentity("sable_reed", "Sable", "Reed")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 1.02f;
                    av.Height = 1.0f;
                    av.Weight = 0.46f;
                    var skin = new Color(0.38f, 0.3f, 0.26f);
                    av.SkinColor = skin;
                    av.LeftEyeLidColor = skin;
                    av.RightEyeLidColor = skin;
                    av.EyeBallTint = new Color(0.9f, 0.84f, 0.8f);
                    av.PupilDilation = 0.73f;
                    av.EyebrowScale = 1.05f;
                    av.EyebrowThickness = 0.9f;
                    av.EyebrowRestingHeight = -0.18f;
                    av.EyebrowRestingAngle = 2.4f;
                    av.LeftEye = (0.3f, 0.36f);
                    av.RightEye = (0.3f, 0.36f);
                    av.HairColor = new Color(0.08f, 0.07f, 0.06f);
                    av.HairPath = "Avatar/Hair/LongCurly/LongCurly";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/FlannelButtonUp", new Color(0.32f, 0.14f, 0.14f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.16f, 0.16f, 0.18f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(0.12f, 0.12f, 0.13f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/Belt/Belt", new Color(0.22f, 0.18f, 0.14f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 630f, maxWeekly: 910f)
                        .WithOrdersPerWeek(1, 4)
                        .WithPreferredOrderDay(Day.Wednesday)
                        .WithOrderTime(1940)
                        .WithStandards(CustomerStandard.Low)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.17f)
                        .WithDependence(baseAddiction: 0.16f, dependenceMultiplier: 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.12f), (DrugType.Methamphetamine, 0.52f), (DrugType.Shrooms, -0.22f), (DrugType.Cocaine, 0.31f)
                        })
                        .WithPreferredProperties(Property.Paranoia, Property.Energizing, Property.Sneaky);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(docksIndustrial, 0655, 86);
                    plan.Add(new WalkToSpec { Destination = roundRoom, StartTime = 0821, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 180, 0) * Vector3.forward });
                    plan.StayInBuilding(randysBait, 0945, 94);
                    plan.Add(new WalkToSpec { Destination = underParkingGarage, StartTime = 1119, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 180, 0) * Vector3.forward });
                    plan.StayInBuilding(fishWarehouse, 1308, 98);
                    plan.UseVendingMachine(1446);
                    plan.Add(new WalkToSpec { Destination = sewerStorageEntrance, StartTime = 1514, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 210, 0) * Vector3.forward });
                    plan.StayInBuilding(docksIndustrial, 1648, 118);
                    plan.StayInBuilding(randysBait, 1806, 112);
                    plan.StayInBuilding(docksIndustrial, 1928, 686);
                });
        }

        public SableReed() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.55f;
                Region = Region.Docks;
                Schedule.Enable();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"SableReed OnCreated failed: {ex.Message}");
            }
        }
    }
}
