using MelonLoader;
using S1API.Economy;
using S1API.Entities;
using S1API.Entities.NPCs.Northtown;
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
    /// Nico Marlowe - a low-end Northtown floater who sits between Marla Hale and Owen Crowe socially.
    /// Connected to Owen Crowe.
    /// </summary>
    public sealed class NicoMarlowe : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var chineseRestaurant = Building.Get<ChineseRestaurant>();
            var budsBar = Building.Get<BudsBar>();
            var northWarehouse = Building.Get<NorthWarehouse>();
            var northIndustrial = Building.Get<NorthIndustrialBuilding>();
            Vector3 spawnPos = new Vector3(-71.2847f, -2.935f, 145.6754f);
            Vector3 northStreet = new Vector3(-49.5478f, -4.035f, 168.5777f);

            builder.WithIdentity("nico_marlowe", "Nico", "Marlowe")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.38f;
                    av.Height = 0.98f;
                    av.Weight = 0.41f;
                    av.SkinColor = new Color(0.63f, 0.53f, 0.44f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.98f, 0.90f, 0.85f);
                    av.PupilDilation = 0.72f;
                    av.EyebrowScale = 1.00f;
                    av.EyebrowThickness = 0.96f;
                    av.EyebrowRestingHeight = -0.10f;
                    av.EyebrowRestingAngle = 1.4f;
                    av.LeftEye = (0.29f, 0.39f);
                    av.RightEye = (0.29f, 0.39f);
                    av.HairColor = new Color(0.17f, 0.12f, 0.09f);
                    av.HairPath = "Avatar/Hair/Spiky/Spiky";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Neutral", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/RolledButtonUp", new Color(0.37f, 0.41f, 0.35f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.24f, 0.25f, 0.23f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.84f, 0.84f, 0.84f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/OpenVest/OpenVest", new Color(0.22f, 0.24f, 0.26f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/FlatCap/FlatCap", new Color(0.29f, 0.30f, 0.28f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 200f, maxWeekly: 500f)
                        .WithOrdersPerWeek(1, 3)
                        .WithPreferredOrderDay(Day.Thursday)
                        .WithOrderTime(1930)
                        .WithStandards(CustomerStandard.Low)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.15f)
                        .WithDependence(baseAddiction: 0.17f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.41f), (DrugType.Methamphetamine, 0.35f), (DrugType.Shrooms, -0.12f), (DrugType.Cocaine, -0.08f)
                        })
                        .WithPreferredProperties(Property.Euphoric, Property.Foggy, Property.Smelly);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("owen_crowe");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(northIndustrial, 000, 430);
                    plan.StayInBuilding(chineseRestaurant, 845, 94);
                    plan.StayInBuilding(northWarehouse, 1015, 101);
                    plan.Add(new WalkToSpec { Destination = northStreet, StartTime = 1245, FaceDestinationDirection = true });
                    plan.StayInBuilding(budsBar, 1450, 128);
                    plan.UseVendingMachine(1715);
                    plan.StayInBuilding(northIndustrial, 2145, 294);
                });
        }

        public NicoMarlowe() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 0.49f;
                Region = Region.Northtown;

                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"NicoMarlowe OnCreated failed: {ex.Message}");
            }
        }
    }
}

