using System;
using System.Collections;
using System.Reflection;
using MelonLoader;
using ScheduleOne.DevUtilities;
using ScheduleOne.Money;
using ScheduleOne.PlayerScripts;
using ScheduleOne.Vehicles;
using UnityEngine;

namespace MoreNPCs.Utils
{
    internal static class PoliceVehicleHelper
    {
        public const string ShitboxPoliceCode = "shitbox_police";
        public const string BruiserPoliceCode = "bruiser_police";

        public const float ShitboxPolicePrice = 25000f;
        public const float BruiserPolicePrice = 30000f;

        public static readonly Vector3 SpawnPosition = new Vector3(16.3f, 1.2f, 34.5f);
        public static readonly Vector3 SpawnEuler = Vector3.zero;

        private static float _nextTrunkScanTime;
        private static Type? _policeLightType;
        private static MethodInfo? _policeLightSetIsOn;
        private static FieldInfo? _policeLightIsOnField;

        public static void UpdateTrunkWatcher()
        {
            if (Time.time < _nextTrunkScanTime) return;
            _nextTrunkScanTime = Time.time + 3f;
            FixAllOwnedPoliceShitboxTrunks();
        }

        public static void UpdatePoliceVehicleInput()
        {
            if (!Input.GetKeyDown(KeyCode.R)) return;

            try
            {
                if (!TryGetLocalPoliceVehicle(out var vehicle) || vehicle == null) return;
                ToggleLightbar(vehicle);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"PoliceVehicleHelper.UpdatePoliceVehicleInput: {ex.Message}");
            }
        }

        public static bool TryPurchaseAndSpawn(string vehicleCode, float price, out string failureMessage)
        {
            failureMessage = string.Empty;

            if (!PlayerBankHelper.HasOnlineBalance(price))
            {
                failureMessage = $"You need {MoneyManager.FormatAmount(price, false, false)} on your card.";
                return false;
            }

            if (!TrySpawnOwnedPoliceVehicle(vehicleCode, out _))
            {
                failureMessage = "Didn't work. Try again.";
                return false;
            }

            if (!PlayerBankHelper.TryChargeOnline(price, "Motor Pool Purchase", vehicleCode))
            {
                failureMessage = $"You need {MoneyManager.FormatAmount(price, false, false)} on your card.";
                return false;
            }

            return true;
        }

        public static bool TrySpawnOwnedPoliceVehicle(string vehicleCode, out LandVehicle? vehicle)
        {
            vehicle = null;
            if (!NetworkSingleton<VehicleManager>.InstanceExists)
            {
                MelonLogger.Warning("PoliceVehicleHelper: VehicleManager not ready.");
                return false;
            }

            var rotation = Quaternion.Euler(SpawnEuler);
            var manager = NetworkSingleton<VehicleManager>.Instance;

            try
            {
                vehicle = manager.SpawnAndReturnVehicle(
                    vehicleCode,
                    SpawnPosition,
                    rotation,
                    playerOwned: true);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"PoliceVehicleHelper: SpawnAndReturnVehicle threw for '{vehicleCode}': {ex.Message}");
            }

            vehicle ??= FindSpawnedPoliceVehicle(manager, vehicleCode);
            if (vehicle == null)
            {
                MelonLogger.Warning($"PoliceVehicleHelper: failed to spawn '{vehicleCode}'.");
                return false;
            }

            ScheduleTrunkFix(vehicle);
            MelonLogger.Msg($"PoliceVehicleHelper: spawned owned '{vehicleCode}' at {SpawnPosition}.");
            return true;
        }

        private static LandVehicle? FindSpawnedPoliceVehicle(VehicleManager manager, string vehicleCode)
        {
            try
            {
                var list = manager.PlayerOwnedVehicles;
                if (list != null)
                {
                    LandVehicle? best = null;
                    float bestDistance = 25f;

                    for (int i = 0; i < list.Count; i++)
                    {
                        var owned = list[i];
                        if (owned == null) continue;
                        if (!string.Equals(owned.VehicleCode, vehicleCode, StringComparison.OrdinalIgnoreCase))
                            continue;

                        float distance = Vector3.Distance(owned.transform.position, SpawnPosition);
                        if (distance > bestDistance) continue;

                        if (best == null || distance < bestDistance)
                        {
                            best = owned;
                            bestDistance = distance;
                        }
                    }

                    if (best != null) return best;
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"PoliceVehicleHelper.FindSpawnedPoliceVehicle: {ex.Message}");
            }

            return FindPoliceVehicleNearSpawn(vehicleCode);
        }

        private static LandVehicle? FindPoliceVehicleNearSpawn(string vehicleCode)
        {
            foreach (var vehicle in UnityEngine.Object.FindObjectsOfType<LandVehicle>())
            {
                if (vehicle == null || !vehicle.IsPlayerOwned) continue;
                if (!string.Equals(vehicle.VehicleCode, vehicleCode, StringComparison.OrdinalIgnoreCase)) continue;
                if (Vector3.Distance(vehicle.transform.position, SpawnPosition) > 25f) continue;
                return vehicle;
            }

            return null;
        }

        public static void TryFixTrunkOnVehicle(LandVehicle? vehicle)
        {
            if (vehicle == null) return;
            if (!IsPoliceShitbox(vehicle.VehicleCode)) return;
            EnsureShitboxTrunkInteraction(vehicle.gameObject);
        }

        public static void FixAllOwnedPoliceShitboxTrunks()
        {
            try
            {
                if (!NetworkSingleton<VehicleManager>.InstanceExists)
                    return;

                var list = NetworkSingleton<VehicleManager>.Instance.PlayerOwnedVehicles;
                if (list == null) return;

                for (int i = 0; i < list.Count; i++)
                    TryFixTrunkOnVehicle(list[i]);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"PoliceVehicleHelper.FixAllOwnedPoliceShitboxTrunks: {ex.Message}");
            }
        }

        public static void EnsureShitboxTrunkInteraction(GameObject vehicleRoot)
        {
            if (vehicleRoot == null) return;

            var suv = FindChildTransform(vehicleRoot.transform, "SUV");
            if (suv != null && !suv.gameObject.activeSelf)
                suv.gameObject.SetActive(true);

            foreach (var transform in vehicleRoot.GetComponentsInChildren<Transform>(true))
            {
                if (!string.Equals(transform.name, "TrunkInteraction", StringComparison.OrdinalIgnoreCase))
                    continue;

                ActivateParentChain(transform, vehicleRoot.transform);

                var go = transform.gameObject;
                if (!go.activeSelf)
                    go.SetActive(true);

                foreach (var behaviour in go.GetComponents<Behaviour>())
                {
                    if (behaviour != null && !behaviour.enabled)
                        behaviour.enabled = true;
                }

                foreach (var collider in go.GetComponentsInChildren<Collider>(true))
                {
                    if (collider != null)
                        collider.enabled = true;
                }
            }
        }

        public static void ScheduleTrunkFixAfterLoad()
        {
            MelonCoroutines.Start(FixTrunksAfterLoadRoutine());
        }

        private static void ScheduleTrunkFix(LandVehicle vehicle)
        {
            MelonCoroutines.Start(DeferredTrunkFix(vehicle));
        }

        private static IEnumerator DeferredTrunkFix(LandVehicle vehicle)
        {
            for (int i = 0; i < 5; i++)
            {
                yield return null;
                try
                {
                    TryFixTrunkOnVehicle(vehicle);
                }
                catch (Exception ex)
                {
                    MelonLogger.Warning($"PoliceVehicleHelper.DeferredTrunkFix: {ex.Message}");
                }
            }
        }

        private static IEnumerator FixTrunksAfterLoadRoutine()
        {
            for (int i = 0; i < 12; i++)
            {
                yield return new WaitForSeconds(2f);
                FixAllOwnedPoliceShitboxTrunks();
            }
        }

        private static void ToggleLightbar(LandVehicle vehicle)
        {
            EnsurePoliceLightTypes();
            if (_policeLightType == null || _policeLightSetIsOn == null) return;

            var lightbar = FindChildTransform(vehicle.transform, "Lightbar");
            if (lightbar == null) return;

            var policeLight = lightbar.GetComponent(_policeLightType);
            if (policeLight == null) return;

            bool isOn = _policeLightIsOnField != null && Convert.ToBoolean(_policeLightIsOnField.GetValue(policeLight));
            _policeLightSetIsOn.Invoke(policeLight, new object[] { !isOn });
        }

        private static void EnsurePoliceLightTypes()
        {
            if (_policeLightType != null) return;

            _policeLightType = FindType("ScheduleOne.Lighting.PoliceLight");
            if (_policeLightType == null) return;

            _policeLightSetIsOn = _policeLightType.GetMethod("SetIsOn", BindingFlags.Public | BindingFlags.Instance);
            _policeLightIsOnField = _policeLightType.GetField("IsOn", BindingFlags.Public | BindingFlags.Instance);
        }

        private static void ActivateParentChain(Transform node, Transform root)
        {
            var current = node;
            while (current != null && current != root)
            {
                if (!current.gameObject.activeSelf)
                    current.gameObject.SetActive(true);
                current = current.parent;
            }
        }

        private static Transform? FindChildTransform(Transform root, string name)
        {
            foreach (var child in root.GetComponentsInChildren<Transform>(true))
            {
                if (string.Equals(child.name, name, StringComparison.OrdinalIgnoreCase))
                    return child;
            }

            return null;
        }

        private static bool IsPoliceVehicle(string? vehicleCode) => IsPoliceVehicleCode(vehicleCode);

        public static bool IsLocalPlayerInPoliceVehicle() =>
            TryGetLocalPoliceVehicle(out _);

        public static bool TryGetLocalPoliceVehicle(out LandVehicle? vehicle)
        {
            vehicle = null;
            try
            {
                var player = Player.Local;
                if (player == null || !player.IsInVehicle || player.CurrentVehicle == null)
                    return false;

                vehicle = player.CurrentVehicle.GetComponent<LandVehicle>();
                return vehicle != null && IsPoliceVehicleCode(vehicle.VehicleCode);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsPoliceVehicleCode(string? vehicleCode) =>
            IsPoliceShitbox(vehicleCode)
            || IsPoliceCruiser(vehicleCode);


        private static bool IsPoliceCruiser(string? vehicleCode) =>
            !string.IsNullOrEmpty(vehicleCode)
            && vehicleCode.Equals(BruiserPoliceCode, StringComparison.OrdinalIgnoreCase);

        private static bool IsPoliceShitbox(string? vehicleCode) =>
            !string.IsNullOrEmpty(vehicleCode)
            && vehicleCode.Equals(ShitboxPoliceCode, StringComparison.OrdinalIgnoreCase);

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
