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
    /// Nadia Rim — sewer-adjacent docks hanger-on tied to Mike, Anna, and Diesel’s orbit.
    /// </summary>
    public sealed class NadiaRim : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var docksIndustrial = Building.Get<DocksIndustrialBuilding>();
            var randysBait = Building.Get<RandysBaitTackle>();
            Vector3 spawnPos = new Vector3(-88f, -1.485f, -40f);
            Vector3 underMotel = new Vector3(-52.25f, -6.535f, 92.5f);
            Vector3 roundRoom = new Vector3(39.75f, -8.035f, 40.75f);
            builder.WithIdentity("nadia_rim", "Nadia", "Rim")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 1.05f;
                    av.Height = 0.99f;
                    av.Weight = 0.52f;
                    var skin = new Color(0.55f, 0.42f, 0.36f);
                    av.SkinColor = skin;
                    av.LeftEyeLidColor = skin;
                    av.RightEyeLidColor = skin;
                    av.EyeBallTint = new Color(0.94f, 0.86f, 0.84f);
                    av.PupilDilation = 0.74f;
                    av.EyebrowScale = 0.95f;
                    av.EyebrowThickness = 0.88f;
                    av.EyebrowRestingHeight = -0.12f;
                    av.EyebrowRestingAngle = 1.6f;
                    av.LeftEye = (0.3f, 0.38f);
                    av.RightEye = (0.3f, 0.38f);
                    av.HairColor = new Color(0.08f, 0.08f, 0.09f);
                    av.HairPath = "Avatar/Hair/Mohawk/Mohawk";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_NeutralPout", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/Tucked T-Shirt", new Color(0.38f, 0.2f, 0.24f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.16f, 0.18f, 0.22f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/OpenVest/OpenVest", new Color(0.28f, 0.14f, 0.18f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(0.18f, 0.16f, 0.18f));
                    av.WithAccessoryLayer("Avatar/Accessories/Neck/GoldChain/GoldChain", new Color(0.72f, 0.58f, 0.22f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 620f, maxWeekly: 880f)
                        .WithOrdersPerWeek(1, 4)
                        .WithPreferredOrderDay(Day.Tuesday)
                        .WithOrderTime(2045)
                        .WithStandards(CustomerStandard.Low)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.14f)
                        .WithDependence(baseAddiction: 0.18f, dependenceMultiplier: 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.12f), (DrugType.Methamphetamine, 0.58f), (DrugType.Shrooms, 0.08f), (DrugType.Cocaine, 0.22f)
                        })
                        .WithPreferredProperties(Property.Paranoia, Property.Laxative, Property.Energizing);
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
                    plan.StayInBuilding(randysBait, 0718, 86);
                    plan.Add(new WalkToSpec { Destination = underMotel, StartTime = 0904, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 105, 0) * Vector3.forward });
                    plan.StayInBuilding(docksIndustrial, 1108, 118);
                    plan.StayInBuilding(randysBait, 1246, 92);
                    plan.Add(new WalkToSpec { Destination = roundRoom, StartTime = 1418, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 180, 0) * Vector3.forward });
                    plan.StayInBuilding(docksIndustrial, 1550, 138);
                    plan.UseVendingMachine(1748);
                    plan.StayInBuilding(randysBait, 1848, 118);
                    plan.StayInBuilding(docksIndustrial, 2024, 653);
                });
        }

        public NadiaRim() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.52f;
                Region = Region.Docks;
                Schedule.Enable();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"NadiaRim OnCreated failed: {ex.Message}");
            }
        }
    }
}
