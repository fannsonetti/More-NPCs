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
    /// Marcus Sherman - Kid living in the Trent house with gangsters. Connected to Victor Hughes and Bobby Cooley. Moderate standards but limited budget. 100% affinity for all drugs.
    /// </summary>
    public sealed class BryceSherman : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var shermanHouse = Building.Get<ShermanHouse>();
            var cornerStore = Building.Get<CornerStore>();
            var thePissHut = Building.Get<ThePissHut>();
            Vector3 westGasmart = new Vector3(-113.1828f, -2.835f, 61.2241f);
            Vector3 spawnPos = new Vector3(-60.5784f, 1.065f, 80.3446f);

            builder.WithIdentity("bryce_sherman", "Bryce", "Sherman")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 0.82f;
                    av.Weight = 0.38f;
                    av.SkinColor = new Color(0.58f, 0.48f, 0.40f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.98f, 0.90f, 0.85f);
                    av.PupilDilation = 0.76f;
                    av.EyebrowScale = 1.0f;
                    av.EyebrowThickness = 0.88f;
                    av.EyebrowRestingHeight = -0.14f;
                    av.EyebrowRestingAngle = 1.6f;
                    av.LeftEye = (0.28f, 0.38f);
                    av.RightEye = (0.28f, 0.38f);
                    av.HairColor = new Color(0.16f, 0.12f, 0.08f);
                    av.HairPath = "Avatar/Hair/BuzzCut/BuzzCut";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Neutral", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/Tucked T-Shirt", new Color(0.32f, 0.34f, 0.36f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.22f, 0.24f, 0.28f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.18f, 0.18f, 0.20f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(150f, 350f)
                        .WithOrdersPerWeek(1, 2)
                        .WithPreferredOrderDay(Day.Friday)
                        .WithOrderTime(1630)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(0f, 1f)
                        .WithCallPoliceChance(0.03f)
                        .WithDependence(0.0f, 1f)
                        .WithAffinities(new[] { (DrugType.Marijuana, 1f), (DrugType.Methamphetamine, 1f), (DrugType.Shrooms, 1f), (DrugType.Cocaine, 1f) })
                        .WithPreferredProperties();
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(3.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("victor_hughes", "bobby_cooley");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(cornerStore, 830, 89);
                    plan.Add(new WalkToSpec { Destination = westGasmart, StartTime = 1015, FaceDestinationDirection = true });
                    plan.StayInBuilding(thePissHut, 1145, 119);
                    plan.UseVendingMachine(1330);
                    plan.StayInBuilding(cornerStore, 1415, 104);
                    plan.StayInBuilding(shermanHouse, 1730, 547);
                });
        }

        public BryceSherman() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.42f;
                Region = Region.Westville;
                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"MarcusSherman OnCreated failed: {ex.Message}");
            }
        }
    }
}
