using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using MoreNPCs.Manager;
using UnityEngine;

namespace MoreNPCs.Utils
{
    /// <summary>
    /// Supplies sprites for RE Office whiteboard <c>PropertyListing … / Image</c> (<see cref="SpriteRenderer"/>).
    /// Tries stems in order: <c>business_{slug}</c>, <c>{slug}</c>, compact slug (underscores removed), a few alias stems, then <c>Tier_{tier}</c>;
    /// for each stem tries <c>.png</c> then <c>.jpg</c>.
    /// Loads from embedded resources (<c>MoreNPCs.Resources.WhiteboardListing.*</c>) or
    /// <c>&lt;mod DLL dir&gt;/Resources/WhiteboardListing/</c>.
    /// Decoded images keep their pixel dimensions; <see cref="Sprite"/> uses the same <see cref="Sprite.pixelsPerUnit"/> as the
    /// renderer’s previous sprite (world size ≈ texWidth / PPU). A fixed PPU of 100 often shrinks relative to vanilla.
    /// </summary>
    public static class WhiteboardListingSpriteLoader
    {
        private static readonly Dictionary<string, Texture2D?> TextureCache =
            new Dictionary<string, Texture2D?>(StringComparer.Ordinal);

        private static readonly Dictionary<string, Sprite?> SpriteCache =
            new Dictionary<string, Sprite?>(StringComparer.Ordinal);

        private static readonly Assembly Asm = typeof(WhiteboardListingSpriteLoader).Assembly;
        private static string? _modDir;

        private static string ModDirectory => _modDir ??= Path.GetDirectoryName(Asm.Location) ?? "";

        public static void TryApplyListingImage(SpriteRenderer? sr, ArtificialBusinessMapping.LaunderTier tier, string businessName)
        {
            if (sr == null) return;
            var existing = sr.sprite;
            var ppu = existing != null && existing.pixelsPerUnit > 1e-6f
                ? existing.pixelsPerUnit
                : 100f;
            var sprite = GetSprite(tier, businessName, ppu);
            if (sprite != null) sr.sprite = sprite;
        }

        private static Sprite? GetSprite(ArtificialBusinessMapping.LaunderTier tier, string businessName, float pixelsPerUnit)
        {
            var slug = ArtificialBusinessCatalog.ListingImageSlug(businessName);
            var cacheKey = $"{(int)tier}|{slug}|{pixelsPerUnit.ToString("G", CultureInfo.InvariantCulture)}";
            if (SpriteCache.TryGetValue(cacheKey, out var cached)) return cached;

            var tex = LoadTextureForTierSlug(tier, slug);
            if (tex == null) return null;

            var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
            SpriteCache[cacheKey] = sprite;
            return sprite;
        }

        private static Texture2D? LoadTextureForTierSlug(ArtificialBusinessMapping.LaunderTier tier, string slug)
        {
            var texKey = $"{(int)tier}|{slug}";
            if (TextureCache.TryGetValue(texKey, out var cached) && cached != null) return cached;

            foreach (var stem in CandidateStems(tier, slug))
            {
                foreach (var ext in ImageExtensions)
                {
                    var fn = $"{stem}{ext}";
                    var tex = TryLoadTextureEmbedded(fn) ?? TryLoadTextureFromModFolder(fn);
                    if (tex != null)
                    {
                        TextureCache[texKey] = tex;
                        return tex;
                    }
                }
            }
            return null;
        }

        private static IEnumerable<string> CandidateStems(ArtificialBusinessMapping.LaunderTier tier, string slug)
        {
            yield return $"business_{slug}";
            yield return slug;
            if (slug.Contains("_"))
            {
                var compact = slug.Replace("_", "");
                if (compact.Length > 0 && !string.Equals(compact, slug, StringComparison.Ordinal))
                    yield return compact;
            }
            if (string.Equals(slug, "bleuballs_boutique", StringComparison.OrdinalIgnoreCase))
                yield return "bleuball_boutique";
            yield return $"Tier_{tier}";
        }

        private static readonly string[] ImageExtensions = { ".png", ".jpg", ".jpeg" };

        private static string EmbeddedResourceName(string fileName) =>
            $"MoreNPCs.Resources.WhiteboardListing.{fileName}";

        private static Texture2D? TryLoadTextureEmbedded(string fileName)
        {
            using var stream = Asm.GetManifestResourceStream(EmbeddedResourceName(fileName));
            if (stream == null) return null;
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return LoadTextureFromBytes(ms.ToArray());
        }

        private static Texture2D? TryLoadTextureFromModFolder(string fileName)
        {
            var path = Path.Combine(ModDirectory, "Resources", "WhiteboardListing", fileName);
            if (!File.Exists(path)) return null;
            try
            {
                return LoadTextureFromBytes(File.ReadAllBytes(path));
            }
            catch
            {
                return null;
            }
        }

        private static Texture2D? LoadTextureFromBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return null;
            var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!UnityEngine.ImageConversion.LoadImage(tex, bytes)) return null;
            return tex;
        }
    }
}
