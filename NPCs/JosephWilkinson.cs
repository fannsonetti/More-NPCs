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
    public sealed class JosephWilkinson : NPC
    {
        public override bool IsPhysical => true;
        
        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            Vector3 raysrealty = new Vector3(87.4855f, 1.065f, -6.889f);
            Vector3 hounddog = new Vector3(23.9503f, 1.065f, -36.5644f);
            Vector3 hylandauto = new Vector3(24.1091f, 1.065f, -40.8684f);
            Vector3 elizabeth = new Vector3(2.8427f, 1.065f, 46.9832f);
            Vector3 spawnPos = new Vector3(68.9616f, 5.5412f, -119.5116f);
            // var building = Buildings.GetAll().First();
            builder.WithIdentity("joseph_wilkinson", "Joseph", "Wilkinson")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 1.05f;
                    av.Weight = 0.5f;
                    av.SkinColor = new Color(0.784f, 0.654f, 0.545f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(1.0f, 0.8f, 0.8f);
                    av.PupilDilation = 1.2f;
                    av.EyebrowScale = 1.1f;
                    av.EyebrowThickness = 1.2f;
                    av.EyebrowRestingHeight = -0.432f;
                    av.EyebrowRestingAngle = -2.451f;
                    av.LeftEye = (0.219f, 0.5f);
                    av.RightEye = (0.219f, 0.5f);
                    av.HairColor = new Color(0.1f, 0.1f, 0.1f);
                    av.HairPath = "Avatar/Hair/Spiky/Spiky";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Stubble", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/FlannelButtonUp", new Color(0.326f, 0.578f, 0.896f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.23529411852359773f, 0.23529411852359773f, 0.23529411852359773f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.23529411852359773f, 0.23529411852359773f, 0.23529411852359773f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/OpenVest/OpenVest", new Color(0.23529411852359773f, 0.23529411852359773f, 0.23529411852359773f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 500f, maxWeekly: 900f)
                        .WithOrdersPerWeek(2, 4)
                        .WithPreferredOrderDay(Day.Saturday)
                        .WithOrderTime(1100)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.15f)
                        .WithDependence(baseAddiction: 0.25f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, -0.06f), (DrugType.Methamphetamine, 0.58f), (DrugType.Shrooms, 0.88f), (DrugType.Cocaine, 0.96f)
                        })
                        // .WithPreferredPropertiesById("Munchies", "Energizing", "Cyclopean");
                        .WithPreferredProperties(Property.Zombifying, Property.Spicy, Property.Sedating);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("elizabeth_homley", "fungal_phil");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new UseATMSpec { StartTime = 657 });
                    plan.Add(new WalkToSpec { Destination = hounddog, StartTime = 804, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 330, 0) * Vector3.forward });
                    plan.Add(new WalkToSpec { Destination = hylandauto, StartTime = 856, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 150, 0) * Vector3.forward });
                    plan.Add(new UseVendingMachineSpec { StartTime = 927 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Supermarket", StartTime = 1003, DurationMinutes = 120 });
                    plan.Add(new LocationDialogueSpec { Destination = elizabeth, StartTime = 1204, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 180, 0) * Vector3.forward });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Sauerkraut Supreme", StartTime = 1256, DurationMinutes = 127 });
                    plan.Add(new WalkToSpec { Destination = hylandauto, StartTime = 1504, FaceDestinationDirection = true, Forward = Quaternion.Euler(0, 150, 0) * Vector3.forward });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Wilkinson House", StartTime = 1803, DurationMinutes = 173 });
                    plan.Add(new UseVendingMachineSpec { StartTime = 2057 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Wilkinson House", StartTime = 2226, DurationMinutes = 509 });
                });
        }
        
        public JosephWilkinson() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                
                Aggressiveness = 5f;
                Region = S1API.Map.Region.Downtown;

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


