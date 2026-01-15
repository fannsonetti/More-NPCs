using System;
using System.Linq;
using MelonLoader;
using S1API.Avatar;
using S1API.Economy;
using S1API.Entities;
using S1API.Entities.Appearances.CustomizationFields;
using S1API.Entities.NPCs.Northtown;
using S1API.Entities.Schedule;
using S1API.GameTime;
using S1API.Growing;
using S1API.Map;
using S1API.Map.Buildings;
using S1API.Map.ParkingLots;
using S1API.Money;
using S1API.Products;
using S1API.Properties;
using S1API.Vehicles;
using UnityEngine;

namespace CustomNPCTest.NPCs
{
    /// <summary>
    /// An example S1API NPC that opts into dealer functionality.
    /// Demonstrates dealer configuration, customer assignment, and cash management.
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
            MelonLogger.Msg("Configuring prefab for FannsoNetti");

            Vector3 posA = new Vector3(-27.6272f, 1.065f, 62.2025f);
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
                    av.EyebrowThickness = 1.0f;
                    av.EyebrowRestingHeight = 0.0f;
                    av.EyebrowRestingAngle = 0.0f;
                    av.LeftEye = (0.35f, 0.44f);
                    av.RightEye = (0.35f, 0.44f);
                    av.HairColor = new Color(0.239f, 0.182f, 0.139f);
                    av.HairPath = "Avatar/Hair/Spiky/Spiky";
                    av.WithFaceLayer("Avatar/Layers/Face/Face_SmugPout", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Goatee", new Color(0.198f, 0.118f, 0.062f));
                    av.WithBodyLayer("Avatar/Layers/Top/ButtonUp", new Color(0.178f, 0.217f, 0.406f));
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
                    dd.WithSigningFee(0) // Cost to recruit this dealer
                        .WithCut(0.4f) // Dealer keeps 15% of earnings
                        .WithDealerType(DealerType.PlayerDealer) // Works for the player
                        .WithHome(northwarehouse) // Home building name
                        .AllowInsufficientQuality(false) // Won't sell below-quality items
                        .AllowExcessQuality(true) // Can sell above-quality items
                        .WithCompletedDealsVariable("dealer_completed_deals"); // Variable to track deals
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f) // Starting relationship
                        .SetUnlocked(true) // Start locked
                        .WithConnectionsById("")
                        .SetUnlockType(NPCRelationship.UnlockType.Recommendation); // Can be unlocked via direct approach
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal(); // Signal for handling deals
                });
        }

        public FannsoNetti() : base()
        {
        }

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
            catch (System.Exception ex)
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

        private void HandleDealerRecruited()
        {
            MelonLogger.Msg($"Dealer {ID} has been recruited!");
            SendTextMessage("Thank you for using MoreNPCs. As your reward, I’ll assist you with your dealings in exchange for a 40% share.");
        }

        private void HandleContractAccepted()
        {
            MelonLogger.Msg($"Dealer {ID} accepted a new contract!");
        }

        private void HandleDealerRecommended()
        {
            MelonLogger.Msg($"Dealer {ID} has been recommended!");
        }
    }
}
