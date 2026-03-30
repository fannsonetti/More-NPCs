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
    /// Lena Hart - Northtown resident who spends most of her day around downtown offices and cafes.
    /// Connected to Kathy Henderson and Donna Martin.
    /// </summary>
    public sealed class LenaHart : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var northApartments = Building.Get<NorthApartments>();
            var townHall = Building.Get<TownHall>();
            var cafe = Building.Get<Cafe>();
            var slopShop = Building.Get<SlopShop>();
            var hylandBank = Building.Get<HylandBank>();
            Vector3 spawnPos = new Vector3(-25.6521f, -3.0349f, 154.5471f);
            Vector3 plaza = new Vector3(69.7895f, 1.065f, 15.4409f);

            builder.WithIdentity("lena_hart", "Lena", "Hart")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.84f;
                    av.Height = 0.98f;
                    av.Weight = 0.36f;
                    av.SkinColor = new Color(0.69f, 0.57f, 0.48f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.99f, 0.92f, 0.88f);
                    av.PupilDilation = 0.66f;
                    av.EyebrowScale = 0.94f;
                    av.EyebrowThickness = 0.84f;
                    av.EyebrowRestingHeight = -0.02f;
                    av.EyebrowRestingAngle = 1.8f;
                    av.LeftEye = (0.30f, 0.41f);
                    av.RightEye = (0.30f, 0.41f);
                    av.HairColor = new Color(0.37f, 0.25f, 0.18f);
                    av.HairPath = "Avatar/Hair/SidePartBob/SidePartBob";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Neutral", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/Buttonup", new Color(0.49f, 0.46f, 0.40f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.26f, 0.27f, 0.31f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Flats/Flats", new Color(0.22f, 0.22f, 0.24f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/Blazer/Blazer", new Color(0.28f, 0.31f, 0.36f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/RectangleFrameGlasses/RectangleFrameGlasses", new Color(0.14f, 0.14f, 0.16f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 400f, maxWeekly: 900f)
                        .WithOrdersPerWeek(1, 3)
                        .WithPreferredOrderDay(Day.Wednesday)
                        .WithOrderTime(1730)
                        .WithStandards(CustomerStandard.Low)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.15f)
                        .WithDependence(baseAddiction: 0.08f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.18f), (DrugType.Methamphetamine, -0.21f), (DrugType.Shrooms, 0.39f), (DrugType.Cocaine, 0.29f)
                        })
                        .WithPreferredProperties(Property.Refreshing, Property.Calming, Property.ThoughtProvoking);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("kathy_henderson", "donna_martin");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(northApartments, 0014, 419);
                    plan.StayInBuilding(townHall, 0844, 149);
                    plan.StayInBuilding(cafe, 1054, 94);
                    plan.UseATM(1229);
                    plan.Add(new WalkToSpec { Destination = plaza, StartTime = 1334, FaceDestinationDirection = true });
                    plan.StayInBuilding(slopShop, 1534, 99);
                    plan.StayInBuilding(hylandBank, 1714, 94);
                    plan.StayInBuilding(townHall, 1849, 139);
                    plan.StayInBuilding(northApartments, 2214, 239);
                });
        }

        public LenaHart() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 0.51f;
                Region = Region.Northtown;

                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"LenaHart OnCreated failed: {ex.Message}");
            }
        }
    }
}

