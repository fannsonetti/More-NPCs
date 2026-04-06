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
    /// Dewey Koontz — dock rat who runs with Jane / Mack / Diesel’s crowd. Unlocks from those vanilla-adjacent links.
    /// </summary>
    public sealed class DeweyKoontz : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var docksIndustrial = Building.Get<DocksIndustrialBuilding>();
            var fishWarehouse = Building.Get<FishWarehouse>();
            Vector3 spawnPos = new Vector3(-34.5f, -1.535f, -28.5f);
            Vector3 roundRoom = new Vector3(39.75f, -8.035f, 40.75f);
            Vector3 underMotel = new Vector3(-52.25f, -6.535f, 92.5f);
            Vector3 sewerStorageEntrance = new Vector3(36.7f, -8.035f, 76.7f);
            builder.WithIdentity("dewey_koontz", "Dewey", "Koontz")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0f;
                    av.Height = 1.0f;
                    av.Weight = 0.62f;
                    av.SkinColor = new Color(0.66f, 0.54f, 0.44f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(1f, 0.85f, 0.82f);
                    av.PupilDilation = 0.7f;
                    av.EyebrowScale = 1.08f;
                    av.EyebrowThickness = 1.05f;
                    av.EyebrowRestingHeight = -0.35f;
                    av.EyebrowRestingAngle = 3.1f;
                    av.LeftEye = (0.22f, 0.33f);
                    av.RightEye = (0.22f, 0.33f);
                    av.HairColor = new Color(0.25f, 0.2f, 0.16f);
                    av.HairPath = "Avatar/Hair/Monk/Monk";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Stubble", new Color(0.2f, 0.16f, 0.12f));
                    av.WithBodyLayer("Avatar/Layers/Top/RolledButtonUp", new Color(0.26f, 0.32f, 0.24f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.2f, 0.19f, 0.17f));
                    av.WithBodyLayer("Avatar/Layers/Accessories/FingerlessGloves", new Color(0.15f, 0.14f, 0.13f));
                    av.WithAccessoryLayer("Avatar/Accessories/Neck/GoldChain/GoldChain", new Color(0.58f, 0.48f, 0.18f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(0.16f, 0.15f, 0.14f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 550f, maxWeekly: 820f)
                        .WithOrdersPerWeek(1, 4)
                        .WithPreferredOrderDay(Day.Friday)
                        .WithOrderTime(1815)
                        .WithStandards(CustomerStandard.Low)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.16f)
                        .WithDependence(baseAddiction: 0.12f, dependenceMultiplier: 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.28f), (DrugType.Methamphetamine, 0.22f), (DrugType.Shrooms, -0.2f), (DrugType.Cocaine, 0.35f)
                        })
                        .WithPreferredProperties(Property.Sneaky, Property.Munchies, Property.Euphoric);
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
                    plan.StayInBuilding(docksIndustrial, 0642, 86);
                    plan.Add(new WalkToSpec { Destination = roundRoom, StartTime = 0808, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 180, 0) * Vector3.forward });
                    plan.StayInBuilding(fishWarehouse, 0905, 118);
                    plan.Add(new WalkToSpec { Destination = underMotel, StartTime = 1103, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 105, 0) * Vector3.forward });
                    plan.StayInBuilding(docksIndustrial, 1310, 132);
                    plan.Add(new WalkToSpec { Destination = sewerStorageEntrance, StartTime = 1542, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 210, 0) * Vector3.forward });
                    plan.UseVendingMachine(1705);
                    plan.StayInBuilding(fishWarehouse, 1815, 118);
                    plan.StayInBuilding(docksIndustrial, 2006, 635);
                });
        }

        public DeweyKoontz() : base() { }

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
                MelonLogger.Error($"DeweyKoontz OnCreated failed: {ex.Message}");
            }
        }
    }
}
