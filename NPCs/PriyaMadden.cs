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
    /// <summary>
    /// Priya Madden - Downtown professional. Schedule uses placeholder buildings; update when provided.
    /// </summary>
    public sealed class PriyaMadden : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            Vector3 spawnPos = new Vector3(134.1605f, 6.0623f, 114.3804f);
            Vector3 plaza = new Vector3(69.7895f, 1.065f, 15.4409f);

            builder.WithIdentity("priya_madden", "Priya", "Madden")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.78f;
                    av.Height = 1.00f;
                    av.Weight = 0.38f;
                    av.SkinColor = new Color(0.60f, 0.49f, 0.39f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.98f, 0.90f, 0.86f);
                    av.PupilDilation = 0.74f;
                    av.EyebrowScale = 0.94f;
                    av.EyebrowThickness = 0.84f;
                    av.EyebrowRestingHeight = -0.06f;
                    av.EyebrowRestingAngle = 2.2f;
                    av.LeftEye = (0.29f, 0.41f);
                    av.RightEye = (0.29f, 0.41f);
                    av.HairColor = new Color(0.18f, 0.13f, 0.10f);
                    av.HairPath = "Avatar/Hair/SidePartBob/SidePartBob";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Neutral", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/EyeShadow", new Color(0.12f, 0.10f, 0.10f, 0.55f));
                    av.WithBodyLayer("Avatar/Layers/Top/Buttonup", new Color(0.42f, 0.39f, 0.34f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.27f, 0.26f, 0.29f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Flats/Flats", new Color(0.20f, 0.20f, 0.22f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/Blazer/Blazer", new Color(0.24f, 0.28f, 0.33f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/RectangleFrameGlasses/RectangleFrameGlasses", new Color(0.15f, 0.15f, 0.17f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(500f, 700f)
                        .WithOrdersPerWeek(1, 4)
                        .WithPreferredOrderDay(Day.Tuesday)
                        .WithOrderTime(1830)
                        .WithStandards(CustomerStandard.Moderate)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(0f, 1f)
                        .WithCallPoliceChance(0.20f)
                        .WithDependence(0.10f, 1f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, 0.36f), (DrugType.Methamphetamine, 0.06f), (DrugType.Shrooms, 0.24f), (DrugType.Cocaine, -0.18f)
                        })
                        .WithPreferredProperties(Property.Refreshing, Property.Calming, Property.Glowie);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("jennifer_rivera", "eugene_buckley");
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new StayInBuildingSpec { BuildingName = "Tall Tower", StartTime = 0852, DurationMinutes = 149 });
                    plan.Add(new WalkToSpec { Destination = plaza, StartTime = 1117, FaceDestinationDirection = true });
                    plan.Add(new StayInBuildingSpec { BuildingName = "Cafe", StartTime = 1252, DurationMinutes = 104 });
                    plan.UseVendingMachine(1532);
                    plan.Add(new StayInBuildingSpec { BuildingName = "Apartment Buiding", StartTime = 1937, DurationMinutes = 524 });
                });
        }

        public PriyaMadden() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.47f;
                Region = Region.Downtown;
                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"PriyaMadden OnCreated failed: {ex.Message}");
            }
        }
    }
}

