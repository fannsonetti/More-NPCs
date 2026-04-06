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
    public sealed class RickTorres : NPC
    {
        public override bool IsPhysical => true;
        public override bool IsDealer => true;

        private Action _dealerRecruitedHandler;
        private Action _dealerContractAcceptedHandler;
        private Action _dealerRecommendedHandler;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var home = Building.GetByName("Long House Side Door");
            Vector3 spawnPos = new Vector3(65.18f, 4.935f, -87.45f);

            builder.WithIdentity("rick_torres", "Rick", "Torres")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 1.04f;
                    av.Weight = 0.48f;
                    av.SkinColor = new Color(0.56f, 0.44f, 0.36f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(1f, 0.92f, 0.88f);
                    av.PupilDilation = 0.52f;
                    av.EyebrowScale = 1.06f;
                    av.EyebrowThickness = 1.12f;
                    av.EyebrowRestingHeight = -0.04f;
                    av.EyebrowRestingAngle = 0.28f;
                    av.LeftEye = (0.32f, 0.42f);
                    av.RightEye = (0.32f, 0.42f);
                    av.HairColor = new Color(0.10f, 0.08f, 0.07f);
                    av.HairPath = "Avatar/Hair/BuzzCut/BuzzCut";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SmugPout", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Stubble", new Color(0.10f, 0.07f, 0.06f));
                    // Suburbia dealer: shirt + vest + cargos + boots + chain (no glasses / no office stack)
                    av.WithBodyLayer("Avatar/Layers/Top/RolledButtonUp", new Color(0.26f, 0.28f, 0.32f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.20f, 0.21f, 0.24f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/OpenVest/OpenVest", new Color(0.18f, 0.17f, 0.20f));
                    av.WithAccessoryLayer("Avatar/Accessories/Neck/GoldChain/GoldChain", new Color(0.72f, 0.74f, 0.78f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(0.11f, 0.11f, 0.12f));
                    av.WithBodyLayer("Avatar/Layers/Accessories/FingerlessGloves", new Color(0.13f, 0.13f, 0.15f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureDealer()
                .WithDealerDefaults(dd =>
                {
                    dd.WithSigningFee(3000f)
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
                        .WithConnectionsById("")
                        .SetUnlockType(NPCRelationship.UnlockType.Recommendation);
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(home, 0016, 1439);
                });
        }

        public RickTorres() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Dealer.Home = Building.GetByName("Long House Side Door");
                WireDealerEvents();
                Aggressiveness = 0.72f;
                Region = Region.Suburbia;
                Schedule.Enable();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"RickTorres OnCreated failed: {ex.Message}");
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
