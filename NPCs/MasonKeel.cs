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
    /// Mason Keel - rough Westville runner tied to Dean Webster and Rory Dempsey.
    /// </summary>
    public sealed class MasonKeel : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var cornerStore = Building.Get<CornerStore>();
            var thePissHut = Building.Get<ThePissHut>();
            var room3 = Building.Get<Room3>();
            Vector3 spawnPos = new Vector3(-109.5340f, -2.8350f, 54.6160f);
            Vector3 alley = new Vector3(-63.7220f, 1.0650f, 70.4180f);
            Vector3 busStop = new Vector3(-13.0495f, 1.065f, 95.5169f);

            builder.WithIdentity("mason_keel", "Mason", "Keel")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.18f;
                    av.Height = 1.04f;
                    av.Weight = 0.48f;
                    av.SkinColor = new Color(0.60f, 0.49f, 0.41f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.96f, 0.88f, 0.84f);
                    av.PupilDilation = 0.79f;
                    av.EyebrowScale = 1.11f;
                    av.EyebrowThickness = 1.04f;
                    av.EyebrowRestingHeight = -0.23f;
                    av.EyebrowRestingAngle = 2.4f;
                    av.LeftEye = (0.31f, 0.36f);
                    av.RightEye = (0.31f, 0.36f);
                    av.HairColor = new Color(0.14f, 0.11f, 0.09f);
                    av.HairPath = "Avatar/Hair/Tony/Tony";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Agitated", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Moustache", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/FlannelButtonUp", new Color(0.34f, 0.27f, 0.22f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.20f, 0.22f, 0.24f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/OpenVest/OpenVest", new Color(0.16f, 0.18f, 0.20f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(0.13f, 0.13f, 0.14f));
                    av.WithAccessoryLayer("Avatar/Accessories/Hands/Polex/Polex", new Color(0.62f, 0.62f, 0.64f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(400f, 700f)
                        .WithOrdersPerWeek(1, 4)
                        .WithPreferredOrderDay(Day.Monday)
                        .WithOrderTime(2110)
                        .WithStandards(CustomerStandard.Low)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(0f, 1f)
                        .WithCallPoliceChance(0.03f)
                        .WithDependence(0.24f, 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.20f), (DrugType.Methamphetamine, 0.58f), (DrugType.Shrooms, -0.14f), (DrugType.Cocaine, 0.32f)
                        })
                        .WithPreferredProperties(Property.Energizing, Property.Smelly, Property.Sneaky);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("dean_webster", "rory_dempsey");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(room3, 0, 389);
                    plan.Add(new WalkToSpec { Destination = alley, StartTime = 700, FaceDestinationDirection = true });
                    plan.StayInBuilding(cornerStore, 830, 119);
                    plan.Add(new WalkToSpec { Destination = busStop, StartTime = 1100, FaceDestinationDirection = true });
                    plan.StayInBuilding(thePissHut, 1315, 124);
                    plan.UseVendingMachine(1525);
                    plan.StayInBuilding(room3, 1745, 824);
                });
        }

        public MasonKeel() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.66f;
                Region = Region.Westville;
                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"MasonKeel OnCreated failed: {ex.Message}");
            }
        }
    }
}
