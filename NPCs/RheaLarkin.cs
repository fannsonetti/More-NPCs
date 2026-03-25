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
    public sealed class RheaLarkin : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var supermarket = Building.Get<Supermarket>();
            var cafe = Building.Get<Cafe>();
            var slopShop = Building.Get<SlopShop>();
            Vector3 downtownBench = new Vector3(69.7895f, 1.065f, 15.4409f);
            Vector3 spawnPos = new Vector3(128.5441f, 6.0615f, 108.1199f);

            builder.WithIdentity("rhea_larkin", "Rhea", "Larkin")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.91f;
                    av.Height = 0.99f;
                    av.Weight = 0.34f;
                    av.SkinColor = new Color(0.66f, 0.54f, 0.45f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.98f, 0.91f, 0.87f);
                    av.PupilDilation = 0.64f;
                    av.EyebrowScale = 0.98f;
                    av.EyebrowThickness = 0.86f;
                    av.EyebrowRestingHeight = -0.08f;
                    av.EyebrowRestingAngle = 2.1f;
                    av.LeftEye = (0.30f, 0.41f);
                    av.RightEye = (0.30f, 0.41f);
                    av.HairColor = new Color(0.30f, 0.22f, 0.16f);
                    av.HairPath = "Avatar/Hair/MidFringe/MidFringe";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/Buttonup", new Color(0.46f, 0.41f, 0.36f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.25f, 0.27f, 0.32f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Flats/Flats", new Color(0.18f, 0.18f, 0.20f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/SmallRoundGlasses/SmallRoundGlasses", new Color(0.16f, 0.16f, 0.17f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(500f, 800f)
                        .WithOrdersPerWeek(1, 3)
                        .WithPreferredOrderDay(Day.Thursday)
                        .WithOrderTime(2000)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(0f, 1f)
                        .WithCallPoliceChance(0.22f)
                        .WithDependence(0.14f, 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.24f), (DrugType.Methamphetamine, 0.08f), (DrugType.Shrooms, 0.22f), (DrugType.Cocaine, -0.10f)
                        })
                        .WithPreferredProperties(Property.Refreshing, Property.Focused, Property.Glowie);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("jennifer_rivera", "sienna_crowley");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(cafe, 900, 99);
                    plan.StayInBuilding(slopShop, 1115, 109);
                    plan.Add(new WalkToSpec { Destination = downtownBench, StartTime = 1320, FaceDestinationDirection = true });
                    plan.UseATM(1510);
                    plan.StayInBuilding(supermarket, 1700, 109);
                    plan.StayInBuilding(supermarket, 2000, 359);
                });
        }

        public RheaLarkin() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.48f;
                Region = Region.Downtown;
                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"RheaLarkin OnCreated failed: {ex.Message}");
            }
        }
    }
}
