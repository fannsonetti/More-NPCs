using MelonLoader;
using MoreNPCs.Utils;
using UnityEngine;

namespace MoreNPCs.Distributor
{
    /// <summary>Richard's trawler and other venue set dressing.</summary>
    internal static class DistributorPlaceholderProps
    {
        private static GameObject? _boat;
        private static GameObject? _truck;

        public static GameObject? TrawlerRoot => _boat;

        public static void ShowForVenue(DistributorVenue venue)
        {
            HideAll();

            switch (venue)
            {
                case DistributorVenue.DocksBoat:
                case DistributorVenue.Meetup:
                    SpawnTrawler();
                    break;
                case DistributorVenue.NorthtownTruck:
                    SpawnTruckPlaceholder();
                    break;
            }
        }

        public static void HideAll()
        {
            if (_boat != null) UnityEngine.Object.Destroy(_boat);
            if (_truck != null) UnityEngine.Object.Destroy(_truck);
            _boat = null;
            _truck = null;
        }

        private static void SpawnTrawler()
        {
            var model = GlbModelLoader.LoadTrawler();
            if (model == null)
            {
                MelonLogger.Warning("DistributorPlaceholderProps: trawler GLB failed; using cube fallback.");
                _boat = CreatePlaceholder(
                    "DistributorBoatFallback",
                    DistributorConfig.TrawlerPosition,
                    new Vector3(8f, 3f, 18f),
                    new Color(0.12f, 0.14f, 0.16f));
                ApplyTrawlerTransform(_boat);
                return;
            }

            ApplyTrawlerTransform(model);
            _boat = model;
        }

        private static void ApplyTrawlerTransform(GameObject model)
        {
            model.transform.position = DistributorConfig.TrawlerPosition;
            model.transform.localScale = Vector3.one * DistributorConfig.TrawlerUniformScale;
            // GLB mesh is pre-oriented; Unity shows 0,0,0 — inspector tweaks are local euler.
            model.transform.localRotation = Quaternion.Euler(DistributorConfig.TrawlerEuler);
        }

        private static void SpawnTruckPlaceholder()
        {
            _truck = CreatePlaceholder(
                "DistributorTruckPlaceholder",
                DistributorConfig.MeetPosition,
                new Vector3(5f, 2.5f, 10f),
                new Color(0.25f, 0.25f, 0.28f));
        }

        private static GameObject CreatePlaceholder(string name, Vector3 position, Vector3 scale, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.position = position;
            go.transform.localScale = scale;

            var renderer = go.GetComponent<Renderer>();
            if (renderer != null)
            {
                var mat = new Material(Shader.Find("Standard"));
                mat.color = color;
                renderer.material = mat;
            }

            var collider = go.GetComponent<Collider>();
            if (collider != null) UnityEngine.Object.Destroy(collider);

            return go;
        }
    }
}
