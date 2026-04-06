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
    /// <summary>Downtown customer — East Asian look aligned with in-game palette (Ming-family tones); docks-adjacent loop.</summary>
    public sealed class NinaCho : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var supermarket = Building.Get<Supermarket>();
            var cafe = Building.Get<Cafe>();
            Vector3 spawnPos = new Vector3(52.1f, 1.065f, 12.8f);

            builder.WithIdentity("nina_cho", "Nina", "Cho")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.88f;
                    av.Height = 0.98f;
                    av.Weight = 0.36f;
                    // Same golden-warm base as Jian Ming / Mr. Ming (Northtown Chinese NPCs) for a consistent in-game read.
                    av.SkinColor = new Color(0.867f, 0.7369f, 0.5623f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.98f, 0.94f, 0.88f);
                    av.PupilDilation = 0.64f;
                    av.EyebrowScale = 1.02f;
                    av.EyebrowThickness = 1.02f;
                    av.EyebrowRestingHeight = -0.10f;
                    av.EyebrowRestingAngle = 1.25f;
                    av.LeftEye = (0.24f, 0.22f);
                    av.RightEye = (0.24f, 0.22f);
                    av.HairColor = new Color(0.04f, 0.04f, 0.045f);
                    av.HairPath = "Avatar/Hair/DoubleTopKnot/DoubleTopKnot";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_NeutralPout", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/V-Neck", new Color(0.36f, 0.30f, 0.28f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.20f, 0.22f, 0.28f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.18f, 0.18f, 0.20f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(500f, 900f)
                        .WithOrdersPerWeek(1, 3)
                        .WithPreferredOrderDay(Day.Monday)
                        .WithOrderTime(2000)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(0f, 1f)
                        .WithCallPoliceChance(0.16f)
                        .WithDependence(0.08f, 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.18f), (DrugType.Methamphetamine, 0.12f), (DrugType.Shrooms, 0.36f), (DrugType.Cocaine, -0.08f)
                        })
                        .WithPreferredProperties(Property.Focused, Property.Paranoia, Property.Refreshing);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .WithConnectionsById("orlando_castillo")
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach);
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new StayInBuildingSpec { BuildingName = "Boutique Store", StartTime = 0845, DurationMinutes = 134 });
                    plan.Add(new SitSpec { SeatSetPath = "@Businesses/Taco Ticklers/Fast Food Booth (2)/fast food booth/Seat", StartTime = 1100, DurationMinutes = 139 });
                    plan.StayInBuilding(cafe, 1240, 89);
                    plan.StayInBuilding(supermarket, 1415, 149);
                    plan.Add(new StayInBuildingSpec { BuildingName = "RE Office", StartTime = 1745, DurationMinutes = 164 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Apartment Building 2", StartTime = 2045, DurationMinutes = 764 });
                });
        }

        public NinaCho() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.41f;
                Region = Region.Downtown;
                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"NinaCho OnCreated failed: {ex.Message}");
            }
        }
    }
}
