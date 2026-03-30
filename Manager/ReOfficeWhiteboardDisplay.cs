using System;
using System.Collections.Generic;
using System.Reflection;
using MelonLoader;
using MoreNPCs.NPCs;
using MoreNPCs.Utils;
using S1API.Entities;
using S1API.Entities.NPCs.Docks;
using S1API.Entities.NPCs.Downtown;
using S1API.Entities.NPCs.Northtown;
using S1API.Entities.NPCs.Suburbia;
using S1API.Entities.NPCs.Uptown;
using S1API.Entities.NPCs.Westville;
using S1API.Property;
using UnityEngine;

namespace MoreNPCs.Manager
{
    /// <summary>
    /// When an RE tier property is owned, the vanilla whiteboard card is hidden. We re-enable the matching
    /// <c>PropertyListing …</c> and show one unlocked artificial business on Title/Price (3D TMP), optional custom
    /// sprite on child <c>Image</c> (<see cref="SpriteRenderer"/>), then notify via Ray.
    /// </summary>
    public static class ReOfficeWhiteboardDisplay
    {
        private static readonly Dictionary<ArtificialBusinessMapping.LaunderTier, string> LastRayTextBusinessByTier =
            new Dictionary<ArtificialBusinessMapping.LaunderTier, string>();

        private static bool _loggedMissingWhiteboard;
        private static readonly Dictionary<string, Transform?> ListingTransformCache =
            new Dictionary<string, Transform?>(StringComparer.Ordinal);

        /// <summary>Which business name is on the RE whiteboard Title for each tier (one slot per tier — Ray purchase must match).</summary>
        private static readonly Dictionary<ArtificialBusinessMapping.LaunderTier, string> ActiveListingByTier =
            new Dictionary<ArtificialBusinessMapping.LaunderTier, string>();

        /// <summary>Same business shown on the downtown RE whiteboard for that tier; use for purchase gating.</summary>
        public static bool TryGetActiveListedBusinessForPurchase(ArtificialBusinessMapping.LaunderTier tier, out string businessName)
        {
            businessName = "";
            if (ActiveListingByTier.TryGetValue(tier, out var cached) && !string.IsNullOrEmpty(cached))
            {
                businessName = cached;
                return true;
            }
            // Without this, TryPickDisplayBusiness can return a tier’s first unpurchased business even when the player
            // does not own that RE slot.
            if (!ReOfficePropertyBusinessUnlock.PlayerOwnsReTierSlot(tier)) return false;
            return TryPickDisplayBusiness(tier, out businessName);
        }

        public static void SyncFromGameState()
        {
            try
            {
                ActiveListingByTier.Clear();
                var owned = BusinessManager.GetOwnedBusinesses();
                if (owned == null || owned.Count == 0) return;

                foreach (var v in Enum.GetValues(typeof(ArtificialBusinessMapping.LaunderTier)))
                {
                    var tier = (ArtificialBusinessMapping.LaunderTier)v;
                    if (tier == ArtificialBusinessMapping.LaunderTier.Tier5) continue;
                    if (!ReOfficePropertyBusinessUnlock.PlayerOwnsReTierSlot(tier)) continue;
                    if (!TryPickDisplayBusiness(tier, out var businessName))
                    {
                        TryHideTierListing(tier);
                        continue;
                    }

                    ActiveListingByTier[tier] = businessName;
                    if (!ArtificialBusinessCatalog.TryGetListingPrice(businessName, out var price))
                        price = 0f;
                    TryNotifyRayListingChanged(tier, businessName, price);
                    TryApplyTierCard(tier, businessName, price);
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"ReOfficeWhiteboardDisplay: {ex.Message}");
            }
        }

        /// <summary>Current downtown board listing for a tier = first business in order not yet bought through Ray.</summary>
        private static bool TryPickDisplayBusiness(ArtificialBusinessMapping.LaunderTier tier, out string businessName)
        {
            businessName = "";
            foreach (var name in ArtificialBusinessMapping.GetArtificialBusinessNamesForTier(tier))
            {
                if (ManagerBusinessSave.IsVanillaPurchasedStatic(name)) continue;
                businessName = name;
                return true;
            }
            return false;
        }

        private static string PropertyListingObjectName(ArtificialBusinessMapping.LaunderTier tier) =>
            tier switch
            {
                ArtificialBusinessMapping.LaunderTier.Laundromat => "PropertyListing Laundromat",
                ArtificialBusinessMapping.LaunderTier.PostOffice => "PropertyListing Post Office",
                ArtificialBusinessMapping.LaunderTier.CarWash => "PropertyListing Car Wash",
                ArtificialBusinessMapping.LaunderTier.TacoTicklers => "PropertyListing Taco Ticklers",
                _ => ""
            };

        /// <summary>When every catalog business for a tier is bought, hide the RE whiteboard card for that tier.</summary>
        private static void TryHideTierListing(ArtificialBusinessMapping.LaunderTier tier)
        {
            var listingName = PropertyListingObjectName(tier);
            if (string.IsNullOrEmpty(listingName)) return;

            var listingRoot = FindPropertyListingTransform(listingName);
            if (listingRoot == null) return;

            listingRoot.gameObject.SetActive(false);
        }

        private static void TryApplyTierCard(ArtificialBusinessMapping.LaunderTier tier, string businessName, float price)
        {
            var listingName = PropertyListingObjectName(tier);
            if (string.IsNullOrEmpty(listingName)) return;

            var listingRoot = FindPropertyListingTransform(listingName);
            if (listingRoot == null)
            {
                if (!_loggedMissingWhiteboard)
                {
                    _loggedMissingWhiteboard = true;
                    MelonLogger.Msg($"ReOfficeWhiteboardDisplay: could not find '{listingName}' (RE interior not loaded or path changed).");
                }
                return;
            }

            listingRoot.gameObject.SetActive(true);

            var titleT = FindChildRecursive(listingRoot, "Title");
            var priceT = FindChildRecursive(listingRoot, "Price");
            SetTmpText(titleT, businessName);
            SetTmpText(priceT, $"${price:N0}");

            var imageT = FindChildRecursive(listingRoot, "Image");
            if (imageT != null)
            {
                var sr = imageT.GetComponent<SpriteRenderer>();
                WhiteboardListingSpriteLoader.TryApplyListingImage(sr, tier, businessName);
            }
        }

        private static void TryNotifyRayListingChanged(ArtificialBusinessMapping.LaunderTier tier, string businessName, float price)
        {
            if (LastRayTextBusinessByTier.TryGetValue(tier, out var last) &&
                string.Equals(last, businessName, StringComparison.OrdinalIgnoreCase))
                return;

            if (!TryRaySendListingText(businessName, price)) return;
            LastRayTextBusinessByTier[tier] = businessName;
        }

        private static Transform? FindPropertyListingTransform(string listingName)
        {
            if (ListingTransformCache.TryGetValue(listingName, out var cached) && cached != null)
                return cached;

            Transform? best = null;
            foreach (var o in Resources.FindObjectsOfTypeAll(typeof(Transform)))
            {
                if (o is not Transform t || t.name != listingName) continue;
                var go = t.gameObject;
                if (!go.scene.IsValid() || !go.scene.isLoaded) continue;
                best = t;
                break;
            }

            if (best != null)
                ListingTransformCache[listingName] = best;
            return best;
        }

        private static Transform? FindChildRecursive(Transform root, string childName)
        {
            if (root.name == childName) return root;
            for (int i = 0; i < root.childCount; i++)
            {
                var c = root.GetChild(i);
                if (c.name == childName) return c;
                var deep = FindChildRecursive(c, childName);
                if (deep != null) return deep;
            }
            return null;
        }

        private static void SetTmpText(Transform? target, string text)
        {
            if (target == null || string.IsNullOrEmpty(text)) return;
            var go = target.gameObject;
            const BindingFlags bf = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            foreach (var typeName in new[] { "TMPro.TextMeshPro", "TMPro.TMP_Text" })
            {
                var tmpType = FindType(typeName);
                if (tmpType == null) continue;
                var comp = go.GetComponent(tmpType);
                if (comp == null) continue;
                var prop = tmpType.GetProperty("text", bf) ?? tmpType.GetProperty("Text", bf);
                if (prop != null && prop.CanWrite)
                {
                    try { prop.SetValue(comp, text); return; }
                    catch { }
                }
            }
        }

        private static bool TryRaySendListingText(string businessName, float price)
        {
            try
            {
                var rayNpc = NPC.Get<RayHoffman>();
                if (rayNpc == null) return false;
                rayNpc.SendTextMessage(
                    $"Heads up — {businessName} is listed on the downtown board (${price:N0}). Stop by the office when you want to buy in.");
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static Type? FindType(string fullName)
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                var t = asm.GetType(fullName);
                if (t != null) return t;
            }
            return null;
        }
    }
}
