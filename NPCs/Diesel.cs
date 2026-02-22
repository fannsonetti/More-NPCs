using MelonLoader;
using S1API.Economy;
using S1API.Entities;
using S1API.Entities.Schedule;
using S1API.GameTime;
using S1API.Products;
using S1API.Properties;
using UnityEngine;

namespace MoreNPCs.NPCs
{
    /// <summary>
    /// An example S1API NPC that opts into a physical rig.
    /// Demonstrates movement and inventory usage.
    /// </summary>
    public sealed class Diesel : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            Vector3 behindFishShopDiesel = new Vector3(-99.2265f, -1.485f, -42.492f);
            Vector3 waterDiesel = new Vector3(-69.4977f, -1.535f, -38.2393f);
            Vector3 scopeBank = new Vector3(73.4736f, 0.9662f, 39.0171f);
            Vector3 gasmart = new Vector3(17.7331f, 1.215f, -3.0911f);
            Vector3 pos5 = new Vector3(-121.0444f, -2.935f, 78.6722f);
            Vector3 pos6 = new Vector3(-22.7021f, 1.065f, 46.8433f);
            Vector3 spawnPos = new Vector3(-12.8076f, 1.065f, 66.7038f);
            // var building = Buildings.GetAll().First();
            builder.WithIdentity("diesel", "Diesel", "")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 1.05f;
                    av.Weight = 0.7f;
                    av.SkinColor = new Color(0.713f, 0.592f, 0.486f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(1.0f, 0.8f, 0.8f);
                    av.PupilDilation = 0.75f;
                    av.EyebrowScale = 1.39f;
                    av.EyebrowThickness = 0.7f;
                    av.EyebrowRestingHeight = -0.48f;
                    av.EyebrowRestingAngle = -4.64f;
                    av.LeftEye = (0.18f, 0.24f);
                    av.RightEye = (0.18f, 0.24f);
                    av.HairColor = new Color(0.198f, 0.118f, 0.062f);
                    av.HairPath = "Avatar/Hair/CloseBuzzCut/CloseBuzzCut";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Agitated", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/OldPersonWrinkles", new Color(0.957f, 0.474f, 0.938f));
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Stubble", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Goatee", new Color(0.198f, 0.118f, 0.062f));
                    av.WithBodyLayer("Avatar/Layers/Top/T-Shirt", new Color(0.151f, 0.151f, 0.151f));
                    av.WithBodyLayer("Avatar/Layers/Top/UpperBodyTattoos", new Color(0.151f, 0.151f, 0.151f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.178f, 0.217f, 0.406f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(0.151f, 0.151f, 0.151f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 600f, maxWeekly: 900f)
                        .WithOrdersPerWeek(1, 4)
                        .WithPreferredOrderDay(Day.Monday)
                        .WithOrderTime(0030)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.1f)
                        .WithDependence(baseAddiction: 0.0f, dependenceMultiplier: 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, -0.12f), (DrugType.Methamphetamine, 0.78f), (DrugType.Shrooms, -0.85f), (DrugType.Cocaine, 0.41f)
                        })
                        .WithPreferredProperties(Property.Energizing, Property.Paranoia, Property.Explosive);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("melissa_wood", "mack");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new WalkToSpec { Destination = behindFishShopDiesel, StartTime = 0903, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 250, 0) * Vector3.forward });
                    plan.Add(new WalkToSpec { Destination = waterDiesel, StartTime = 1056, FaceDestinationDirection = true });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Fish Warehouse", StartTime = 1227, DurationMinutes = 156 });
                    plan.Add(new WalkToSpec { Destination = scopeBank, StartTime = 1504, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = gasmart, StartTime = 1804, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 340, 0) * Vector3.forward });
                    plan.Add(new WalkToSpec { Destination = waterDiesel, StartTime = 1857, FaceDestinationDirection = true });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Fish Warehouse", StartTime = 2026, DurationMinutes = 157 });
                    plan.Add(new UseVendingMachineSpec { StartTime = 2304 });
                    plan.Add(new WalkToSpec { Destination = waterDiesel, StartTime = 0057, FaceDestinationDirection = true });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Docks Industrial Building", StartTime = 0157, DurationMinutes = 445 });
                });
        }

        public Diesel() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 5f;
                Region = S1API.Map.Region.Docks;

                // Customer.RequestProduct();

                Schedule.Enable();

            }
            catch (Exception ex)
            {
                MelonLogger.Error($"ExamplePhysicalNPC OnCreated failed: {ex.Message}");
                MelonLogger.Error($"StackTrace: {ex.StackTrace}");
            }
        }
    }
}


