using System;
using System.Collections.Generic;
using MoreNPCs.Utils;

namespace MoreNPCs.Manager
{
    /// <summary>
    /// Artificial businesses grouped into RE Office property slots (T1–T4). Launder capacity is
    /// listing price × tier percent (T1 50%, T2 40%, T3 30%, T4 20%). T5 is reserved for a future slot.
    /// Daily passive income is configured per tier (T1–T4), applied once per tier among assignments, not per property.
    /// </summary>
    public static class ArtificialBusinessMapping
    {
        public enum LaunderTier
        {
            Laundromat = 1,
            PostOffice = 2,
            CarWash = 3,
            TacoTicklers = 4,
            /// <summary>Reserved — not mapped to any businesses yet.</summary>
            Tier5 = 5
        }

        /// <summary>Fraction of listing price laundered per day for the tier (T5 reserved).</summary>
        public static float LaunderFractionForTier(LaunderTier tier) =>
            tier switch
            {
                LaunderTier.Laundromat => 0.50f,
                LaunderTier.PostOffice => 0.40f,
                LaunderTier.CarWash => 0.30f,
                LaunderTier.TacoTicklers => 0.20f,
                LaunderTier.Tier5 => 0.10f,
                _ => 0f
            };

        /// <summary>RE Office whiteboard: owning the matching property unlocks all businesses in that tier.</summary>
        public static IEnumerable<string> GetArtificialBusinessNamesForTier(LaunderTier tier)
        {
            foreach (var kv in _byExactName)
            {
                if (kv.Value != tier) continue;
                if (ExperimentalArtificialProperties.IsArtificialPropertyDisabled(kv.Key)) continue;
                yield return kv.Key;
            }
        }

        /// <summary>Insertion order = display priority for whiteboard / first pick.</summary>
        private static readonly Dictionary<string, LaunderTier> _byExactName =
            new Dictionary<string, LaunderTier>(StringComparer.OrdinalIgnoreCase)
            {
                ["Chinese Restaurant"] = LaunderTier.Laundromat,
                ["Barbershop"] = LaunderTier.Laundromat,
                ["Thrifty Threads"] = LaunderTier.Laundromat,
                ["Top Tattoo"] = LaunderTier.Laundromat,

                ["West Gasmart"] = LaunderTier.PostOffice,
                ["Bleuball's Boutique"] = LaunderTier.PostOffice,
                ["Auto Shop"] = LaunderTier.PostOffice,
                ["Pawn Shop"] = LaunderTier.PostOffice,

                ["Thompson Construction"] = LaunderTier.CarWash,
                ["Koyama Chemical"] = LaunderTier.CarWash,
                ["Central Gasmart"] = LaunderTier.CarWash,
                ["Hyland Auto"] = LaunderTier.CarWash,

                ["Hyland Bank"] = LaunderTier.TacoTicklers,
                ["Casino"] = LaunderTier.TacoTicklers
            };

        private static float TierDailyEarnings(LaunderTier t)
        {
            if (!MoreNPCsPreferences.Registered)
            {
                return t switch
                {
                    LaunderTier.Laundromat => 200f,
                    LaunderTier.PostOffice => 400f,
                    LaunderTier.CarWash => 600f,
                    LaunderTier.TacoTicklers => 800f,
                    _ => 0f
                };
            }
            return t switch
            {
                LaunderTier.Laundromat => MoreNPCsPreferences.Tier_Laundromat_DailyPassive.Value,
                LaunderTier.PostOffice => MoreNPCsPreferences.Tier_PostOffice_DailyPassive.Value,
                LaunderTier.CarWash => MoreNPCsPreferences.Tier_CarWash_DailyPassive.Value,
                LaunderTier.TacoTicklers => MoreNPCsPreferences.Tier_TacoTicklers_DailyPassive.Value,
                LaunderTier.Tier5 => 0f,
                _ => 0f
            };
        }

        /// <summary>Mid-tier listing when only a keyword match exists (no catalog row).</summary>
        private static float DefaultListingForTier(LaunderTier t) =>
            t switch
            {
                LaunderTier.Laundromat => 5500f,
                LaunderTier.PostOffice => 12000f,
                LaunderTier.CarWash => 27500f,
                LaunderTier.TacoTicklers => 90000f,
                LaunderTier.Tier5 => 150000f,
                _ => 0f
            };

        public static bool TryGetTier(string businessName, out LaunderTier tier)
        {
            tier = default;
            if (string.IsNullOrWhiteSpace(businessName)) return false;
            var key = businessName.Trim();
            if (ExperimentalArtificialProperties.IsArtificialPropertyDisabled(key)) return false;
            return _byExactName.TryGetValue(key, out tier);
        }

        /// <summary>Map name to tier using catalog + keyword hints (for sorting / passive).</summary>
        public static bool TryInferTier(string businessName, out LaunderTier tier)
        {
            tier = default;
            if (string.IsNullOrWhiteSpace(businessName)) return false;
            if (TryGetTier(businessName, out tier)) return true;
            var key = businessName.Trim().ToLowerInvariant();
            if (key.Contains("laundromat")) { tier = LaunderTier.Laundromat; return true; }
            if (key.Contains("post office") || key.Contains("postoffice")) { tier = LaunderTier.PostOffice; return true; }
            if (key.Contains("carwash") || key.Contains("car wash")) { tier = LaunderTier.CarWash; return true; }
            if (key.Contains("taco") || key.Contains("tickler")) { tier = LaunderTier.TacoTicklers; return true; }
            return false;
        }

        /// <summary>Daily passive for one business name = its tier’s rate (for UI); earnings are capped per tier via <see cref="GetDailyPassiveEarningsTotalForAssignments"/>.</summary>
        public static float GetDailyPassiveEarnings(string businessName)
        {
            return TryInferTier(businessName, out var tier) ? TierDailyEarnings(tier) : 0f;
        }

        /// <summary>One passive payout per tier slot among assigned businesses (not multiplied by how many listings in that tier).</summary>
        public static float GetDailyPassiveEarningsTotalForAssignments(IReadOnlyList<string>? assigned)
        {
            if (assigned == null || assigned.Count == 0) return 0f;
            var tiers = new HashSet<LaunderTier>();
            foreach (var name in assigned)
            {
                if (string.IsNullOrWhiteSpace(name)) continue;
                if (TryInferTier(name.Trim(), out var tier))
                    tiers.Add(tier);
            }
            float total = 0f;
            foreach (var t in tiers)
                total += TierDailyEarnings(t);
            return total;
        }

        /// <summary>Daily launder = listing price × tier launder fraction.</summary>
        public static float GetDailyLaunderCapacity(string businessName)
        {
            if (string.IsNullOrWhiteSpace(businessName)) return 0f;
            var key = businessName.Trim();
            if (ExperimentalArtificialProperties.IsArtificialPropertyDisabled(key)) return 0f;

            if (TryGetTier(key, out var tier))
            {
                var price = ArtificialBusinessCatalog.TryGetListingPrice(key, out var p) ? p : DefaultListingForTier(tier);
                return Math.Max(0f, price * LaunderFractionForTier(tier));
            }

            return KeywordFallbackCapacity(key);
        }

        public static bool MatchesRouteStop(string routeStopDisplayName, string assignedBusinessName)
        {
            if (string.IsNullOrWhiteSpace(routeStopDisplayName) || string.IsNullOrWhiteSpace(assignedBusinessName))
                return false;
            if (!TryGetTier(assignedBusinessName, out var tier))
                return false;
            var r = routeStopDisplayName.Trim().ToLowerInvariant();
            if (r.Contains("taco") || r.Contains("tickler")) return tier == LaunderTier.TacoTicklers;
            if (r.Contains("laundromat")) return tier == LaunderTier.Laundromat;
            if (r.Contains("car") && r.Contains("wash")) return tier == LaunderTier.CarWash;
            if (r.Contains("post") && r.Contains("office")) return tier == LaunderTier.PostOffice;
            return false;
        }

        private static float KeywordFallbackCapacity(string businessName)
        {
            var key = businessName.Trim().ToLowerInvariant();
            if (key.Contains("laundromat"))
                return DefaultListingForTier(LaunderTier.Laundromat) * LaunderFractionForTier(LaunderTier.Laundromat);
            if (key.Contains("post") && key.Contains("office"))
                return DefaultListingForTier(LaunderTier.PostOffice) * LaunderFractionForTier(LaunderTier.PostOffice);
            if (key.Contains("car") && key.Contains("wash"))
                return DefaultListingForTier(LaunderTier.CarWash) * LaunderFractionForTier(LaunderTier.CarWash);
            if (key.Contains("taco") || key.Contains("tickler"))
                return DefaultListingForTier(LaunderTier.TacoTicklers) * LaunderFractionForTier(LaunderTier.TacoTicklers);
            return 0f;
        }
    }
}
