using System;
using MelonLoader;
using S1API.Economy;
using S1API.Entities;
using S1API.Entities.Schedule;
using S1API.Map;
using S1API.Map.Buildings;
using UnityEngine;

namespace MoreNPCs.NPCs
{
    /// <summary>Downtown dealer — emo: hair matches beanie, layered heavy eyeshadow, very dark lids, tired eyes, teardrop tattoo, silver jewelry, dark outfit.</summary>
    public sealed class BrookeWalsh : NPC
    {
        public override bool IsPhysical => true;
        public override bool IsDealer => true;

        private Action _dealerRecruitedHandler;
        private Action _dealerContractAcceptedHandler;
        private Action _dealerRecommendedHandler;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var home = Building.GetByName("Apartment Building 2");
            Vector3 spawnPos = new Vector3(-0.2f, 0.72f, 72.2f);

            builder.WithIdentity("brooke_walsh", "Brooke", "Walsh")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.96f;
                    av.Height = 1.01f;
                    av.Weight = 0.38f;
                    var skin = new Color(0.62f, 0.50f, 0.44f);
                    av.SkinColor = skin;
                    var eyelidBlack = new Color(0.02f, 0.02f, 0.023f);
                    av.LeftEyeLidColor = eyelidBlack;
                    av.RightEyeLidColor = eyelidBlack;
                    av.EyeBallTint = new Color(0.92f, 0.86f, 0.88f);
                    av.PupilDilation = 0.36f;
                    av.EyebrowScale = 0.94f;
                    av.EyebrowThickness = 0.82f;
                    av.EyebrowRestingHeight = -0.14f;
                    av.EyebrowRestingAngle = 1.4f;
                    av.LeftEye = (0.40f, 0.10f);
                    av.RightEye = (0.40f, 0.10f);
                    var beanieBlack = new Color(0.06f, 0.06f, 0.07f);
                    av.HairColor = beanieBlack;
                    av.HairPath = "Avatar/Hair/MessyBob/MessyBob";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Agitated", Color.black);
                    var shadowBase = new Color(0.08f, 0.08f, 0.082f, 1f);
                    av.WithFaceLayer("Avatar/Layers/Face/EyeShadow", shadowBase);
                    av.WithFaceLayer("Avatar/Layers/Face/EyeShadow", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/TiredEyes", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Tattoos/face/Face_Teardrop", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/Nipples", skin);
                    av.WithBodyLayer("Avatar/Layers/Bottom/FemaleUnderwear", new Color(0.74f, 0.70f, 0.68f));
                    av.WithBodyLayer("Avatar/Layers/Top/Tucked T-Shirt", new Color(0.07f, 0.07f, 0.08f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.09f, 0.09f, 0.11f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/OpenVest/OpenVest", new Color(0.11f, 0.11f, 0.13f));
                    var silver = new Color(0.72f, 0.72f, 0.74f);
                    av.WithAccessoryLayer("Avatar/Accessories/Neck/GoldChain/GoldChain", silver);
                    av.WithAccessoryLayer("Avatar/Accessories/Hands/Polex/Polex", silver);
                    av.WithBodyLayer("Avatar/Layers/Accessories/FingerlessGloves", new Color(0.07f, 0.07f, 0.09f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/Beanie/Beanie", beanieBlack);
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/Belt/Belt", new Color(0.09f, 0.09f, 0.10f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(0.08f, 0.08f, 0.09f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureDealer()
                .WithDealerDefaults(dd =>
                {
                    dd.WithSigningFee(2000f)
                        .WithCut(0.20f)
                        .WithDealerType(DealerType.PlayerDealer)
                        .WithHome(home)
                        .AllowInsufficientQuality(false)
                        .AllowExcessQuality(true)
                        .WithCompletedDealsVariable("dealer_completed_deals");
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .WithConnectionsById("joseph_wilkinson")
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach);
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(home, 0016, 1439);
                });
        }

        public BrookeWalsh() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Dealer.Home = Building.GetByName("Apartment Building 2");
                WireDealerEvents();
                Aggressiveness = 0.58f;
                Region = Region.Downtown;
                Schedule.Enable();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"BrookeWalsh OnCreated failed: {ex.Message}");
            }
        }

        protected override void OnDestroyed()
        {
            base.OnDestroyed();
            UnwireDealerEvents();
        }

        private void WireDealerEvents()
        {
            if (Dealer == null) { MelonLogger.Warning($"Dealer component missing for {ID}; cannot wire dealer events."); return; }
            _dealerRecruitedHandler ??= HandleDealerRecruited;
            _dealerContractAcceptedHandler ??= HandleContractAccepted;
            _dealerRecommendedHandler ??= HandleDealerRecommended;
            Dealer.OnRecruited -= _dealerRecruitedHandler;
            Dealer.OnRecruited += _dealerRecruitedHandler;
            Dealer.OnContractAccepted -= _dealerContractAcceptedHandler;
            Dealer.OnContractAccepted += _dealerContractAcceptedHandler;
            Dealer.OnRecommended -= _dealerRecommendedHandler;
            Dealer.OnRecommended += _dealerRecommendedHandler;
        }

        private void UnwireDealerEvents()
        {
            if (Dealer == null) return;
            if (_dealerRecruitedHandler != null) Dealer.OnRecruited -= _dealerRecruitedHandler;
            if (_dealerContractAcceptedHandler != null) Dealer.OnContractAccepted -= _dealerContractAcceptedHandler;
            if (_dealerRecommendedHandler != null) Dealer.OnRecommended -= _dealerRecommendedHandler;
        }

        private void HandleDealerRecruited() { }
        private void HandleContractAccepted() { }
        private void HandleDealerRecommended() { }
    }
}
