using System;
using System.Collections;
using MelonLoader;
using ScheduleOne.DevUtilities;
using ScheduleOne.PlayerScripts;
using ScheduleOne.UI;
using ScheduleOne.Vehicles;
using TMPro;
using UnityEngine;

namespace MoreNPCs.Utils
{
    /// <summary>
    /// Extra driver prompt for police lightbar (R). Leaves the vanilla headlights prompt alone;
    /// sirens is a visual clone with InputPrompt removed so text cannot be overwritten.
    /// Template only exists after a save is loaded (not on the main menu).
    /// </summary>
    internal static class PoliceVehicleSirenPrompt
    {
        private const string HeadlightsPromptName = "InputPrompt (Headlights)";
        private const string SirensPromptName = "InputPrompt (Sirens)";
        private static readonly Vector2 SirensAnchoredOffset = new Vector2(0f, 40f);

        private static GameObject? _sirensPrompt;
        private static bool _shown;
        private static bool _waitRoutineRunning;

        public static void Initialize()
        {
            // Do nothing at Melon init — VehicleCanvas/headlights only exist in a loaded game.
        }

        public static void Update()
        {
            // Unity destroys UI when returning to menu; drop stale refs.
            if (_sirensPrompt != null && _sirensPrompt.Equals(null))
            {
                _sirensPrompt = null;
                _shown = false;
            }

            if (!IsWorldReady())
            {
                Hide();
                return;
            }

            if (IsLocalDriverInPoliceVehicle())
                Show();
            else
                Hide();
        }

        public static void EnsureForVehicle(LandVehicle vehicle)
        {
            if (vehicle == null || !vehicle.LocalPlayerIsDriver) return;
            if (!PoliceVehicleHelper.IsPoliceVehicleCode(vehicle.VehicleCode)) return;
            if (!IsWorldReady()) return;

            if (!TryCreateSirensPrompt())
            {
                if (!_waitRoutineRunning)
                    MelonCoroutines.Start(WaitAndShow());
                return;
            }

            Show();
        }

        public static void Hide()
        {
            if (_sirensPrompt != null && !_sirensPrompt.Equals(null) && _sirensPrompt.activeSelf)
                _sirensPrompt.SetActive(false);
            _shown = false;
        }

        private static void Show()
        {
            if (!IsWorldReady()) return;

            if (!TryCreateSirensPrompt() || _sirensPrompt == null)
                return;

            if (!_sirensPrompt.activeSelf)
                _sirensPrompt.SetActive(true);

            if (!_shown)
            {
                ForceSirensText(_sirensPrompt);
                _shown = true;
            }

            _sirensPrompt.GetComponent<RectTransform>()?.SetAsLastSibling();
        }

        private static IEnumerator WaitAndShow()
        {
            _waitRoutineRunning = true;
            try
            {
                for (int i = 0; i < 40; i++)
                {
                    if (!IsWorldReady())
                    {
                        yield return new WaitForSeconds(0.5f);
                        continue;
                    }

                    if (TryCreateSirensPrompt())
                    {
                        Show();
                        yield break;
                    }

                    yield return new WaitForSeconds(0.25f);
                }
            }
            finally
            {
                _waitRoutineRunning = false;
            }
        }

        /// <summary>True once a save is loaded and vehicle HUD can exist.</summary>
        private static bool IsWorldReady()
        {
            try
            {
                if (Player.Local == null)
                    return false;

                if (Singleton<VehicleCanvas>.InstanceExists)
                    return true;

                return GameObject.Find("VehicleCanvas") != null;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsLocalDriverInPoliceVehicle()
        {
            try
            {
                var player = Player.Local;
                if (player == null || !player.IsInVehicle || player.CurrentVehicle == null)
                    return false;

                var vehicle = player.CurrentVehicle.GetComponent<LandVehicle>();
                return vehicle != null
                       && vehicle.LocalPlayerIsDriver
                       && PoliceVehicleHelper.IsPoliceVehicleCode(vehicle.VehicleCode);
            }
            catch
            {
                return false;
            }
        }

        private static bool TryCreateSirensPrompt()
        {
            if (_sirensPrompt != null && !_sirensPrompt.Equals(null))
                return true;

            _sirensPrompt = null;

            if (!IsWorldReady())
                return false;

            var template = FindHeadlightsPrompt();
            if (template == null)
                return false;

            if (template.name.IndexOf("Sirens", StringComparison.OrdinalIgnoreCase) >= 0)
                return false;

            var parent = template.transform.parent;
            if (parent == null)
                return false;

            var templateRect = template.GetComponent<RectTransform>();
            _sirensPrompt = UnityEngine.Object.Instantiate(template, parent);
            _sirensPrompt.name = SirensPromptName;

            StripInputPrompt(_sirensPrompt);

            var rect = _sirensPrompt.GetComponent<RectTransform>();
            if (rect != null && templateRect != null)
            {
                rect.anchorMin = templateRect.anchorMin;
                rect.anchorMax = templateRect.anchorMax;
                rect.pivot = templateRect.pivot;
                rect.sizeDelta = templateRect.sizeDelta;
                rect.anchoredPosition = templateRect.anchoredPosition + SirensAnchoredOffset;
                rect.localScale = templateRect.localScale;
                rect.SetAsLastSibling();
            }

            ForceActivateVisualRoot(_sirensPrompt);
            ForceSirensText(_sirensPrompt);
            _sirensPrompt.SetActive(false);

            MelonLogger.Msg(
                "PoliceVehicleSirenPrompt: created '{0}' above '{1}'.",
                SirensPromptName,
                template.name);
            return true;
        }

        private static void StripInputPrompt(GameObject promptRoot)
        {
            foreach (var behaviour in promptRoot.GetComponents<Behaviour>())
            {
                if (behaviour == null) continue;
                var typeName = behaviour.GetType().Name;
                if (typeName == "InputPrompt" || typeName.EndsWith("InputPrompt", StringComparison.Ordinal))
                    UnityEngine.Object.DestroyImmediate(behaviour);
            }
        }

        private static void ForceActivateVisualRoot(GameObject promptRoot)
        {
            foreach (Transform child in promptRoot.transform)
            {
                if (child.name.IndexOf("Container", StringComparison.OrdinalIgnoreCase) >= 0)
                    child.gameObject.SetActive(true);
            }

            promptRoot.SetActive(true);
        }

        private static void ForceSirensText(GameObject promptRoot)
        {
            var tmps = promptRoot.GetComponentsInChildren<TextMeshProUGUI>(true);
            if (tmps == null || tmps.Length == 0)
            {
                MelonLogger.Warning("PoliceVehicleSirenPrompt: no TextMeshProUGUI under sirens prompt.");
                return;
            }

            TextMeshProUGUI? label = null;
            TextMeshProUGUI? key = null;

            foreach (var tmp in tmps)
            {
                if (tmp == null) continue;
                var t = tmp.text?.Trim() ?? string.Empty;
                var n = tmp.gameObject.name;

                if (t.IndexOf("Headlight", StringComparison.OrdinalIgnoreCase) >= 0
                    || n.IndexOf("Label", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    label = tmp;
                    continue;
                }

                if (t.Length <= 2
                    || n.IndexOf("Key", StringComparison.OrdinalIgnoreCase) >= 0
                    || IsUnderImages(tmp.transform))
                {
                    key ??= tmp;
                }
            }

            if (label == null || key == null)
            {
                TextMeshProUGUI? longest = null;
                TextMeshProUGUI? shortest = null;
                foreach (var tmp in tmps)
                {
                    if (tmp == null) continue;
                    var len = tmp.text?.Length ?? 0;
                    if (longest == null || len > (longest.text?.Length ?? 0)) longest = tmp;
                    if (shortest == null || len < (shortest.text?.Length ?? int.MaxValue)) shortest = tmp;
                }

                label ??= longest;
                key ??= shortest;
            }

            if (label != null)
            {
                label.text = "Sirens";
                label.ForceMeshUpdate();
            }

            if (key != null && !ReferenceEquals(key, label))
            {
                key.text = "R";
                key.ForceMeshUpdate();
            }

            foreach (var tmp in tmps)
            {
                if (tmp == null) continue;
                var t = tmp.text?.Trim() ?? string.Empty;
                if (t.Equals("Headlights", StringComparison.OrdinalIgnoreCase)
                    || t.IndexOf("Headlight", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    tmp.text = "Sirens";
                    tmp.ForceMeshUpdate();
                }
                else if (t.Equals("H", StringComparison.OrdinalIgnoreCase))
                {
                    tmp.text = "R";
                    tmp.ForceMeshUpdate();
                }
            }
        }

        private static bool IsUnderImages(Transform t)
        {
            while (t != null)
            {
                if (t.name.IndexOf("Image", StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
                t = t.parent;
            }

            return false;
        }

        private static GameObject? FindHeadlightsPrompt()
        {
            var driverPrompts = FindDriverPromptsRoot();
            if (driverPrompts == null)
                return null;

            var exact = driverPrompts.Find(HeadlightsPromptName);
            if (exact != null && exact.name.IndexOf("Sirens", StringComparison.OrdinalIgnoreCase) < 0)
                return exact.gameObject;

            foreach (Transform child in driverPrompts)
            {
                if (child.name.IndexOf("Sirens", StringComparison.OrdinalIgnoreCase) >= 0)
                    continue;
                if (child.name.IndexOf("Headlight", StringComparison.OrdinalIgnoreCase) >= 0)
                    return child.gameObject;
            }

            return null;
        }

        private static Transform? FindDriverPromptsRoot()
        {
            if (Singleton<VehicleCanvas>.InstanceExists)
            {
                var canvas = Singleton<VehicleCanvas>.Instance;
                if (canvas.DriverPromptsContainer != null)
                    return canvas.DriverPromptsContainer.transform;

                var fromCanvas = canvas.transform.Find("InputPrompts/DriverPrompts");
                if (fromCanvas != null) return fromCanvas;
            }

            var vehicleCanvasGo = GameObject.Find("VehicleCanvas");
            if (vehicleCanvasGo != null)
            {
                var fromGo = vehicleCanvasGo.transform.Find("InputPrompts/DriverPrompts");
                if (fromGo != null) return fromGo;
            }

            return null;
        }
    }
}
