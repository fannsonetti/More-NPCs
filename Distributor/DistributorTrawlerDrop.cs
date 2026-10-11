using System;
using System.Collections;
using System.Reflection;
using MelonLoader;
using UnityEngine;

namespace MoreNPCs.Distributor
{
    /// <summary>Reachable pier locker for the deal; trawler offshore is scenery only.</summary>
    internal static class DistributorTrawlerDrop
    {
        public const string DisplayName = "Docks pier locker";

        private static GameObject? _root;
        private static object? _storage;

        public static Vector3 Waypoint => _root != null
            ? _root.transform.position
            : DistributorConfig.PierDropPosition;

        public static bool TryResolve(out object? storage, out Vector3 waypoint, out string label)
        {
            storage = _storage;
            waypoint = Waypoint;
            label = DisplayName;
            return _storage != null;
        }

        public static bool EnsurePierDrop()
        {
            if (_storage != null) return true;

            try
            {
                if (!TryFindTemplateDeadDrop(out var templateGo))
                {
                    MelonLogger.Warning("DistributorTrawlerDrop: no dead-drop template found.");
                    return false;
                }

                _root = UnityEngine.Object.Instantiate(templateGo);
                _root.name = "DistributorPierLocker";
                _root.transform.position = DistributorConfig.PierDropPosition;
                _root.transform.rotation = Quaternion.Euler(DistributorConfig.PierDropEuler);

                UnregisterDeadDrop(_root);
                DisablePoI(_root);
                AssignFreshGuid(_root);
                ConfigureStorageLabels(_root);

                _storage = FindStorageComponent(_root);
                if (_storage == null)
                {
                    MelonLogger.Warning("DistributorTrawlerDrop: cloned root has no storage.");
                    Destroy();
                    return false;
                }

                MelonLogger.Msg("DistributorTrawlerDrop: pier locker ready at {0}.", DistributorConfig.PierDropPosition);
                DistributorPierLadder.Ensure();
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"DistributorTrawlerDrop.EnsurePierDrop: {ex.Message}");
                Destroy();
                return false;
            }
        }

        public static void Destroy()
        {
            if (_root != null)
                UnityEngine.Object.Destroy(_root);
            _root = null;
            _storage = null;
            DistributorPierLadder.Destroy();
        }

        private static bool TryFindTemplateDeadDrop(out GameObject templateGo)
        {
            templateGo = null!;
            var deadDropType = FindType("ScheduleOne.Economy.DeadDrop");
            if (deadDropType == null) return false;

            var listField = deadDropType.GetField("DeadDrops", BindingFlags.Public | BindingFlags.Static);
            if (listField?.GetValue(null) is not IEnumerable drops) return false;

            foreach (var drop in drops)
            {
                if (drop is not Component component) continue;
                var storage = deadDropType.GetField("Storage")?.GetValue(drop);
                if (storage == null) continue;
                templateGo = component.gameObject;
                return true;
            }

            return false;
        }

        private static void UnregisterDeadDrop(GameObject root)
        {
            var deadDropType = FindType("ScheduleOne.Economy.DeadDrop");
            if (deadDropType == null) return;

            var deadDrop = root.GetComponent(deadDropType);
            if (deadDrop == null) return;

            if (deadDropType.GetField("DeadDrops")?.GetValue(null) is IList list)
                list.Remove(deadDrop);

            if (deadDrop is Component component)
                UnityEngine.Object.Destroy(component);
        }

        private static void DisablePoI(GameObject root)
        {
            foreach (var behaviour in root.GetComponentsInChildren<Behaviour>(true))
            {
                if (behaviour == null) continue;
                var name = behaviour.GetType().FullName ?? string.Empty;
                if (name.EndsWith(".POI", StringComparison.Ordinal) || name.Contains("POI"))
                    behaviour.gameObject.SetActive(false);
            }
        }

        private static void AssignFreshGuid(GameObject root)
        {
            var storageType = FindType("ScheduleOne.Storage.WorldStorageEntity");
            if (storageType == null) return;

            var storage = root.GetComponent(storageType);
            if (storage == null) return;

            string guid = Guid.NewGuid().ToString();
            SetMember(storage, "BakedGUID", guid);

            var guidManagerType = FindType("ScheduleOne.Persistence.GUIDManager");
            guidManagerType?.GetMethod("RegisterObject", BindingFlags.Public | BindingFlags.Static)
                ?.Invoke(null, new[] { storage });
        }

        private static void ConfigureStorageLabels(GameObject root)
        {
            var storageType = FindType("ScheduleOne.Storage.StorageEntity");
            if (storageType == null) return;

            var storage = root.GetComponent(storageType);
            if (storage == null) return;

            SetMember(storage, "StorageEntityName", "Locker");
            SetMember(storage, "StorageEntitySubtitle", DisplayName);
            storageType.GetProperty("MaxAccessDistance")?.SetValue(storage, 8f);

            var interactableType = FindType("ScheduleOne.Storage.StorageEntityInteractable");
            if (interactableType == null) return;

            foreach (var interactable in root.GetComponentsInChildren(interactableType, true))
            {
                interactable.GetType()
                    .GetMethod("SetMessage", BindingFlags.Public | BindingFlags.Instance)
                    ?.Invoke(interactable, new object[] { "Open locker" });
            }
        }

        private static object? FindStorageComponent(GameObject root)
        {
            var storageType = FindType("ScheduleOne.Storage.WorldStorageEntity")
                ?? FindType("ScheduleOne.Storage.StorageEntity");
            return storageType != null ? root.GetComponent(storageType) : null;
        }

        private static void SetMember(object instance, string name, object value)
        {
            var type = instance.GetType();
            var field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null) { field.SetValue(instance, value); return; }
            type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(instance, value);
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
