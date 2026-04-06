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
    /// Uptown dealer — ex-cartel; shares Manor Tunnel Hatch with Hayes Denberg.
    /// </summary>
    public sealed class EstebanCordova : NPC
    {
        public override bool IsPhysical => true;
        public override bool IsDealer => true;

        private Action? _dealerRecruitedHandler;
        private Action? _dealerContractAcceptedHandler;
        private Action? _dealerRecommendedHandler;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var tunnelHome = Building.GetByName("Manor Tunnel Hatch");
            Vector3 spawnPos = new Vector3(166.52f, 5.93f, -54.72f);

            builder.WithIdentity("esteban_cordova", "Esteban", "Cordova")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 0.9916f;
                    av.Weight = 0.2704f;
                    av.SkinColor = new Color(0.8082f, 0.6341f, 0.4929f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = Color.white;
                    av.PupilDilation = 0.50f;
                    av.EyebrowScale = 1.1563f;
                    av.EyebrowThickness = 1.8281f;
                    av.EyebrowRestingHeight = -0.475f;
                    av.EyebrowRestingAngle = 3.375f;
                    av.LeftEye = (0.50f, 0.38f);
                    av.RightEye = (0.50f, 0.38f);
                    av.HairColor = new Color(0.2453f, 0.1921f, 0.14f);
                    av.HairPath = "Avatar/Hair/Peaked/Peaked";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_NeutralPout", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Goatee", new Color(0f, 0f, 0f, 0.8193f));
                    av.WithBodyLayer("Avatar/Layers/Top/FlannelButtonUp", new Color(0.1788f, 0.3991f, 0.0669f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jorts", new Color(0.266f, 0.1975f, 0.0861f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/Belt/Belt", Color.black);
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/BulletproofVest/BulletproofVest", new Color(0.151f, 0.151f, 0.151f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/LegendSunglasses/LegendSunglasses", new Color(1f, 0.82f, 0.38f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sandals/Sandals", new Color(0.42f, 0.28f, 0.18f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureDealer()
                .WithDealerDefaults(dd =>
                {
                    dd.WithSigningFee(2800f)
                        .WithCut(0.20f)
                        .WithDealerType(DealerType.PlayerDealer)
                        .WithHome(tunnelHome)
                        .AllowInsufficientQuality(false)
                        .AllowExcessQuality(true)
                        .WithCompletedDealsVariable("dealer_completed_deals");
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .WithConnectionsById("hayes_denberg", "darius_cole")
                        .SetUnlockType(NPCRelationship.UnlockType.Recommendation);
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(tunnelHome, 0020, 1429);
                });
        }

        public EstebanCordova() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Dealer.Home = Building.GetByName("Manor Tunnel Hatch");
                WireDealerEvents();
                Aggressiveness = 0.62f;
                Region = Region.Uptown;
                Schedule.Enable();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"EstebanCordova OnCreated failed: {ex.Message}");
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
