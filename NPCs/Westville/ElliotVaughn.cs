using System;
using MelonLoader;
using S1API.Economy;
using S1API.Entities;
using S1API.Map;
using UnityEngine;

namespace MoreNPCs.NPCs
{
    /// <summary>
    /// Elliot Vaughn — Westville dealer at Chemical Plant B (mod enterable). Hazmat matches Maya Webb’s loadout (no respirator); glasses, neutral pout, single goatee, dark watch.
    /// </summary>
    public sealed class ElliotVaughn : NPC
    {
        public override bool IsPhysical => true;
        public override bool IsDealer => true;

        private Action _dealerRecruitedHandler;
        private Action _dealerContractAcceptedHandler;
        private Action _dealerRecommendedHandler;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            Vector3 spawnPos = new Vector3(-102.603f, -2.935f, 90.9458f);
            var chemicalPlantB = Building.GetByName("Chemical Plant B");

            builder.WithIdentity("elliot_vaughn", "Elliot", "Vaughn")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 1.01f;
                    av.Weight = 0.44f;
                    av.SkinColor = new Color(0.68f, 0.55f, 0.45f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.98f, 0.98f, 0.98f);
                    av.PupilDilation = 0.62f;
                    av.EyebrowScale = 1.05f;
                    av.EyebrowThickness = 1.02f;
                    av.EyebrowRestingHeight = -0.14f;
                    av.EyebrowRestingAngle = 1.1f;
                    av.LeftEye = (0.30f, 0.40f);
                    av.RightEye = (0.30f, 0.40f);
                    av.HairColor = new Color(0.20f, 0.15f, 0.11f);
                    av.HairPath = "Avatar/Hair/Peaked/Peaked";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_NeutralPout", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Goatee", av.HairColor);
                    av.WithFaceLayer("Avatar/Layers/Face/TiredEyes", new Color(0f, 0f, 0f, 0.7f));
                    av.WithBodyLayer("Avatar/Layers/Top/HazmatSuit", new Color(0.9451f, 0.7882f, 0.0118f));
                    av.WithBodyLayer("Avatar/Layers/Accessories/Gloves", new Color(0.2531f, 0.5563f, 0.7578f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(0.2539f, 0.2539f, 0.2539f));
                    av.WithAccessoryLayer("Avatar/Accessories/Hands/Polex/Polex", new Color(0.22f, 0.22f, 0.26f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/RectangleFrameGlasses/RectangleFrameGlasses", new Color(0.18f, 0.18f, 0.22f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureDealer()
                .WithDealerDefaults(dd =>
                {
                    dd.WithSigningFee(1000f)
                        .WithCut(0.20f)
                        .WithDealerType(DealerType.PlayerDealer)
                        .WithHome(chemicalPlantB)
                        .AllowInsufficientQuality(false)
                        .AllowExcessQuality(true)
                        .WithCompletedDealsVariable("dealer_completed_deals");
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .WithConnectionsById("brent_halver")
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach);
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(chemicalPlantB, 0016, 1439);
                });
        }

        public ElliotVaughn() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                var b = Building.GetByName("Chemical Plant B");
                if (b != null && Dealer != null)
                    Dealer.Home = b;
                WireDealerEvents();
                Aggressiveness = 0.68f;
                Region = Region.Westville;
                Schedule.Enable();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"ElliotVaughn OnCreated failed: {ex.Message}");
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
