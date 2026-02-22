using MelonLoader;
using S1API.Economy;
using S1API.Entities;
using S1API.Entities.Schedule;
using S1API.Entities.NPCs.Westville;
using S1API.GameTime;
using S1API.Map;
using S1API.Map.Buildings;
using S1API.Products;
using S1API.Properties;
using UnityEngine;

namespace MoreNPCs.NPCs
{
    /// <summary>
    /// Rhonda Vex - Westville GasMart manager. Female, enjoys meth most. Spawns at West Gas-Mart, works ~8 hrs/day, steps out for vending breaks (~30 min). Lives in Room 3. Connections: charles_rowland, kim_delaney.
    /// </summary>
    public sealed class RhondaVex : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var westGasMart = Building.Get<WestGasMart>();
            var room3 = Building.Get<Room3>();
            Vector3 managerSpot = new Vector3(-117.6071f, -2.835f, 58.6968f);
            builder.WithIdentity("rhonda_vex", "Rhonda", "Vex")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 2.85f;
                    av.Height = 1.02f;
                    av.Weight = 0.38f;
                    av.SkinColor = new Color(0.68f, 0.55f, 0.45f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(1.0f, 0.88f, 0.82f);
                    av.PupilDilation = 0.78f;
                    av.EyebrowScale = 0.95f;
                    av.EyebrowThickness = 0.85f;
                    av.EyebrowRestingHeight = -0.15f;
                    av.EyebrowRestingAngle = 1.8f;
                    av.LeftEye = (0.26f, 0.40f);
                    av.RightEye = (0.26f, 0.40f);
                    av.HairColor = new Color(0.45f, 0.32f, 0.22f);
                    av.HairPath = "Avatar/Hair/LongCurly/LongCurly";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Neutral", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/RolledButtonUp", new Color(0.55f, 0.42f, 0.48f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.28f, 0.26f, 0.30f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.22f, 0.22f, 0.24f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/Belt/Belt", new Color(0.20f, 0.18f, 0.16f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/CollarJacket/CollarJacket", new Color(0.35f, 0.32f, 0.38f));
                })
                .WithSpawnPosition(managerSpot)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 500f, maxWeekly: 700f)
                        .WithOrdersPerWeek(2, 4)
                        .WithPreferredOrderDay(Day.Thursday)
                        .WithOrderTime(2000)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.12f)
                        .WithDependence(baseAddiction: 0.22f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.12f), (DrugType.Methamphetamine, 0.92f), (DrugType.Shrooms, -0.35f), (DrugType.Cocaine, 0.28f)
                        })
                        .WithPreferredProperties(Property.Energizing, Property.Smelly, Property.Paranoia);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("charles_rowland", "kim_delaney");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new WalkToSpec { Destination = managerSpot, StartTime = 702, FaceDestinationDirection = true });
                    plan.StayInBuilding(westGasMart, 732, 179); // ~3h, ends 1 min before vending
                    plan.UseVendingMachine(912); // ~30 min break (close to vending machine)
                    plan.StayInBuilding(westGasMart, 928, 179); // ~3h
                    plan.UseVendingMachine(1108); // afternoon break
                    plan.StayInBuilding(westGasMart, 1112, 120); // ~2h (total ~8h at GasMart)
                    plan.Add(new WalkToSpec { Destination = managerSpot, StartTime = 1232, FaceDestinationDirection = false });
                    plan.StayInBuilding(room3, 1858, 764); // Room 3 overnight until next shift
                });
        }

        public RhondaVex() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 4f;
                Region = Region.Westville;

                Schedule.Enable();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"RhondaVex OnCreated failed: {ex.Message}");
                MelonLogger.Error($"StackTrace: {ex.StackTrace}");
            }
        }
    }
}
