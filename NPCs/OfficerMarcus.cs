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
    public sealed class OfficerMarcus : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            Vector3 pos1 = new Vector3(27.4168f, 1.065f, -14.4305f);
            Vector3 pos2 = new Vector3(-22.5378f, 1.065f, -43.1838f);
            Vector3 pos3 = new Vector3(-150.581f, -2.935f, 118.1681f);
            Vector3 pos4 = new Vector3(-139.3423f, -4.335f, 18.8808f);
            Vector3 pos5 = new Vector3(-121.0444f, -2.935f, 78.6722f);
            Vector3 pos6 = new Vector3(-22.7021f, 1.065f, 46.8433f);
            Vector3 spawnPos = new Vector3(16.0717f, 1.065f, 38.0883f);
            builder.WithIdentity("officer_marcus", "Officer", "Marcus")
                .WithAppearanceDefaults(av =>
                {
                    var skinColor = new Color(0.713f, 0.592f, 0.486f);
                    av.Gender = 0.0f;
                    av.Height = 1.0f;
                    av.Weight = 0.4f;
                    av.SkinColor = skinColor;
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = Color.white;
                    av.PupilDilation = 1.0f;
                    av.EyebrowScale = 1.1f;
                    av.EyebrowThickness = 1.0f;
                    av.EyebrowRestingHeight = -0.432f;
                    av.EyebrowRestingAngle = -2.451f;
                    av.LeftEye = (0.219f, 0.5f);
                    av.RightEye = (0.219f, 0.5f);
                    av.HairColor = new Color(0.31f, 0.2f, 0.12f);
                    av.HairPath = "Avatar/Hair/Buzzcut/Buzzcut";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_NeutralPout", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/OldPersonWrinkles", new Color32(0, 0, 0, 0));
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Goatee", new Color(0.31f, 0.2f, 0.12f));
                    av.WithBodyLayer("Avatar/Layers/Top/RolledButtonUp", new Color(0.178f, 0.217f, 0.406f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.235f, 0.235f, 0.235f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(0.151f, 0.151f, 0.151f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/PoliceBelt/PoliceBelt", Color.white);
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/BulletproofVest/BulletproofVest_Police", Color.white);
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 800f, maxWeekly: 1200f)
                        .WithOrdersPerWeek(1, 4)
                        .WithPreferredOrderDay(Day.Sunday)
                        .WithOrderTime(2330)
                        .WithStandards(CustomerStandard.High)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.90f)
                        .WithDependence(baseAddiction: 0.0f, dependenceMultiplier: 1.1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.52f), (DrugType.Methamphetamine, -0.84f), (DrugType.Shrooms, 0.28f), (DrugType.Cocaine, -0.09f)
                        })
                        .WithPreferredProperties(Property.Athletic, Property.AntiGravity, Property.Sneaky);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(1.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("alison_knight", "carl_bundy", "jack_knight");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new WalkToSpec { Destination = pos1, StartTime = 657, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = pos2, StartTime = 856, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = pos3, StartTime = 1104, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = pos4, StartTime = 1256, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = pos5, StartTime = 1504, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = pos6, StartTime = 1656, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = pos3, StartTime = 1903, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = pos5, StartTime = 2057, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = pos2, StartTime = 2304, FaceDestinationDirection = true });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Police Station", StartTime = 1435, DurationMinutes = 120 });
                })
                .WithInventoryDefaults(inv =>
                {
                    // Startup items that will always be in inventory when spawned
                    inv.WithStartupItems("m1911", "m1911mag")
                        // Random cash between $50 and $500
                        .WithRandomCash(min: 50, max: 500)
                        // Preserve inventory across sleep cycles
                        .WithClearInventoryEachNight(false);
                });
        }

        public OfficerMarcus() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 3f;
                Region = S1API.Map.Region.Suburbia;

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
