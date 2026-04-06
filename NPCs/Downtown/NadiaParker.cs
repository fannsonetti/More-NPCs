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
    public sealed class NadiaParker : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var supermarket = Building.Get<Supermarket>();
            var slopShop = Building.Get<SlopShop>();
            var cafe = Building.Get<Cafe>();
            Vector3 townCenter = new Vector3(69.7895f, 1.065f, 15.4409f);
            Vector3 spawnPos = new Vector3(122.4035f, 6.0615f, 105.2812f);

            builder.WithIdentity("nadia_parker", "Nadia", "Parker")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.90f;
                    av.Height = 0.98f;
                    av.Weight = 0.35f;
                    av.SkinColor = new Color(0.64f, 0.52f, 0.44f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.93f, 0.88f, 0.84f);
                    av.PupilDilation = 0.52f;
                    av.EyebrowScale = 1.05f;
                    av.EyebrowThickness = 0.92f;
                    av.EyebrowRestingHeight = -0.18f;
                    av.EyebrowRestingAngle = 3.1f;
                    av.LeftEye = (0.28f, 0.38f);
                    av.RightEye = (0.28f, 0.38f);
                    av.HairColor = new Color(0.24f, 0.18f, 0.13f);
                    av.HairPath = "Avatar/Hair/lowbun/LowBun";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_NeutralPout", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/EyeShadow", new Color(0.10f, 0.08f, 0.08f));
                    av.WithBodyLayer("Avatar/Layers/Top/RolledButtonUp", new Color(0.43f, 0.50f, 0.62f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.24f, 0.26f, 0.30f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Flats/Flats", new Color(0.18f, 0.18f, 0.20f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/SmallRoundGlasses/SmallRoundGlasses", new Color(0.16f, 0.16f, 0.17f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(500f, 800f)
                        .WithOrdersPerWeek(1, 3)
                        .WithPreferredOrderDay(Day.Tuesday)
                        .WithOrderTime(1930)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(0f, 1f)
                        .WithCallPoliceChance(0.22f)
                        .WithDependence(0.16f, 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.28f), (DrugType.Methamphetamine, 0.04f), (DrugType.Shrooms, 0.20f), (DrugType.Cocaine, -0.08f)
                        })
                        .WithPreferredProperties(Property.Refreshing, Property.Focused, Property.Sneaky);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .WithConnectionsById("eugene_buckley", "gavin_holt")
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach);
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    // Offset from Rhea Larkin (cafe 900 / slop 1115) — no concurrent cafe or slop overlap
                    plan.StayInBuilding(cafe, 1050, 153);
                    plan.StayInBuilding(slopShop, 1324, 124);
                    plan.Add(new WalkToSpec { Destination = townCenter, StartTime = 1530, FaceDestinationDirection = true });
                    plan.UseATM(1625);
                    plan.StayInBuilding(supermarket, 1710, 179);
                    plan.StayInBuilding(supermarket, 2040, 329);
                });
        }

        public NadiaParker() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.46f;
                Region = Region.Downtown;
                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"NadiaParker OnCreated failed: {ex.Message}");
            }
        }
    }
}
