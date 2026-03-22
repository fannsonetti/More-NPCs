using MelonLoader;
using S1API.Economy;
using S1API.Entities;
using S1API.Entities.Schedule;
using S1API.GameTime;
using S1API.Map;
using S1API.Products;
using S1API.Properties;
using UnityEngine;

namespace MoreNPCs.NPCs
{
    public sealed class WesleyPike : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            Vector3 spawnPos = new Vector3(-60.5784f, 1.065f, 80.3446f);
            Vector3 gasMart = new Vector3(-113.1828f, -2.835f, 61.2241f);

            builder.WithIdentity("wesley_pike", "Wesley", "Pike")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.26f;
                    av.Height = 1.00f;
                    av.Weight = 0.42f;
                    av.SkinColor = new Color(0.66f, 0.54f, 0.45f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.98f, 0.90f, 0.85f);
                    av.PupilDilation = 0.71f;
                    av.EyebrowScale = 1.07f;
                    av.EyebrowThickness = 0.95f;
                    av.EyebrowRestingHeight = -0.16f;
                    av.EyebrowRestingAngle = 1.8f;
                    av.LeftEye = (0.28f, 0.39f);
                    av.RightEye = (0.28f, 0.39f);
                    av.HairColor = new Color(0.16f, 0.12f, 0.09f);
                    av.HairPath = "Avatar/Hair/Peaked/Peaked";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Neutral", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Goatee", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/Buttonup", new Color(0.52f, 0.32f, 0.28f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.23f, 0.24f, 0.26f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/DressShoes/DressShoes", new Color(0.17f, 0.17f, 0.19f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/Belt/Belt", new Color(0.16f, 0.16f, 0.16f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/FlatCap/FlatCap", new Color(0.29f, 0.29f, 0.31f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(500f, 700f)
                        .WithOrdersPerWeek(1, 4)
                        .WithPreferredOrderDay(Day.Thursday)
                        .WithOrderTime(2100)
                        .WithStandards(CustomerStandard.Low)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(0f, 1f)
                        .WithCallPoliceChance(0.04f)
                        .WithDependence(0.14f, 1f)
                        .WithAffinities(new[] { (DrugType.Marijuana, 0.24f), (DrugType.Methamphetamine, 0.22f), (DrugType.Shrooms, -0.14f), (DrugType.Cocaine, 0.05f) })
                        .WithPreferredProperties(Property.Energizing, Property.Foggy, Property.Smelly);
                })
                .WithRelationshipDefaults(r => r.WithDelta(2.0f).SetUnlocked(false).SetUnlockType(NPCRelationship.UnlockType.DirectApproach).WithConnectionsById("kim_delaney", "rhonda_vex"))
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new StayInBuildingSpec { BuildingName = "Jane's Caravan", StartTime = 830, DurationMinutes = 104 });
                    plan.Add(new WalkToSpec { Destination = gasMart, StartTime = 1015, FaceDestinationDirection = true });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Corner Store", StartTime = 1145, DurationMinutes = 119 });
                    plan.Add(new StayInBuildingSpec { BuildingName = "The Piss Hut", StartTime = 1325, DurationMinutes = 99 });
                    plan.UseATM(1645);
                    plan.Add(new StayInBuildingSpec { BuildingName = "Room 5", StartTime = 1950, DurationMinutes = 670 }); // until 7am, no 4am kickout
                });
        }

        public WesleyPike() : base() { }

        protected override void OnCreated()
        {
            try { base.OnCreated(); Appearance.Build(); Aggressiveness = 0.41f; Region = Region.Westville; Schedule.Enable(); }
            catch (System.Exception ex) { MelonLogger.Error($"WesleyPike OnCreated failed: {ex.Message}"); }
        }
    }
}
