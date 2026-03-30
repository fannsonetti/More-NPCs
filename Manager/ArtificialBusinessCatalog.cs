using System;
using System.Collections.Generic;
using MelonLoader;
using MoreNPCs.Utils;

namespace MoreNPCs.Manager
{
    /// <summary>
    /// Listing price (RE-style card) per artificial business. Launder capacity is listing × tier %; daily passive is per tier (T1–T4), not per listing.
    /// </summary>
    public static class ArtificialBusinessCatalog
    {
        public readonly struct Economy
        {
            public float ListingPrice { get; }

            public Economy(float listingPrice)
            {
                ListingPrice = listingPrice;
            }
        }

        private static readonly Dictionary<string, MelonPreferences_Entry<float>> _prefListing =
            new Dictionary<string, MelonPreferences_Entry<float>>(StringComparer.OrdinalIgnoreCase);

        private static readonly Dictionary<string, Economy> ByName =
            new Dictionary<string, Economy>(StringComparer.OrdinalIgnoreCase)
            {
                ["Chinese Restaurant"] = new Economy(4000f),
                ["Barbershop"] = new Economy(5000f),
                ["Thrifty Threads"] = new Economy(6000f),
                ["Top Tattoo"] = new Economy(7000f),

                ["West Gasmart"] = new Economy(16000f),
                ["Bleuball's Boutique"] = new Economy(10000f),
                ["Auto Shop"] = new Economy(12000f),
                ["Pawn Shop"] = new Economy(14000f),

                ["Central Gasmart"] = new Economy(20000f),
                ["Thompson Construction"] = new Economy(25000f),
                ["Koyama Chemical"] = new Economy(30000f),
                ["Hyland Auto"] = new Economy(35000f),

                ["Hyland Bank"] = new Economy(80000f),
                ["Casino"] = new Economy(100000f)
            };

        /// <summary>Call from <see cref="MoreNPCs.Utils.MoreNPCsPreferences.Register"/> after creating the category.</summary>
        public static void RegisterBusinessEconomyEntries(MelonPreferences_Category category)
        {
            _prefListing.Clear();
            foreach (var kv in ByName)
            {
                var key = kv.Key;
                var e = kv.Value;
                var slug = PrefSlug(key);
                _prefListing[key] = category.CreateEntry($"listing_{slug}", e.ListingPrice, key);
            }
        }

        private static string PrefSlug(string businessName)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var c in businessName)
            {
                if (char.IsLetterOrDigit(c)) sb.Append(char.ToLowerInvariant(c));
                else if (c == ' ' || c == '\'' || c == '-' || c == '.') sb.Append('_');
            }
            var s = sb.ToString();
            while (s.Contains("__")) s = s.Replace("__", "_");
            return s.Trim('_');
        }

        /// <summary>File name stem for whiteboard listing PNGs (e.g. <c>business_chinese_restaurant.png</c>).</summary>
        public static string ListingImageSlug(string businessName) =>
            string.IsNullOrWhiteSpace(businessName) ? "" : PrefSlug(businessName.Trim());

        private static Economy ResolveEconomy(string trimmedName, Economy fallback)
        {
            if (_prefListing.Count == 0) return fallback;
            if (!_prefListing.TryGetValue(trimmedName, out var pl)) return fallback;
            return new Economy(pl.Value);
        }

        public static bool TryGetEconomy(string businessName, out Economy economy)
        {
            economy = default;
            if (string.IsNullOrWhiteSpace(businessName)) return false;
            var key = businessName.Trim();
            if (ExperimentalArtificialProperties.IsArtificialPropertyDisabled(key)) return false;
            if (!ByName.TryGetValue(key, out var baseE)) return false;
            economy = ResolveEconomy(key, baseE);
            return true;
        }

        public static bool TryGetListingPrice(string businessName, out float price)
        {
            price = 0f;
            if (!TryGetEconomy(businessName, out var e)) return false;
            price = e.ListingPrice;
            return true;
        }

        public static float GetListingPrice(string businessName)
        {
            return TryGetListingPrice(businessName, out var p) ? p : 0f;
        }

        /// <summary>Catalog keys, longest first so substring checks prefer e.g. “West Gasmart” over “Gasmart”.</summary>
        public static void AppendCatalogBusinessNamesLongestFirst(List<string> dest)
        {
            foreach (var k in ByName.Keys)
                dest.Add(k);
            dest.Sort((a, b) => b.Length.CompareTo(a.Length));
        }
    }
}
