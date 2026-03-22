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
    /// Owen Crowe - Northtown burnout who drifts between Peter's Room, the motel office, Bud's Bar, and Kyle's place.
    /// Connected to Kyle Cooley and Jason Reed.
    /// </summary>
    public sealed class OwenCrowe : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var petersRoom = Building.Get<PetersRoom>();
            var motelOffice = Building.Get<MotelOffice>();
            var budsBar = Building.Get<BudsBar>();
            Vector3 spawnPos = new Vector3(-71.2847f, -2.935f, 145.6754f); // swapped with Wayne
            Vector3 northWaterfront = new Vector3(-49.5478f, -4.035f, 168.5777f);

            builder.WithIdentity("owen_crowe", "Owen", "Crowe")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.12f;
                    av.Height = 1.01f;
                    av.Weight = 0.43f;
                    av.SkinColor = new Color(0.57f, 0.48f, 0.39f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.97f, 0.89f, 0.83f);
                    av.PupilDilation = 0.74f;
                    av.EyebrowScale = 1.12f;
                    av.EyebrowThickness = 1.18f;
                    av.EyebrowRestingHeight = -0.26f;
                    av.EyebrowRestingAngle = 3.1f;
                    av.LeftEye = (0.27f, 0.38f);
                    av.RightEye = (0.27f, 0.38f);
                    av.HairColor = new Color(0.14f, 0.11f, 0.09f);
                    av.HairPath = "Avatar/Hair/Receding/Receding";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SmugPout", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Stubble", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/FlannelButtonUp", new Color(0.26f, 0.22f, 0.18f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.19f, 0.19f, 0.21f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(0.12f, 0.12f, 0.12f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/OpenVest/OpenVest", new Color(0.22f, 0.22f, 0.24f));
                    av.WithAccessoryLayer("Avatar/Accessories/Hands/Polex/Polex", new Color(0.58f, 0.58f, 0.60f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(minWeekly: 200f, maxWeekly: 500f)
                        .WithOrdersPerWeek(1, 3)
                        .WithPreferredOrderDay(Day.Sunday)
                        .WithOrderTime(2215)
                        .WithStandards(CustomerStandard.Low)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(minAt50: 2.5f, maxAt100: 4.0f)
                        .WithCallPoliceChance(0.01f)
                        .WithDependence(baseAddiction: 0.26f, dependenceMultiplier: 1.0f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.22f), (DrugType.Methamphetamine, 0.87f), (DrugType.Shrooms, -0.44f), (DrugType.Cocaine, -0.18f)
                        })
                        .WithPreferredProperties(Property.Energizing, Property.Smelly, Property.ThoughtProvoking);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("kyle_cooley", "jason_reed");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(motelOffice, 0, 390); // overnight at Motel Office (swapped with Wayne)
                    plan.UseVendingMachine(920);
                    plan.StayInBuilding(petersRoom, 1005, 96);
                    plan.Add(new WalkToSpec { Destination = northWaterfront, StartTime = 1205, FaceDestinationDirection = true });
                    plan.StayInBuilding(budsBar, 1430, 124);
                    plan.UseATM(1625);
                    plan.StayInBuilding(motelOffice, 1715, 91);
                    plan.StayInBuilding(motelOffice, 2145, 294); // overnight at Motel Office (swapped with Wayne)
                });
        }

        public OwenCrowe() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                Aggressiveness = 0.65f;
                Region = Region.Northtown;

                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"OwenCrowe OnCreated failed: {ex.Message}");
            }
        }
    }
}
