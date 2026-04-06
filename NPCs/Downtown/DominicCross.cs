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
    public sealed class DominicCross : NPC
    {
        public override bool IsPhysical => true;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            builder.WithIdentity("dominic_cross", "Dominic", "Cross")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 1.01f;
                    av.Weight = 0.42f;
                    av.SkinColor = new Color(0.58f, 0.48f, 0.40f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.98f, 0.92f, 0.88f);
                    av.PupilDilation = 0.68f;
                    av.EyebrowScale = 1.15f;
                    av.EyebrowThickness = 1.25f;
                    av.EyebrowRestingHeight = -0.35f;
                    av.EyebrowRestingAngle = 3.8f;
                    av.LeftEye = (0.28f, 0.38f);
                    av.RightEye = (0.28f, 0.38f);
                    av.HairColor = new Color(0.12f, 0.09f, 0.06f);
                    av.HairPath = "Avatar/Hair/BuzzCut/BuzzCut";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SmugPout", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Stubble", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/FlannelButtonUp", new Color(0.28f, 0.18f, 0.14f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.18f, 0.16f, 0.14f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(0.12f, 0.12f, 0.12f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/OpenVest/OpenVest", new Color(0.22f, 0.22f, 0.24f));
                    av.WithAccessoryLayer("Avatar/Accessories/Neck/GoldChain/GoldChain", new Color(0.85f, 0.72f, 0.35f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/FlatCap/FlatCap", new Color(0.22f, 0.22f, 0.24f));
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
                    r.WithDelta(2.0f)
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

        public DominicCross() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 1f;
                Region = S1API.Map.Region.Downtown;
                ClearConversationCategories();
                DisableRandomInventoryGeneration();

                Schedule.Enable();
                SupervisorDialogue.SetupFor(this, "Dominic Cross", SupervisorIds.Dominic);
                SupervisorActivityChain.Initialize(this, SupervisorIds.Dominic);
                SupervisorIndicator.Initialize();
                SupervisorTextingSetup.SetupForDominic(this);
                MelonCoroutines.Start(Utils.NPCIdleLocations.MoveSupervisorToIdleWhenReady(this, SupervisorIds.Dominic));
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"DominicCross OnCreated failed: {ex.Message}");
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

