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
    /// Cody Lubbin - Westville teen with a cowboy hat. Hangs around Taco Ticklers and the Corner Store.
    /// </summary>
    public sealed class CodyLubbin : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var cornerStore = Building.Get<CornerStore>();
            Vector3 playZone = new Vector3(-135.7772f, -3.0349f, 44.1562f);
            Vector3 dadsWork = new Vector3(-62.4378f, 1.065f, 46.811f);
            Vector3 busStop = new Vector3(-13.0495f, 1.065f, 95.5169f);
            Vector3 spawnPos = new Vector3(-130.4420f, -2.9550f, 48.9370f);

            builder.WithIdentity("cody_lubbin", "Cody", "Lubbin")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.08f;
                    av.Height = 0.84f;
                    av.Weight = 0.44f;
                    av.SkinColor = new Color(0.66f, 0.54f, 0.44f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.99f, 0.89f, 0.84f);
                    av.PupilDilation = 0.74f;
                    av.EyebrowScale = 1.02f;
                    av.EyebrowThickness = 0.92f;
                    av.EyebrowRestingHeight = -0.12f;
                    av.EyebrowRestingAngle = 1.6f;
                    av.LeftEye = (0.29f, 0.39f);
                    av.RightEye = (0.29f, 0.39f);
                    av.HairColor = new Color(0.20f, 0.15f, 0.10f);
                    av.HairPath = "Avatar/Hair/BuzzCut/BuzzCut";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Neutral", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/Freckles", new Color(0.5f, 0.35f, 0.25f));
                    av.WithBodyLayer("Avatar/Layers/Top/Tucked T-Shirt", new Color(0.38f, 0.30f, 0.22f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.24f, 0.25f, 0.29f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.17f, 0.17f, 0.19f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/Cowboy/CowboyHat", new Color(0.25f, 0.18f, 0.12f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(200f, 500f)
                        .WithOrdersPerWeek(1, 3)
                        .WithPreferredOrderDay(Day.Saturday)
                        .WithOrderTime(1500)
                        .WithStandards(CustomerStandard.VeryLow)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(0f, 1f)
                        .WithCallPoliceChance(0.04f)
                        .WithDependence(0.05f, 1f)
                        .WithAffinities(new[] { (DrugType.Marijuana, 0.36f), (DrugType.Methamphetamine, -0.18f), (DrugType.Shrooms, 0.24f), (DrugType.Cocaine, -0.28f) })
                        .WithPreferredProperties(Property.Calming, Property.Munchies, Property.Refreshing);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(3.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("bobby_cooley", "evan_rowland");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new StayInBuildingSpec { BuildingName = "Mick's House", StartTime = 0, DurationMinutes = 449 });
                    plan.Add(new WalkToSpec { Destination = playZone, StartTime = 750, FaceDestinationDirection = true });
                    plan.StayInBuilding(cornerStore, 835, 114);
                    plan.Add(new WalkToSpec { Destination = dadsWork, StartTime = 1035, FaceDestinationDirection = true });
                    plan.UseVendingMachine(1230);
                    plan.Add(new WalkToSpec { Destination = busStop, StartTime = 1330, FaceDestinationDirection = true });
                    plan.Add(new WalkToSpec { Destination = playZone, StartTime = 1515, FaceDestinationDirection = true });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Mick's House", StartTime = 1830, DurationMinutes = 779 });
                });
        }

        public CodyLubbin() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.33f;
                Region = Region.Westville;
                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"CodyLubbin OnCreated failed: {ex.Message}");
            }
        }
    }
}
