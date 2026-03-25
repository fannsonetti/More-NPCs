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
    /// Piper Sloan - outgoing Northtown errand-runner who treats Kyle as her only real contact.
    /// Connected only to Kyle Cooley.
    /// </summary>
    public sealed class PiperSloan : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var shack = Building.Get<Shack>();
            var budsBar = Building.Get<BudsBar>();
            var northWarehouse = Building.Get<NorthWarehouse>();
            var chineseRestaurant = Building.Get<ChineseRestaurant>();
            Vector3 spawnPos = new Vector3(-41.7551f, -2.9354f, 171.8678f);
            Vector3 northLookout = new Vector3(-63.2588f, -4.035f, 168.9073f);

            builder.WithIdentity("piper_sloan", "Piper", "Sloan")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.74f;
                    av.Height = 1.00f;
                    av.Weight = 0.40f;
                    av.SkinColor = new Color(0.73f, 0.60f, 0.50f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.99f, 0.91f, 0.87f);
                    av.PupilDilation = 0.68f;
                    av.EyebrowScale = 0.98f;
                    av.EyebrowThickness = 0.90f;
                    av.EyebrowRestingHeight = -0.02f;
                    av.EyebrowRestingAngle = 2.0f;
                    av.LeftEye = (0.30f, 0.42f);
                    av.RightEye = (0.30f, 0.42f);
                    av.HairColor = new Color(0.33f, 0.21f, 0.14f);
                    av.HairPath = "Avatar/Hair/DoubleTopKnot/DoubleTopKnot";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Neutral", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/Tucked T-Shirt", new Color(0.48f, 0.44f, 0.36f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.23f, 0.27f, 0.38f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sandals/Sandals", new Color(0.38f, 0.30f, 0.22f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/CollarJacket/CollarJacket", new Color(0.31f, 0.33f, 0.37f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/BucketHat/BucketHat", new Color(0.28f, 0.36f, 0.22f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 300f, maxWeekly: 700f)
                        .WithOrdersPerWeek(1, 4)
                        .WithPreferredOrderDay(Day.Tuesday)
                        .WithOrderTime(1830)
                        .WithStandards(CustomerStandard.Low)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.15f)
                        .WithDependence(baseAddiction: 0.12f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.58f), (DrugType.Methamphetamine, 0.12f), (DrugType.Shrooms, 0.72f), (DrugType.Cocaine, -0.28f)
                        })
                        .WithPreferredProperties(Property.Calming, Property.Euphoric, Property.Shrinking);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("kyle_cooley");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(shack, 000, 479);
                    plan.Add(new WalkToSpec { Destination = northLookout, StartTime = 815, FaceDestinationDirection = true });
                    plan.StayInBuilding(budsBar, 930, 104);
                    plan.StayInBuilding(northWarehouse, 1115, 119);
                    plan.UseATM(1255);
                    plan.StayInBuilding(chineseRestaurant, 1615, 89);
                    plan.UseVendingMachine(1755);
                    plan.StayInBuilding(shack, 1825, 394);
                });
        }

        public PiperSloan() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 0.53f;
                Region = Region.Northtown;

                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"PiperSloan OnCreated failed: {ex.Message}");
            }
        }
    }
}

