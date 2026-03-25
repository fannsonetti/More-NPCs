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
    public sealed class MaxineJunefield : NPC
    {
        public override bool IsPhysical => true;
        public override bool IsDealer => true;

        private Action _dealerRecruitedHandler;
        private Action _dealerContractAcceptedHandler;
        private Action _dealerRecommendedHandler;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var caravan = Building.Get<Caravan>();
            Vector3 spawnPos = new Vector3(-94.4505f, -3.0379f, 56.6294f);

            builder.WithIdentity("maxine_junefield", "Maxine", "Junefield")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 1.0f;
                    av.Height = 0.96f;
                    av.Weight = 0.38f;
                    av.SkinColor = new Color(0.72f, 0.58f, 0.48f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(1f, 0.92f, 0.88f);
                    av.PupilDilation = 0.72f;
                    av.EyebrowScale = 1.0f;
                    av.EyebrowThickness = 0.9f;
                    av.EyebrowRestingHeight = -0.18f;
                    av.EyebrowRestingAngle = 2.0f;
                    av.LeftEye = (0.36f, 0.4f);
                    av.RightEye = (0.36f, 0.4f);
                    av.HairColor = new Color(0.45f, 0.28f, 0.18f);
                    av.HairPath = "Avatar/Hair/FringePonytail/FringePonytail";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Neutral", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/Freckles", new Color(0.55f, 0.42f, 0.35f));
                    av.WithBodyLayer("Avatar/Layers/Top/FlannelButtonUp", new Color(0.95f, 0.95f, 0.92f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.35f, 0.38f, 0.48f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.92f, 0.9f, 0.88f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/Belt/Belt", new Color(0.35f, 0.28f, 0.22f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/CollarJacket/CollarJacket", new Color(0.55f, 0.68f, 0.82f));
                    av.WithAccessoryLayer("Avatar/Accessories/Neck/GoldChain/GoldChain", new Color(0.88f, 0.25f, 0.1f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureDealer()
                .WithDealerDefaults(dd =>
                {
                    dd.WithSigningFee(2500f)
                        .WithCut(0.20f)
                        .WithDealerType(DealerType.PlayerDealer)
                        .WithHome(caravan)
                        .AllowInsufficientQuality(false)
                        .AllowExcessQuality(true)
                        .WithCompletedDealsVariable("dealer_completed_deals");
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .WithConnectionsById("joyce_ball", "doris_lubbin", "lila_park")
                        .SetUnlockType(NPCRelationship.UnlockType.Recommendation);
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(caravan, 000, 1439);
                });
        }

        public MaxineJunefield() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Dealer.Home = Building.Get<Caravan>();

                WireDealerEvents();

                Aggressiveness = 0.82f;
                Region = Region.Westville;

                Schedule.Enable();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"MaxineJunefield OnCreated failed: {ex.Message}");
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

        private void HandleDealerRecruited() { /* Recruitment done in person, no message needed */ }

        private void HandleContractAccepted() { }

        private void HandleDealerRecommended() { }
    }
}
