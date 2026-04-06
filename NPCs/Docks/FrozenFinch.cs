using System;
using MelonLoader;
using S1API.Economy;
using S1API.Entities;
using S1API.Entities.Schedule;
using S1API.Map;
using UnityEngine;

namespace MoreNPCs.NPCs
{
    /// <summary>
        /// Docks-region dealer squatting the Gasmart freezer — pale icy skin, disturbed expression, bundled in cold-weather layers.
    /// </summary>
    public sealed class FrozenFinch : NPC
    {
        public override bool IsPhysical => true;
        public override bool IsDealer => true;

        private Action? _dealerRecruitedHandler;
        private Action? _dealerContractAcceptedHandler;
        private Action? _dealerRecommendedHandler;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var freezer = Building.GetByName("Gasmart Freezer");
            Vector3 spawnPos = new Vector3(10.55f, 0.3515f, -6.05f);

            builder.WithIdentity("frozen_finch", "Frozen Finch", "")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 1.01f;
                    av.Weight = 0.0f;
                    var skin = new Color(0.76f, 0.84f, 0.93f);
                    av.SkinColor = skin;
                    av.LeftEyeLidColor = skin;
                    av.RightEyeLidColor = skin;
                    av.EyeBallTint = new Color(0.90f, 0.95f, 1.00f);
                    av.PupilDilation = 0.96f;
                    av.EyebrowScale = 1.15f;
                    av.EyebrowThickness = 1.05f;
                    av.EyebrowRestingHeight = -0.35f;
                    av.EyebrowRestingAngle = -4.2f;
                    av.LeftEye = (0.72f, 0.28f);
                    av.RightEye = (0.72f, 0.28f);
                    av.HairColor = new Color(0.40f, 0.46f, 0.54f);
                    av.HairPath = "Avatar/Hair/CloseBuzzCut/CloseBuzzCut";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Agitated", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/TiredEyes", new Color(0.10f, 0.09f, 0.09f, 0.50f));
                    av.WithFaceLayer("Avatar/Layers/Face/OldPersonWrinkles", new Color(0.22f, 0.22f, 0.22f, 0.42f));
                    av.WithBodyLayer("Avatar/Layers/Top/FlannelButtonUp", new Color(0.70f, 0.76f, 0.84f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.17f, 0.19f, 0.23f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/OpenVest/OpenVest", new Color(0.20f, 0.24f, 0.29f));
                    av.WithBodyLayer("Avatar/Layers/Accessories/FingerlessGloves", new Color(0.12f, 0.13f, 0.15f));
                    av.WithAccessoryLayer("Avatar/Accessories/Neck/GoldChain/GoldChain", new Color(0.72f, 0.88f, 0.96f));
                    av.WithAccessoryLayer("Avatar/Accessories/Hands/Polex/Polex", new Color(0.78f, 0.90f, 0.98f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(0.12f, 0.13f, 0.15f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureDealer()
                .WithDealerDefaults(dd =>
                {
                    dd.WithSigningFee(2800f)
                        .WithCut(0.20f)
                        .WithDealerType(DealerType.PlayerDealer)
                        .WithHome(freezer)
                        .AllowInsufficientQuality(false)
                        .AllowExcessQuality(true)
                        .WithCompletedDealsVariable("dealer_completed_deals");
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .WithConnectionsById("maya_webb", "blake_drift", "dewey_koontz")
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach);
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(freezer, 0005, 1434);
                });
        }

        public FrozenFinch() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Dealer.Home = Building.GetByName("Gasmart Freezer");
                WireDealerEvents();
                Aggressiveness = 0.88f;
                Region = Region.Docks;
                Schedule.Enable();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"FrozenFinch OnCreated failed: {ex.Message}");
                MelonLogger.Error($"StackTrace: {ex.StackTrace}");
            }
        }

        protected override void OnDestroyed()
        {
            base.OnDestroyed();
            UnwireDealerEvents();
        }

        private void WireDealerEvents()
        {
            if (Dealer == null)
            {
                MelonLogger.Warning($"Dealer component missing for {ID}; cannot wire dealer events.");
                return;
            }

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
            if (Dealer == null)
                return;
            if (_dealerRecruitedHandler != null)
                Dealer.OnRecruited -= _dealerRecruitedHandler;
            if (_dealerContractAcceptedHandler != null)
                Dealer.OnContractAccepted -= _dealerContractAcceptedHandler;
            if (_dealerRecommendedHandler != null)
                Dealer.OnRecommended -= _dealerRecommendedHandler;
        }

        private void HandleDealerRecruited() { }
        private void HandleContractAccepted() { }
        private void HandleDealerRecommended() { }
    }
}
