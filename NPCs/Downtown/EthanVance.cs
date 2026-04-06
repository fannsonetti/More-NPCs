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
    /// <summary>Downtown customer — Slop Shop / bank / Apt 2 nights; formerly Uriah, not Evan (distinct from Evan Rowland).</summary>
    public sealed class EthanVance : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var slop = Building.Get<SlopShop>();
            Vector3 spawnPos = new Vector3(73.8f, 0.9662f, 38.2f);

            builder.WithIdentity("ethan_vance", "Ethan", "Vance")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 1.02f;
                    av.Weight = 0.48f;
                    av.SkinColor = new Color(0.38f, 0.30f, 0.26f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.90f, 0.84f, 0.80f);
                    av.PupilDilation = 0.48f;
                    av.EyebrowScale = 1.02f;
                    av.EyebrowThickness = 0.94f;
                    av.EyebrowRestingHeight = -0.11f;
                    av.EyebrowRestingAngle = 0.52f;
                    av.LeftEye = (0.335f, 0.435f);
                    av.RightEye = (0.335f, 0.435f);
                    av.HairColor = new Color(0.07f, 0.07f, 0.08f);
                    av.HairPath = "Avatar/Hair/Peaked/Peaked";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Neutral", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Stubble", new Color(0.09f, 0.07f, 0.06f));
                    av.WithBodyLayer("Avatar/Layers/Top/FlannelButtonUp", new Color(0.32f, 0.30f, 0.28f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.19f, 0.21f, 0.27f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(0.11f, 0.11f, 0.12f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(450f, 850f)
                        .WithOrdersPerWeek(1, 4)
                        .WithPreferredOrderDay(Day.Tuesday)
                        .WithOrderTime(1920)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(0f, 1f)
                        .WithCallPoliceChance(0.19f)
                        .WithDependence(0.11f, 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, -0.10f), (DrugType.Methamphetamine, 0.42f), (DrugType.Shrooms, 0.18f), (DrugType.Cocaine, 0.22f)
                        })
                        .WithPreferredProperties(Property.Paranoia, Property.Energizing, Property.Sneaky);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .WithConnectionsById("fungal_phil", "kevin_oakley")
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach);
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new StayInBuildingSpec { BuildingName = "Town hall", StartTime = 0935, DurationMinutes = 149 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Hyland Bank", StartTime = 1225, DurationMinutes = 119 });
                    plan.StayInBuilding(slop, 1545, 149);
                    plan.Add(new StayInBuildingSpec { BuildingName = "Cafe", StartTime = 1855, DurationMinutes = 194 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Apartment Building 2", StartTime = 2150, DurationMinutes = 679 });
                });
        }

        public EthanVance() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.52f;
                Region = Region.Downtown;
                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"EthanVance OnCreated failed: {ex.Message}");
            }
        }
    }
}
