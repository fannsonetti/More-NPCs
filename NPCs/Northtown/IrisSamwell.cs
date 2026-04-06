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
    /// <summary>
    /// Iris Samwell — Northtown dealer, Dorothy Samwell’s daughter. Home base: parents’ apartment (Dan’s Hardware upstairs).
    /// </summary>
    public sealed class IrisSamwell : NPC
    {
        public override bool IsPhysical => true;
        public override bool IsDealer => true;

        private Action _dealerRecruitedHandler;
        private Action _dealerContractAcceptedHandler;
        private Action _dealerRecommendedHandler;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var parentsHome = Building.Get<DansHardwareUpstairs>();
            Vector3 spawnPos = GetSpawnNearParentsHome();

            builder.WithIdentity("iris_samwell", "Iris", "Samwell")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.92f;
                    av.Height = 0.97f;
                    av.Weight = 0.38f;
                    av.SkinColor = new Color(0.74f, 0.62f, 0.52f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.96f, 0.90f, 0.86f);
                    av.PupilDilation = 0.66f;
                    av.EyebrowScale = 1.02f;
                    av.EyebrowThickness = 0.88f;
                    av.EyebrowRestingHeight = -0.10f;
                    av.EyebrowRestingAngle = 2.15f;
                    av.LeftEye = (0.30f, 0.40f);
                    av.RightEye = (0.30f, 0.40f);
                    av.HairColor = new Color(0.42f, 0.32f, 0.24f);
                    av.HairPath = "Avatar/Hair/MidFringe/MidFringe";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SlightSmile", Color.black);
                    av.WithBodyLayer("Avatar/Layers/Top/Tucked T-Shirt", new Color(0.38f, 0.34f, 0.42f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.22f, 0.24f, 0.30f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/OpenVest/OpenVest", new Color(0.28f, 0.26f, 0.30f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.14f, 0.14f, 0.16f));
                    var goldAccent = new Color(0.72f, 0.58f, 0.22f);
                    av.WithAccessoryLayer("Avatar/Accessories/Neck/GoldChain/GoldChain", goldAccent);
                    av.WithAccessoryLayer("Avatar/Accessories/Hands/Polex/Polex", goldAccent);
                })
                .WithSpawnPosition(spawnPos)
                .EnsureDealer()
                .WithDealerDefaults(dd =>
                {
                    dd.WithSigningFee(500f)
                        .WithCut(0.20f)
                        .WithDealerType(DealerType.PlayerDealer)
                        .WithHome(parentsHome)
                        .AllowInsufficientQuality(false)
                        .AllowExcessQuality(true)
                        .WithCompletedDealsVariable("dealer_completed_deals");
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .WithConnectionsById("dorothy_samwell")
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach);
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(parentsHome, 0009, 1439);
                });
        }

        private static Vector3 GetSpawnNearParentsHome()
        {
            try
            {
                var t = GameObject.Find("Map/Hyland Point/Region_Northtown/Hardware Store/Outdoor chair")?.transform;
                if (t != null) return t.position;
            }
            catch { }
            return new Vector3(-58f, -2.5f, 154f);
        }

        public IrisSamwell() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Dealer.Home = Building.Get<DansHardwareUpstairs>();
                WireDealerEvents();
                Aggressiveness = 0.55f;
                Region = Region.Northtown;
                Schedule.Enable();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"IrisSamwell OnCreated failed: {ex.Message}");
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
