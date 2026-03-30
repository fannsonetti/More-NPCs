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
    /// Bryce Sherman - Happy Westville kid. Connected to Victor Hughes and Bobby Cooley.
    /// </summary>
    public sealed class BryceSherman : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var shermanHouse = Building.Get<ShermanHouse>();
            var cornerStore = Building.Get<CornerStore>();
            Vector3 westGasmart = new Vector3(-113.1828f, -2.835f, 61.2241f);
            Vector3 spawnPos = new Vector3(-60.5784f, 1.065f, 80.3446f);

            builder.WithIdentity("bryce_sherman", "Bryce", "Sherman")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 0.82f;
                    av.Weight = 0.38f;
                    av.SkinColor = new Color(0.3755f, 0.2928f, 0.2257f);
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
                    av.HairColor = new Color(0.08f, 0.06f, 0.05f);
                    av.HairPath = "Avatar/Hair/Afro/Afro";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/Freckles", new Color(0.23f, 0.17f, 0.12f));
                    av.WithBodyLayer("Avatar/Layers/Top/Tucked T-Shirt", new Color(0.8036f, 0.4971f, 0.2673f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.10f, 0.10f, 0.12f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.12f, 0.12f, 0.14f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(50f, 250f)
                        .WithOrdersPerWeek(1, 2)
                        .WithPreferredOrderDay(Day.Friday)
                        .WithOrderTime(1630)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(0f, 1f)
                        .WithCallPoliceChance(0.18f)
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
                    plan.StayInBuilding(cornerStore, 0825, 89);
                    plan.Add(new WalkToSpec { Destination = westGasmart, StartTime = 1010, FaceDestinationDirection = true });
                    plan.StayInBuilding(shermanHouse, 1140, 119);
                    plan.UseVendingMachine(1325);
                    plan.StayInBuilding(cornerStore, 1410, 104);
                    plan.StayInBuilding(shermanHouse, 1725, 547);
                });
        }

        public BryceSherman() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.22f;
                Region = Region.Westville;
                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"BryceSherman OnCreated failed: {ex.Message}");
            }
        }
    }
}

