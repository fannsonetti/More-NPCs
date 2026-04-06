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
    /// <summary>Downtown customer — Apt 3; RE office + errands crowd. Knows Jennifer Rivera (listings) and Rhea Larkin (same downtown loop); Tessa for hook-ups. Charcoal blouse + skirt (no belt).</summary>
    public sealed class HarperLin : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var cafe = Building.Get<Cafe>();
            var supermarket = Building.Get<Supermarket>();
            Vector3 plaza = new Vector3(69.7895f, 1.065f, 15.4409f);
            Vector3 spawnPos = new Vector3(58.4f, 1.065f, 22.1f);

            builder.WithIdentity("harper_lin", "Harper", "Lin")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.92f;
                    av.Height = 0.99f;
                    av.Weight = 0.37f;
                    av.SkinColor = new Color(0.50f, 0.42f, 0.36f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.97f, 0.93f, 0.90f);
                    av.PupilDilation = 0.64f;
                    av.EyebrowScale = 0.98f;
                    av.EyebrowThickness = 0.84f;
                    av.EyebrowRestingHeight = -0.06f;
                    av.EyebrowRestingAngle = 1.85f;
                    av.LeftEye = (0.33f, 0.44f);
                    av.RightEye = (0.33f, 0.44f);
                    av.HairColor = new Color(0.11f, 0.10f, 0.09f);
                    av.HairPath = "Avatar/Hair/lowbun/LowBun";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_NeutralPout", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/RolledButtonUp", new Color(0.22f, 0.24f, 0.30f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/FemaleUnderwear", new Color(0.88f, 0.86f, 0.90f));
                    av.WithAccessoryLayer("Avatar/Accessories/Bottom/MediumSkirt/MediumSkirt", new Color(0.14f, 0.18f, 0.26f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/DressShoes/DressShoes", new Color(0.10f, 0.10f, 0.12f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(480f, 950f)
                        .WithOrdersPerWeek(1, 3)
                        .WithPreferredOrderDay(Day.Wednesday)
                        .WithOrderTime(2015)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(0f, 1f)
                        .WithCallPoliceChance(0.17f)
                        .WithDependence(0.10f, 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.22f),
                            (DrugType.Methamphetamine, -0.10f),
                            (DrugType.Shrooms, 0.28f),
                            (DrugType.Cocaine, 0.14f)
                        })
                        .WithPreferredProperties(Property.Slippery, Property.Focused, Property.Refreshing);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .WithConnectionsById("rhea_larkin")
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach);
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new StayInBuildingSpec { BuildingName = "Apartment Building 3", StartTime = 0820, DurationMinutes = 144 });
                    plan.Add(new SitSpec { SeatSetPath = "@Businesses/Taco Ticklers/Fast Food Booth (1)", StartTime = 1055, DurationMinutes = 69 });
                    plan.StayInBuilding(cafe, 1125, 124);
                    plan.Add(new WalkToSpec { Destination = plaza, StartTime = 1405, FaceDestinationDirection = true });
                    plan.StayInBuilding(supermarket, 1410, 164);
                    plan.Add(new StayInBuildingSpec { BuildingName = "RE Office", StartTime = 1715, DurationMinutes = 149 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Apartment Building 3", StartTime = 2035, DurationMinutes = 204 });
                });
        }

        public HarperLin() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.43f;
                Region = Region.Downtown;
                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"HarperLin OnCreated failed: {ex.Message}");
            }
        }
    }
}
