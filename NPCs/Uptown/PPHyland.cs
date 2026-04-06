using MelonLoader;
using MoreNPCs.Utils;
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
    /// P.P. Hyland — no last name; Tony hair (slightly darker than skin), no brows; gold chain &amp; Polex; mouth/wrinkles like Grunk.
    /// </summary>
    public sealed class PPHyland : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            Vector3 spawnPos = new Vector3(135.8259f, 1.238f, 34.5744f);

            var gold = new Color(0.92f, 0.78f, 0.28f);

            builder.WithIdentity("pp_hyland", "P.P. Hyland", "")
                .WithAppearanceDefaults(av =>
                {
                    var skin = new Color(117f / 255f, 122f / 255f, 92f / 255f);
                    av.Gender = 0.15f;
                    av.Height = 1.02f;
                    av.Weight = 1f;
                    av.SkinColor = skin;
                    av.LeftEyeLidColor = skin;
                    av.RightEyeLidColor = skin;
                    av.EyeBallTint = Color.Lerp(new Color(0.96f, 0.62f, 0.74f), new Color(0.98f, 0.96f, 0.95f), 0.45f);
                    av.PupilDilation = 0f;
                    av.EyebrowScale = 0f;
                    av.EyebrowThickness = 0f;
                    av.EyebrowRestingHeight = 0f;
                    av.EyebrowRestingAngle = 0f;
                    av.LeftEye = (0.65f, 0.32f);
                    av.RightEye = (0.40f, 0.32f);
                    av.HairPath = "Avatar/Hair/Tony/Tony";
                    av.HairColor = new Color(skin.r * 0.9f, skin.g * 0.9f, skin.b * 0.9f);

                    av.WithFaceLayer("Avatar/Layers/Face/Face_OpenMouthSmile", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/TiredEyes", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/OldPersonWrinkles", new Color(0.957f, 0.474f, 0.938f));
                    av.WithFaceLayer("Avatar/Layers/Face/Eyeshadow", new Color(0f, 0f, 0f, 0.55f));
                    av.WithBodyLayer("Avatar/Layers/Top/Nipples", skin);
                    av.WithBodyLayer("Avatar/Layers/Bottom/MaleUnderwear", new Color(0.28f, 0.30f, 0.26f));
                    av.WithBodyLayer("Avatar/Layers/Top/RolledButtonUp", new Color(0.18f, 0.19f, 0.17f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.14f, 0.15f, 0.13f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/Blazer/Blazer", new Color(0.12f, 0.13f, 0.11f));
                    av.WithAccessoryLayer("Avatar/Accessories/Neck/GoldChain/GoldChain", gold);
                    av.WithAccessoryLayer("Avatar/Accessories/Hands/Polex/Polex", gold);
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/DressShoes/DressShoes", new Color(0.08f, 0.08f, 0.09f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(10000f, 20000f)
                        .WithOrdersPerWeek(1, 1)
                        .WithPreferredOrderDay(Day.Wednesday)
                        .WithOrderTime(300)
                        .WithStandards(CustomerStandard.VeryHigh)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(3.5f, 5.5f)
                        .WithCallPoliceChance(0.12f)
                        .WithDependence(0.05f, 0.9f)
                        .WithAffinities(new[]
                        {
                            (DrugType.Marijuana, -0.95f),
                            (DrugType.Shrooms, -0.92f),
                            (DrugType.Cocaine, -0.97f),
                            (DrugType.Methamphetamine, 0.86f)
                        })
                        .WithPreferredProperties(
                            Property.Electrifying, Property.Energizing, Property.Focused, Property.Refreshing);
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(1.5f)
                        .SetUnlocked(false)
                        .WithConnectionsById("")
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach);
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.Add(new StayInBuildingSpec { BuildingName = "PPGrave", StartTime = 0500, DurationMinutes = 1439 });
                });
        }

        public PPHyland() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                ClearConversationCategories();
                PPHylandDialogue.SetupFor(this);
                Aggressiveness = 0.22f;
                Region = Region.Uptown;
                Schedule.Enable();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"PPHyland OnCreated failed: {ex.Message}");
            }
        }
    }
}
