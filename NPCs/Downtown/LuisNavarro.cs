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
    /// <summary>Downtown dealer — Grocery Backdoor; convenience-store look (apron, shades, rolled shirt).</summary>
    public sealed class LuisNavarro : NPC
    {
        public override bool IsPhysical => true;
        public override bool IsDealer => true;

        private Action _dealerRecruitedHandler;
        private Action _dealerContractAcceptedHandler;
        private Action _dealerRecommendedHandler;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var groceryBackdoor = Building.GetByName("Grocery Backdoor");
            Vector3 spawnPos = new Vector3(15.2f, 1.22f, 68.1f);
            builder.WithIdentity("luis_navarro", "Luis", "Navarro")
                .WithAppearanceDefaults(av =>
                {
                    var stash = new Color(0.18f, 0.11f, 0.07f);
                    var apronGreen = new Color(0.22f, 0.40f, 0.24f);
                    av.Gender = 0.0f;
                    av.Height = 1.01f;
                    av.Weight = 0.46f;
                    av.SkinColor = new Color(0.48f, 0.38f, 0.30f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.98f, 0.94f, 0.92f);
                    av.PupilDilation = 0.52f;
                    av.EyebrowScale = 1.08f;
                    av.EyebrowThickness = 1.22f;
                    av.EyebrowRestingHeight = -0.10f;
                    av.EyebrowRestingAngle = 0.95f;
                    av.LeftEye = (0.30f, 0.40f);
                    av.RightEye = (0.30f, 0.40f);
                    av.HairPath = "Avatar/Hair/Balding/Balding";
                    av.HairColor = stash;
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SmugPout", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Swirl", stash);
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Stubble", stash);
                    av.WithBodyLayer("Avatar/Layers/Top/RolledButtonUp", new Color(0.32f, 0.33f, 0.36f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.20f, 0.22f, 0.28f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/Apron/Apron", apronGreen);
                    av.WithAccessoryLayer("Avatar/Accessories/Head/Oakleys/Oakleys", new Color(0.12f, 0.12f, 0.14f));
                    av.WithBodyLayer("Avatar/Layers/Accessories/FingerlessGloves", new Color(0.14f, 0.14f, 0.16f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.14f, 0.14f, 0.16f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureDealer()
                .WithDealerDefaults(dd =>
                {
                    dd.WithSigningFee(2000f)
                        .WithCut(0.20f)
                        .WithDealerType(DealerType.PlayerDealer)
                        .WithHome(groceryBackdoor)
                        .AllowInsufficientQuality(false)
                        .AllowExcessQuality(true)
                        .WithCompletedDealsVariable("dealer_completed_deals");
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .WithConnectionsById("ethan_vance", "nina_cho")
                        .SetUnlockType(NPCRelationship.UnlockType.Recommendation);
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(groceryBackdoor, 0016, 1439);
                });
        }

        public LuisNavarro() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Dealer.Home = Building.GetByName("Grocery Backdoor");
                WireDealerEvents();
                Aggressiveness = 0.68f;
                Region = Region.Downtown;
                Schedule.Enable();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"LuisNavarro OnCreated failed: {ex.Message}");
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
