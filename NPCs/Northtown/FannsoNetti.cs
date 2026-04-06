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
    /// FannsoNetti - Northtown dealer at North Warehouse. Connected to Valerie and Jian.
    /// </summary>
    public sealed class FannsoNetti : NPC
    {
        public override bool IsPhysical => true;
        public override bool IsDealer => true;

        private Action _dealerRecruitedHandler;
        private Action _dealerContractAcceptedHandler;
        private Action _dealerRecommendedHandler;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var northwarehouse = Building.Get<NorthWarehouse>();
            Vector3 spawnPos = new Vector3(-41.7551f, -2.9354f, 171.8678f);

            builder.WithIdentity("fannsonetti", "FannsoNetti", "")
                .WithAppearanceDefaults(av =>
                {
                    // Matches Vincent Reeves’ prior look (warehouse regular — not default-unlocked).
                    av.Gender = 0.0f;
                    av.Height = 0.98f;
                    av.Weight = 0.40f;
                    av.SkinColor = new Color(0.63f, 0.52f, 0.43f);
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = new Color(1.0f, 0.95f, 0.9f);
                    av.PupilDilation = 0.40f;
                    av.EyebrowScale = 1.15f;
                    av.EyebrowThickness = 1.23f;
                    av.EyebrowRestingHeight = -0.44f;
                    av.EyebrowRestingAngle = -5.2f;
                    av.LeftEye = (0.15f, 0.24f);
                    av.RightEye = (0.15f, 0.24f);
                    av.HairColor = new Color(0.18f, 0.14f, 0.10f);
                    av.HairPath = "Avatar/Hair/Spiky/Spiky";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_Neutral", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Stubble", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/TiredEyes", new Color(0.06f, 0.05f, 0.05f, 0.82f));
                    av.WithBodyLayer("Avatar/Layers/Top/Buttonup", new Color(0.2f, 0.2f, 0.24f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.16f, 0.16f, 0.18f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/Sneakers/Sneakers", new Color(0.15f, 0.15f, 0.15f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/OpenVest/OpenVest", new Color(0.15f, 0.15f, 0.15f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/Belt/Belt", new Color(0.28f, 0.22f, 0.16f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureDealer()
                .WithDealerDefaults(dd =>
                {
                    dd.WithSigningFee(500f)
                        .WithCut(0.20f)
                        .WithDealerType(DealerType.PlayerDealer)
                        .WithHome(northwarehouse)
                        .AllowInsufficientQuality(false)
                        .AllowExcessQuality(true)
                        .WithCompletedDealsVariable("dealer_completed_deals");
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .WithConnectionsById("vincent_reeves", "nico_marlowe")
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach);
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal();
                    plan.StayInBuilding(northwarehouse, 0009, 1439);
                });
        }

        public FannsoNetti() : base() { }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();
                Dealer.Home = Building.Get<NorthWarehouse>();
                WireDealerEvents();
                Aggressiveness = 2f;
                Region = Region.Northtown;
                Schedule.Enable();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"FannsoNetti OnCreated failed: {ex.Message}");
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

        private void HandleDealerRecruited()
        {
            MelonLogger.Msg($"Dealer {ID} has been recruited!");
            SendTextMessage("Thank you for using MoreNPCs. As your reward, I�ll assist you with your dealings in exchange for a 20% share of the profits.");
        }
        private void HandleContractAccepted() { }
        private void HandleDealerRecommended() { }
    }
}
