using System;
using MelonLoader;
using MoreNPCs.Supervisor;
using S1API.Economy;
using S1API.Entities;
using S1API.Entities.Schedule;
using S1API.GameTime;
using S1API.Products;
using S1API.Properties;
using UnityEngine;

namespace MoreNPCs.NPCs
{
    public sealed class SilasMercer : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            builder.WithIdentity("silas_mercer", "Silas", "Mercer")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 1.01f;
                    av.Weight = 0.34f;
                    av.SkinColor = new Color(0.63f, 0.52f, 0.43f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.98f, 0.92f, 0.88f);
                    av.PupilDilation = 0.62f;
                    av.EyebrowScale = 1.2f;
                    av.EyebrowThickness = 1.1f;
                    av.EyebrowRestingHeight = -0.22f;
                    av.EyebrowRestingAngle = 2.4f;
                    av.LeftEye = (0.29f, 0.42f);
                    av.RightEye = (0.29f, 0.42f);
                    av.HairColor = new Color(0.16f, 0.12f, 0.09f);
                    av.HairPath = "Avatar/Hair/Peaked/Peaked";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Stubble", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/RolledButtonUp", new Color(0.22f, 0.22f, 0.24f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.14f, 0.15f, 0.18f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/DressShoes/DressShoes", new Color(0.15f, 0.15f, 0.15f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/Blazer/Blazer", new Color(0.33f, 0.24f, 0.14f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/Belt/Belt", new Color(0.15f, 0.15f, 0.15f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/RectangleFrameGlasses/RectangleFrameGlasses", new Color(0.15f, 0.15f, 0.15f));
                })
                .WithSpawnPosition(SupervisorConfig.DefaultSpawnPosition)
                .EnsureCustomer()
                .WithCustomerDefaults(cd =>
                {
                    cd.WithSpending(0f, 0f)
                        .WithOrdersPerWeek(0, 0)
                        .WithPreferredOrderDay(Day.Monday)
                        .WithOrderTime(1200)
                        .WithStandards(CustomerStandard.Low)
                        .AllowDirectApproach(true)
                        .GuaranteeFirstSample(false)
                        .WithMutualRelationRequirement(0f, 1f)
                        .WithCallPoliceChance(0.18f)
                        .WithDependence(0f, 1f)
                        .WithAffinities(new[] { (DrugType.Marijuana, 0f), (DrugType.Methamphetamine, 0f), (DrugType.Shrooms, 0f), (DrugType.Cocaine, 0f) })
                        .WithPreferredProperties();
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(4.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach);
                })
                .WithInventoryDefaults(inv =>
                {
                    inv.WithClearInventoryEachNight(false);
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                });
        }

        public SilasMercer() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 1f;
                Region = S1API.Map.Region.Uptown;
                ClearConversationCategories();
                DisableRandomInventoryGeneration();

                SupervisorRegistry.PrimarySupervisor = this;
                SupervisorDialogue.SetupFor(this, "Silas Mercer", SupervisorIds.Silas);

                Schedule.Enable();
                SupervisorActivityChain.Initialize(this, SupervisorIds.Silas);
                SupervisorIndicator.Initialize();
                SupervisorTextingSetup.Setup(this);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"SilasMercer OnCreated failed: {ex.Message}");
                MelonLogger.Error($"StackTrace: {ex.StackTrace}");
            }
        }

        private void DisableRandomInventoryGeneration()
        {
            try
            {
                var invType = FindGameType("ScheduleOne.NPCs.NPCInventory");
                if (invType == null) return;
                var inv = gameObject.GetComponent(invType) ?? gameObject.GetComponentInChildren(invType, true);
                if (inv == null) return;
                var rf = invType.GetField("RandomCash", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                var ri = invType.GetField("RandomItems", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (rf != null) rf.SetValue(inv, false);
                if (ri != null) ri.SetValue(inv, false);
            }
            catch { }
        }

        private static System.Type? FindGameType(string fullName)
        {
            foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                var t = asm.GetType(fullName);
                if (t != null) return t;
            }
            return null;
        }
    }
}

