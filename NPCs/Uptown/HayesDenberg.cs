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
    /// Uptown dealer — name play on Heisenberg (Hayes + Denberg).
    /// </summary>
    public sealed class HayesDenberg : NPC
    {
        public override bool IsPhysical => true;
        public override bool IsDealer => true;

        private Action? _dealerRecruitedHandler;
        private Action? _dealerContractAcceptedHandler;
        private Action? _dealerRecommendedHandler;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var tunnelHome = Building.GetByName("Manor Tunnel Hatch");
            Vector3 spawnPos = new Vector3(166.45f, 5.93f, -54.85f);

            builder.WithIdentity("hayes_denberg", "Hayes", "Denberg")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f;
                    av.Height = 1.01f;
                    av.Weight = 0.52f;
                    av.SkinColor = new Color(0.76f, 0.65f, 0.54f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(0.98f, 0.96f, 0.94f);
                    av.PupilDilation = 0.44f;
                    av.EyebrowScale = 1.02f;
                    av.EyebrowThickness = 0.95f;
                    av.EyebrowRestingHeight = -0.32f;
                    av.EyebrowRestingAngle = 3.1f;
                    av.LeftEye = (0.30f, 0.36f);
                    av.RightEye = (0.30f, 0.36f);
                    av.HairColor = new Color(0.30f, 0.28f, 0.26f);
                    av.HairPath = "Avatar/Hair/Receding/Receding";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SmugPout", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/OldPersonWrinkles", new Color(0.14f, 0.12f, 0.11f, 0.94f));
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Goatee", new Color(0.32f, 0.28f, 0.24f));
                    av.WithBodyLayer("Avatar/Layers/Top/Buttonup", new Color(0.24f, 0.27f, 0.20f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.09f, 0.08f, 0.08f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/DressShoes/DressShoes", new Color(0.12f, 0.12f, 0.13f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/Blazer/Blazer", new Color(0.09f, 0.08f, 0.08f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/PorkpieHat/PorkpieHat", new Color(0.05f, 0.05f, 0.05f));
                    av.WithAccessoryLayer("Avatar/Accessories/Head/RectangleFrameGlasses/RectangleFrameGlasses", new Color(0.15f, 0.15f, 0.16f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureDealer()
                .WithDealerDefaults(dd =>
                {
                    dd.WithSigningFee(2500f)
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
                        .WithConnectionsById("edward_boog", "daniel_j_dalby", "esteban_cordova")
                        .SetUnlockType(NPCRelationship.UnlockType.Recommendation);
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(tunnelHome, 0010, 1439);
                });
        }

        public HayesDenberg() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Dealer.Home = Building.GetByName("Manor Tunnel Hatch");
                WireDealerEvents();
                Aggressiveness = 0.55f;
                Region = Region.Uptown;
                Schedule.Enable();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"HayesDenberg OnCreated failed: {ex.Message}");
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
