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
    public sealed class SergeantGrey : NPC
    {
        public override bool IsPhysical => true;
        public override bool IsDealer => true;

        private Action _dealerRecruitedHandler;
        private Action _dealerContractAcceptedHandler;
        private Action _dealerRecommendedHandler;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var policeStation = Building.Get<PoliceStation>();
            MelonLogger.Msg("Configuring prefab for Sergeant Grey");

            Vector3 posA = new Vector3(-27.6272f, 1.065f, 62.2025f);
            Vector3 spawnPos = new Vector3(13.7762f, 1.065f, 34.6115f);

            builder.WithIdentity("sergeant_grey", "Sergeant", "Grey")
                .WithAppearanceDefaults(av =>
                {
                    var skinColor = new Color(0.282f, 0.239f, 0.203f);
                    av.Gender = 0.0f;
                    av.Height = 1.0f;
                    av.Weight = 0.5f;
                    av.SkinColor = skinColor;
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = Color.white;
                    av.PupilDilation = 0.8f;
                    av.EyebrowScale = 0.0f;
                    av.EyebrowThickness = 0.0f;
                    av.EyebrowRestingHeight = 0.0f;
                    av.EyebrowRestingAngle = 0.0f;
                    av.LeftEye = (0.36f, 0.44f);
                    av.RightEye = (0.36f, 0.44f);
                    av.HairColor = new Color(0.075f, 0.075f, 0.075f);
                    av.WithFaceLayer("Avatar/Layers/Face/Face_NeutralPout", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/OldPersonWrinkles", new Color32(0, 0, 0, 0));
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Goatee", new Color(0.075f, 0.075f, 0.075f));
                    av.WithBodyLayer("Avatar/Layers/Top/RolledButtonUp", new Color(0.178f, 0.217f, 0.406f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.235f, 0.235f, 0.235f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(0.151f, 0.151f, 0.151f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/PoliceBelt/PoliceBelt", Color.white);
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/CollarJacket/CollarJacket", new Color(0.236f, 0.236f, 0.236f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureDealer()
                .WithDealerDefaults(dd =>
                {
                    dd.WithSigningFee(4000f)
                        .WithCut(0.20f)
                        .WithDealerType(DealerType.PlayerDealer)
                        .WithHome(policeStation)
                        .AllowInsufficientQuality(false)
                        .AllowExcessQuality(true)
                        .WithCompletedDealsVariable("dealer_completed_deals");
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .WithConnectionsById("officer_marcus")
                        .SetUnlockType(NPCRelationship.UnlockType.Recommendation);
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(policeStation, 2352, 1439);
                });
        }

        public SergeantGrey() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                WireDealerEvents();

                Aggressiveness = 0.81f;
                Region = Region.Suburbia;

                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"SergeantGrey OnCreated failed: {ex.Message}");
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
            {
                Dealer.OnRecruited -= _dealerRecruitedHandler;
            }

            if (_dealerContractAcceptedHandler != null)
            {
                Dealer.OnContractAccepted -= _dealerContractAcceptedHandler;
            }

            if (_dealerRecommendedHandler != null)
            {
                Dealer.OnRecommended -= _dealerRecommendedHandler;
            }
        }

        private void HandleDealerRecruited() { /* Recruitment done in person, no message needed */ }

        private void HandleContractAccepted() { }

        private void HandleDealerRecommended() { }
    }
}
