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
    public sealed class ThomasAshford : NPC
    {
        public override bool IsPhysical => true;

        public static readonly Vector3 BankFallbackIdlePosition = new Vector3(73.4736f, 0.9662f, 39.0171f);

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            builder.WithIdentity("thomas_ashford", "Thomas", "Ashford")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 1.0f;
                    av.Weight = 0.42f;
                    av.SkinColor = new Color(0.52f, 0.42f, 0.35f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.98f, 0.92f, 0.88f);
                    av.PupilDilation = 0.62f;
                    av.EyebrowScale = 1.08f;
                    av.EyebrowThickness = 0.9f;
                    av.EyebrowRestingHeight = -0.12f;
                    av.EyebrowRestingAngle = 1.5f;
                    av.LeftEye = (0.28f, 0.38f);
                    av.RightEye = (0.28f, 0.38f);
                    av.HairColor = new Color(0.22f, 0.16f, 0.12f);
                    av.HairPath = "Avatar/Hair/Receding/Receding";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_NeutralPout", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/Buttonup", new Color(0.94f, 0.94f, 0.96f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.18f, 0.18f, 0.2f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.15f, 0.15f, 0.17f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/RectangleFrameGlasses/RectangleFrameGlasses", new Color(0.2f, 0.2f, 0.22f));
                    av.WithAccessoryLayer("Avatar/Accessories/Hands/Polex/Polex", new Color(0.85f, 0.72f, 0.35f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/Belt/Belt", new Color(0.12f, 0.12f, 0.12f));
                })
                .WithSpawnPosition(BankFallbackIdlePosition)
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
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach)
                        .WithConnectionsById("");
                })
                .WithSchedule(plan => plan.EnsureDealSignal());
        }

        public ThomasAshford() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Aggressiveness = 0.42f; // manager, calm
                Region = S1API.Map.Region.Suburbia;
                Manager.ManagerDialogue.SetupFor(this);
                Manager.ManagerIndicator.Initialize();
                Manager.ManagerLaunderingChain.Initialize(this);
                Manager.ManagerTextingSetup.Setup(this);
                Schedule.Enable();
                MelonCoroutines.Start(Utils.NPCIdleLocations.MoveThomasToIdleWhenReady(this));
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"ThomasAshford OnCreated failed: {ex.Message}");
            }
        }
    }
}

