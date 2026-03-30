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
                    var skinColor = new Color(0.684f, 0.554f, 0.445f);
                    av.Gender = 0.0f;
                    av.Height = 1.0f;
                    av.Weight = 0.30f;
                    av.SkinColor = skinColor;
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = Color.white;
                    av.PupilDilation = 0.475f;
                    av.EyebrowScale = 1.02f;
                    av.EyebrowThickness = 1.1f;
                    av.EyebrowRestingHeight = 0.0f;
                    av.EyebrowRestingAngle = 0.0f;
                    av.LeftEye = (0.35f, 0.44f);
                    av.RightEye = (0.35f, 0.44f);
                    av.HairColor = new Color(0.239f, 0.182f, 0.139f);
                    av.HairPath = "Avatar/Hair/Spiky/Spiky";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SmugPout", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Goatee", new Color(0.198f, 0.118f, 0.062f));
                    av.WithBodyLayer("Avatar/Layers/Top/Buttonup", new Color(0.178f, 0.217f, 0.406f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/CargoPants", new Color(0.178f, 0.217f, 0.406f));
                    av.WithBodyLayer("Avatar/Layers/Accessories/Gloves", new Color(0.151f, 0.151f, 0.151f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(1.0f, 1.0f, 1.0f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/Belt/Belt", new Color(0.151f, 0.151f, 0.151f));
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/OpenVest/OpenVest", new Color(1.0f, 1.0f, 1.0f));
                    av.WithAccessoryLayer("Avatar/Accessories/Neck/GoldChain/GoldChain", new Color(1.0f, 0.756f, 0.212f));
                    av.WithAccessoryLayer("Avatar/Accessories/Hands/Polex/Polex", new Color(1.0f, 0.756f, 0.212f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureDealer()
                .WithDealerDefaults(dd =>
                {
                    dd.WithSigningFee(0)
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
                        .SetUnlocked(true)
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
