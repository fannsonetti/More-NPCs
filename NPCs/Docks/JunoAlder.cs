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
    /// <summary>Juno Alder — docks odd-jobber who crosses paths with Kaela’s runs and Lisa’s plant work.</summary>
    public sealed class JunoAlder : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var fishWarehouse = Building.Get<FishWarehouse>();
            var randysBait = Building.Get<RandysBaitTackle>();
            var docksIndustrial = Building.Get<DocksIndustrialBuilding>();
            Vector3 warehousePier = new Vector3(-62.4702f, -1.5315f, 19.8399f);
            Vector3 spawnPos = new Vector3(-69.4f, -1.535f, -22.8f);

            builder.WithIdentity("juno_alder", "Juno", "Alder")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.88f;
                    av.Height = 1.0f;
                    av.Weight = 0.41f;
                    var skin = new Color(0.64f, 0.52f, 0.44f);
                    av.SkinColor = skin;
                    av.LeftEyeLidColor = skin;
                    av.RightEyeLidColor = skin;
                    av.EyeBallTint = new Color(0.96f, 0.9f, 0.86f);
                    av.PupilDilation = 0.71f;
                    av.EyebrowScale = 0.98f;
                    av.EyebrowThickness = 0.84f;
                    av.EyebrowRestingHeight = -0.1f;
                    av.EyebrowRestingAngle = 1.9f;
                    av.LeftEye = (0.3f, 0.36f);
                    av.RightEye = (0.3f, 0.36f);
                    av.HairColor = new Color(0.18f, 0.14f, 0.11f);
                    av.HairPath = "Avatar/Hair/lowbun/LowBun";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/FlannelButtonUp", new Color(0.34f, 0.38f, 0.28f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jorts", new Color(0.22f, 0.24f, 0.2f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/Apron/Apron", new Color(0.42f, 0.36f, 0.3f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.38f, 0.36f, 0.34f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 620f, maxWeekly: 900f)
                        .WithOrdersPerWeek(1, 4)
                        .WithPreferredOrderDay(Day.Tuesday)
                        .WithOrderTime(1745)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.18f)
                        .WithDependence(baseAddiction: 0.17f, dependenceMultiplier: 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.22f), (DrugType.Methamphetamine, 0.18f), (DrugType.Shrooms, 0.44f), (DrugType.Cocaine, -0.08f)
                        })
                        .WithPreferredProperties(Property.ThoughtProvoking, Property.Munchies, Property.Refreshing);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("kaela_thorn", "lisa_gardener", "sable_reed");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(randysBait, 0648, 102);
                    plan.StayInBuilding(fishWarehouse, 0848, 124);
                    plan.Add(new WalkToSpec { Destination = warehousePier, StartTime = 1052, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 180, 0) * Vector3.forward });
                    plan.UseATM(1218);
                    plan.StayInBuilding(docksIndustrial, 1248, 156);
                    plan.UseVendingMachine(1452);
                    plan.StayInBuilding(fishWarehouse, 1518, 118);
                    plan.StayInBuilding(randysBait, 1654, 96);
                    plan.StayInBuilding(docksIndustrial, 1828, 168);
                    plan.Add(new WalkToSpec { Destination = warehousePier, StartTime = 2034, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 180, 0) * Vector3.forward });
                    plan.StayInBuilding(docksIndustrial, 2102, 585);
                });
        }

        public JunoAlder() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.49f;
                Region = Region.Docks;
                Schedule.Enable();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"JunoAlder OnCreated failed: {ex.Message}");
            }
        }
    }
}
