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
    /// <summary>Docks dealer — Red Docks Shipping Container 2. Edgier look (open vest / tattoos).</summary>
    public sealed class SloaneReyes : NPC
    {
        public override bool IsPhysical => true;
        public override bool IsDealer => true;

        private Action _dealerRecruitedHandler;
        private Action _dealerContractAcceptedHandler;
        private Action _dealerRecommendedHandler;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var dockHome = Building.GetByName("Red Docks Shipping Container 2");
            Vector3 spawnPos = new Vector3(-57.2f, -2.25f, -77.5f);

            builder.WithIdentity("sloane_reyes", "Sloane", "Reyes")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 1.2f;
                    av.Height = 1.02f;
                    av.Weight = 0.87f;
                    var skin = new Color(0.58f, 0.46f, 0.40f);
                    av.SkinColor = skin;
                    av.LeftEyeLidColor = skin;
                    av.RightEyeLidColor = skin;
                    av.EyeBallTint = new Color(0.96f, 0.88f, 0.86f);
                    av.PupilDilation = 0.68f;
                    av.EyebrowScale = 0.98f;
                    av.EyebrowThickness = 0.96f;
                    av.EyebrowRestingHeight = -0.08f;
                    av.EyebrowRestingAngle = 2.05f;
                    av.LeftEye = (0.34f, 0.44f);
                    av.RightEye = (0.34f, 0.44f);
                    av.HairColor = new Color(0.14f, 0.10f, 0.09f);
                    av.HairPath = "Avatar/Hair/LongCurly/LongCurly";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SmugPout", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/Nipples", skin);
                    av.WithBodyLayer("Avatar/Layers/Top/UpperBodyTattoos", new Color(0.14f, 0.12f, 0.11f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/FemaleUnderwear", new Color(0.62f, 0.58f, 0.56f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.22f, 0.21f, 0.25f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/OpenVest/OpenVest", new Color(0.26f, 0.22f, 0.20f));
                    av.WithBodyLayer("Avatar/Layers/Accessories/FingerlessGloves", new Color(0.12f, 0.12f, 0.14f));
                    av.WithAccessoryLayer("Avatar/Accessories/Neck/GoldChain/GoldChain", new Color(0.88f, 0.74f, 0.28f));
                    av.WithAccessoryLayer("Avatar/Accessories/Hands/Polex/Polex", new Color(0.72f, 0.74f, 0.78f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(0.11f, 0.11f, 0.12f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureDealer()
                .WithDealerDefaults(dd =>
                {
                    dd.WithSigningFee(3000f)
                        .WithCut(0.20f)
                        .WithDealerType(DealerType.PlayerDealer)
                        .WithHome(dockHome)
                        .AllowInsufficientQuality(false)
                        .AllowExcessQuality(true)
                        .WithCompletedDealsVariable("dealer_completed_deals");
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .WithConnectionsById("manhole_mike", "grunk", "mack")
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach);
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(dockHome, 0016, 1439);
                });
        }

        public SloaneReyes() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Dealer.Home = Building.GetByName("Red Docks Shipping Container 2");
                WireDealerEvents();
                Aggressiveness = 0.71f;
                Region = Region.Docks;
                Schedule.Enable();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"SloaneReyes OnCreated failed: {ex.Message}");
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
