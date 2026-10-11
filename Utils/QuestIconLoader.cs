using System.Reflection;
using MelonLoader;
using UnityEngine;

namespace MoreNPCs.Utils
{
    /// <summary>Loads quest/contact icons from embedded PNG resources.</summary>
    public static class QuestIconLoader
    {
        public static Sprite? Load(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return null;

            try
            {
                var asm = Assembly.GetExecutingAssembly();
                var target = fileName.ToLowerInvariant();

                foreach (var name in asm.GetManifestResourceNames())
                {
                    if (!name.ToLowerInvariant().EndsWith(target))
                        continue;

                    using var stream = asm.GetManifestResourceStream(name);
                    if (stream == null) continue;

                    var data = new byte[stream.Length];
                        stream.Read(data, 0, data.Length);
                        var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                        if (!tex.LoadImage(data)) return null;
                        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
                }
            }
            catch (System.Exception ex)
            {
                MelonLogger.Warning($"[QuestIconLoader] Failed to load {fileName}: {ex.Message}");
            }

            return null;
        }
    }
}
