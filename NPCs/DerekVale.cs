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
    /// Derek Vale - hippie Northtown regular who loves shrooms. Lives in the Shack. Day: Taco Ticklers, Piss Hut, Shack, Community Center. Connected to Jason Reed and Austin Steiner.
    /// </summary>
    public sealed class DerekVale : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var shack = Building.Get<Shack>();
            var thePissHut = Building.Get<ThePissHut>();
            var communityCenter = Building.Get<CommunityCenter>();
            Vector3 spawnPos = new Vector3(-36.3346f, 1.065f, 75.6414f);

            builder.WithIdentity("derek_vale", "Derek", "Vale")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.12f;
                    av.Height = 0.97f;
                    av.Weight = 0.48f;
                    av.SkinColor = new Color(0.62f, 0.52f, 0.42f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.96f, 0.90f, 0.84f);
                    av.PupilDilation = 0.76f;
                    av.EyebrowScale = 1.12f;
                    av.EyebrowThickness = 0.88f;
                    av.EyebrowRestingHeight = -0.12f;
                    av.EyebrowRestingAngle = 2.2f;
                    av.LeftEye = (0.26f, 0.38f);
                    av.RightEye = (0.26f, 0.38f);
                    av.HairColor = new Color(0.45f, 0.32f, 0.20f);
                    av.HairPath = "Avatar/Hair/LongCurly/LongCurly";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Goatee", new Color(0.35f, 0.28f, 0.20f));
                    av.WithBodyLayer("Avatar/Layers/Top/FlannelButtonUp", new Color(0.55f, 0.42f, 0.32f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jorts", new Color(0.28f, 0.26f, 0.22f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sandals/Sandals", new Color(0.38f, 0.30f, 0.22f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/BucketHat/BucketHat", new Color(0.22f, 0.35f, 0.18f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 300f, maxWeekly: 700f)
                        .WithOrdersPerWeek(2, 4)
                        .WithPreferredOrderDay(Day.Monday)
                        .WithOrderTime(2015)
                        .WithStandards(CustomerStandard.Low)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.15f)
                        .WithDependence(baseAddiction: 0.14f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.68f), (DrugType.Methamphetamine, -0.18f), (DrugType.Shrooms, 0.92f), (DrugType.Cocaine, -0.32f)
                        })
                        .WithPreferredProperties(Property.Calming, Property.Euphoric, Property.Glowie);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("jason_reed", "austin_steiner");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(shack, 000, 479);
                    plan.Add(new SitSpec { SeatSetPath = "Map/Hyland Point/Businesses/Taco Ticklers/Fast Food Booth (1)/fast food booth/Seat (1)", StartTime = 800, DurationMinutes = 109 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "The Piss Hut", StartTime = 1010, DurationMinutes = 119 });
                    plan.StayInBuilding(shack, 1155, 104);
                    plan.StayInBuilding(communityCenter, 1300, 120);
                    plan.StayInBuilding(shack, 1425, 1050);
                });
        }

        public DerekVale() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 0.52f;
                Region = Region.Northtown;

                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"DerekVale OnCreated failed: {ex.Message}");
            }
        }
    }
}

