using System.Collections.Generic;
using System.IO;
using MelonLoader;
using S1MAPI.Gltf;
using UnityEngine;
using UnityEngine.Rendering;

namespace MoreNPCs.Utils
{
    internal static class GlbModelLoader
    {
        internal const string TrawlerEmbeddedResource =
            "MoreNPCs.Resources.Models.salt-beaten_fishing_trawler_game-ready.glb";
        internal const string TrawlerRelativePath =
            "Resources/Models/salt-beaten_fishing_trawler_game-ready.glb";

        private static readonly Dictionary<string, GameObject> ModelCache = new();

        internal static GameObject? LoadWorldProp(
            string embeddedResourceName,
            string relativePathFromModDir,
            string objectName)
        {
            var cacheKey = $"{embeddedResourceName}|{relativePathFromModDir}";
            if (ModelCache.TryGetValue(cacheKey, out var cached) && cached != null)
                return InstantiateCached(cached, objectName);

            var bytes = ModAssetLoader.LoadBytes(embeddedResourceName, relativePathFromModDir);
            if (bytes == null || bytes.Length == 0)
            {
                MelonLogger.Warning($"MoreNPCs: missing GLB '{relativePathFromModDir}'.");
                return null;
            }

            var model = GltfLoader.LoadGlb(bytes);
            if (model == null)
            {
                MelonLogger.Warning($"MoreNPCs: failed to load GLB '{relativePathFromModDir}'.");
                return null;
            }

            PrepareWorldProp(model, Path.GetFileNameWithoutExtension(relativePathFromModDir));
            model.SetActive(false);
            UnityEngine.Object.DontDestroyOnLoad(model);
            ModelCache[cacheKey] = model;

            MelonLogger.Msg($"MoreNPCs: loaded GLB '{relativePathFromModDir}' ({bytes.Length} bytes).");
            return InstantiateCached(model, objectName);
        }

        internal static GameObject? LoadTrawler(string objectName = "DistributorTrawler") =>
            LoadWorldProp(TrawlerEmbeddedResource, TrawlerRelativePath, objectName);

        private static void PrepareWorldProp(GameObject model, string cacheName)
        {
            model.name = $"Cached_{cacheName}";

            foreach (var collider in model.GetComponentsInChildren<Collider>(true))
                UnityEngine.Object.Destroy(collider);

            foreach (var renderer in model.GetComponentsInChildren<Renderer>(true))
            {
                renderer.enabled = true;
                renderer.shadowCastingMode = ShadowCastingMode.On;
                ConfigureDoubleSidedRenderers(renderer);
            }

            SetLayerRecursively(model, LayerMask.NameToLayer("Default"));
        }

        private static GameObject InstantiateCached(GameObject cached, string objectName)
        {
            var clone = UnityEngine.Object.Instantiate(cached);
            clone.name = objectName;
            clone.SetActive(true);
            return clone;
        }

        private static void ConfigureDoubleSidedRenderers(Renderer renderer)
        {
            var materials = renderer.materials;
            for (var i = 0; i < materials.Length; i++)
                ConfigureDoubleSided(materials[i]);
            renderer.materials = materials;
        }

        private static void ConfigureDoubleSided(Material material)
        {
            if (material == null) return;

            material.SetInt("_Cull", (int)CullMode.Off);
            if (material.HasProperty("_CullMode"))
                material.SetInt("_CullMode", (int)CullMode.Off);
            if (material.HasProperty("_DoubleSidedEnable"))
                material.SetFloat("_DoubleSidedEnable", 1f);
        }

        private static void SetLayerRecursively(GameObject root, int layer)
        {
            if (layer < 0) return;

            root.layer = layer;
            foreach (Transform child in root.transform)
                SetLayerRecursively(child.gameObject, layer);
        }
    }
}
