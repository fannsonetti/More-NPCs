using System;
using System.Linq;
using MelonLoader;
using S1API.Economy;
using S1API.Entities;
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
    public sealed class SergeantGrey : NPC
    {
        public override bool IsPhysical => true;
        public override bool IsDealer => true;

        private Action _dealerRecruitedHandler;
        private Action _dealerContractAcceptedHandler;
        private Action _dealerRecommendedHandler;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var policeStation = Building.Get<PoliceStation>();
            MelonLogger.Msg("Configuring prefab for Sergeant Grey");

            Vector3 posA = new Vector3(-27.6272f, 1.065f, 62.2025f);
            Vector3 spawnPos = new Vector3(13.7762f, 1.065f, 34.6115f);

            builder.WithIdentity("sergeant_grey", "Sergeant", "Grey")
                .WithAppearanceDefaults(av =>
                {
                    av.Gender = 0.0f; // Neutral gender
                    av.Height = 1.0f;
                    av.Weight = 0.5f;
                    var skinColor = new Color(0.282f, 0.239f, 0.203f);
                    av.SkinColor = skinColor;
                    av.LeftEyeLidColor = av.SkinColor;
                    av.RightEyeLidColor = av.SkinColor;
                    av.EyeBallTint = Color.white;
                    av.PupilDilation = 0.8f;
                    av.EyebrowScale = 0.0f;
                    av.EyebrowThickness = 0.0f;
                    av.EyebrowRestingHeight = 0.0f;
                    av.EyebrowRestingAngle = 0.0f;
                    av.LeftEye = (0.36f, 0.44f);
                    av.RightEye = (0.36f, 0.44f);
                    av.HairColor = new Color(0.075f, 0.075f, 0.075f);
                    av.WithFaceLayer("Avatar/Layers/Face/Face_NeutralPout", Color.black);
                    av.WithFaceLayer("Avatar/Layers/Face/OldPersonWrinkles", new Color32(0, 0, 0, 0));
                    av.WithFaceLayer("Avatar/Layers/Face/FacialHair_Goatee", new Color(0.075f, 0.075f, 0.075f));
                    av.WithBodyLayer("Avatar/Layers/Top/RolledButtonUp", new Color(0.178f, 0.217f, 0.406f));
                    av.WithBodyLayer("Avatar/Layers/Bottom/Jeans", new Color(0.235f, 0.235f, 0.235f));
                    av.WithAccessoryLayer("Avatar/Accessories/Feet/CombatBoots/CombatBoots", new Color(0.151f, 0.151f, 0.151f));
                    av.WithAccessoryLayer("Avatar/Accessories/Waist/PoliceBelt/PoliceBelt", Color.white);
                    av.WithAccessoryLayer("Avatar/Accessories/Chest/CollarJacket/CollarJacket", new Color(0.236f, 0.236f, 0.236f));
                })
                .WithSpawnPosition(spawnPos)
                .EnsureDealer()
                .WithDealerDefaults(dd =>
                {
                    dd.WithSigningFee(7500f) // Cost to recruit this dealer
                        .WithCut(0.15f) // Dealer keeps 15% of earnings
                        .WithDealerType(DealerType.PlayerDealer) // Works for the player
                        .WithHomeName("Police Station") // Home building name
                        .AllowInsufficientQuality(false) // Won't sell below-quality items
                        .AllowExcessQuality(true) // Can sell above-quality items
                        .WithCompletedDealsVariable("dealer_completed_deals"); // Variable to track deals
                })
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f) // Starting relationship
                        .SetUnlocked(false) // Start locked
                        .WithConnectionsById("officer_marcus")
                        .SetUnlockType(NPCRelationship.UnlockType.Recommendation); // Can be unlocked via direct approach
                })
                .WithSchedule(plan =>
                {
                    plan.EnsureDealSignal(); // Signal for handling deals
                });
        }

        public SergeantGrey() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                Appearance.Build();

                WireDealerEvents();

                Aggressiveness = 2f;
                Region = Region.Suburbia;

                Schedule.Enable();
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"SergeantGrey OnCreated failed: {ex.Message}");
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
            SendTextMessage("I'm ready to work for you!");
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
