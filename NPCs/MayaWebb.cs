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
    /// Maya Webb - Chemical factory worker in Westville. Lives in Docks. Spawn and break spot at warehouse pier. Connections: mac_cooper, billy_kramer.
    /// </summary>
    public sealed class MayaWebb : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var chemicalPlantA = Building.Get<ChemicalPlantA>();
            var docksIndustrial = Building.Get<DocksIndustrialBuilding>();
            Vector3 breakSpot = new Vector3(-102.603f, -2.935f, 90.9458f);
            builder.WithIdentity("maya_webb", "Maya", "Webb")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 2.85f;
                    av.Height = 0.96f;
                    av.Weight = 0.38f;
                    av.SkinColor = new Color(0.72f, 0.59f, 0.48f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(1.0f, 0.92f, 0.88f);
                    av.PupilDilation = 0.72f;
                    av.EyebrowScale = 1.02f;
                    av.EyebrowThickness = 0.9f;
                    av.EyebrowRestingHeight = -0.2f;
                    av.EyebrowRestingAngle = 2.2f;
                    av.LeftEye = (0.28f, 0.36f);
                    av.RightEye = (0.28f, 0.36f);
                    av.HairColor = new Color(0.28f, 0.20f, 0.14f);
                    av.HairPath = "Avatar/Hair/lowbun/LowBun";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/HazmatSuit", new Color(0.9451f, 0.7882f, 0.0118f));
                    av.WithBodyLayer("Avatar/Layers/Accessories/Gloves", new Color(0.2531f, 0.5563f, 0.7578f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(0.2539f, 0.2539f, 0.2539f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/Respirator/Respirator", new Color(0.2539f, 0.2539f, 0.2539f));
                })
                .WithSpawnPosition(breakSpot)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 600f, maxWeekly: 1000f)
                        .WithOrdersPerWeek(2, 4)
                        .WithPreferredOrderDay(Day.Wednesday)
                        .WithOrderTime(1830)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.18f)
                        .WithDependence(baseAddiction: 0.2f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.28f), (DrugType.Methamphetamine, -0.42f), (DrugType.Shrooms, 0.51f), (DrugType.Cocaine, -0.19f)
                        })
                        .WithPreferredProperties(Property.Calming, Property.Refreshing, Property.ThoughtProvoking);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("mac_cooper", "billy_kramer");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(chemicalPlantA, 630, 178);
                    plan.Add(new WalkToSpec { Destination = breakSpot, StartTime = 845, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 270, 0) * Vector3.forward });
                    plan.StayInBuilding(chemicalPlantA, 1145, 118);
                    plan.Add(new WalkToSpec { Destination = breakSpot, StartTime = 1302, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 270, 0) * Vector3.forward });
                    plan.StayInBuilding(chemicalPlantA, 1555, 148);
                    plan.StayInBuilding(docksIndustrial, 1740, 768);
                });
        }

        public MayaWebb() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 3f;
                Region = S1API.Map.Region.Docks;

                Schedule.Enable();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"MayaWebb OnCreated failed: {ex.Message}");
                MelonLogger.Error($"StackTrace: {ex.StackTrace}");
            }
        }
    }
}
