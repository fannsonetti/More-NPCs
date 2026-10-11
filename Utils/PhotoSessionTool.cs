using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using MelonLoader;
using S1API.Entities;
using UnityEngine;

namespace MoreNPCs.Utils
{
    /// <summary>Photo grid arranger used from the F6 debug menu Photo tab.</summary>
    internal static class PhotoSessionTool
    {
        private static readonly string[] RegionOptions =
        {
            "All", "Northtown", "Westville", "Downtown", "Docks", "Suburbia", "Uptown"
        };

        private static string _originXText = "-50";
        private static string _originYText = "-2.935";
        private static string _originZText = "170";
        private static string _columnsText = "6";
        private static string _columnSpacingText = "1.5";
        private static string _rowSpacingText = "1.5";
        private static string _faceYawText = "180";
        private static int _regionIndex;

        private static string _statusMessage = string.Empty;

        private sealed class PinnedNpc
        {
            public NPC Npc = null!;
            public Vector3 Position;
            public float Yaw;
        }

        private static readonly List<PinnedNpc> PinnedNpcs = new List<PinnedNpc>();
        private static float _nextPinMaintainTime;

        public static void Update()
        {
            MaintainPinnedNpcs();
        }

        internal static void DrawDebugTab(ref string statusMessage)
        {
            LoadDraftFromPreferences();
            GUILayout.Label("Grid origin (world coordinates — center of grid)");
            _originXText = LabeledTextField("X", _originXText);
            _originYText = LabeledTextField("Y", _originYText);
            _originZText = LabeledTextField("Z", _originZText);

            if (GUILayout.Button("Use my position as origin"))
            {
                CapturePlayerOrigin();
                SetStatus(ref statusMessage, _statusMessage);
            }

            GUILayout.Space(8f);
            GUILayout.Label("Layout");
            _columnsText = LabeledTextField("Columns", _columnsText);
            _columnSpacingText = LabeledTextField("Column spacing (m)", _columnSpacingText);
            _rowSpacingText = LabeledTextField("Row spacing (m)", _rowSpacingText);
            _faceYawText = LabeledTextField("Face yaw (degrees)", _faceYawText);

            GUILayout.Space(4f);
            GUILayout.Label("Region filter");
            _regionIndex = GUILayout.SelectionGrid(_regionIndex, RegionOptions, 2);

            GUILayout.Space(8f);
            if (GUILayout.Button("Save settings to config"))
            {
                if (TryReadSettings(out _, out _, out _, out _, out _, out _, out _, out _))
                {
                    SaveDraftToPreferences();
                    SetStatus(ref statusMessage, "Settings saved to MelonPreferences.");
                }
            }

            if (GUILayout.Button("Arrange NPCs"))
            {
                TryArrangeFromDraft();
                SetStatus(ref statusMessage, _statusMessage);
            }

            if (GUILayout.Button("Release NPCs"))
            {
                ClearPinnedNpcs();
                SetStatus(ref statusMessage, "Released photo grid NPCs back to normal behavior.");
            }
        }

        private static void SetStatus(ref string statusMessage, string message)
        {
            _statusMessage = message;
            statusMessage = message;
        }

        private static string LabeledTextField(string label, string value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(140f));
            var next = GUILayout.TextField(value ?? string.Empty);
            GUILayout.EndHorizontal();
            return next;
        }

        private static void LoadDraftFromPreferences()
        {
            if (!MoreNPCsPreferences.Registered)
                return;

            _originXText = MoreNPCsPreferences.PhotoSession_OriginX.Value.ToString(CultureInfo.InvariantCulture);
            _originYText = MoreNPCsPreferences.PhotoSession_OriginY.Value.ToString(CultureInfo.InvariantCulture);
            _originZText = MoreNPCsPreferences.PhotoSession_OriginZ.Value.ToString(CultureInfo.InvariantCulture);
            _columnsText = MoreNPCsPreferences.PhotoSession_Columns.Value.ToString(CultureInfo.InvariantCulture);
            _columnSpacingText = MoreNPCsPreferences.PhotoSession_Spacing.Value.ToString(CultureInfo.InvariantCulture);
            _rowSpacingText = MoreNPCsPreferences.PhotoSession_RowSpacing.Value.ToString(CultureInfo.InvariantCulture);
            _faceYawText = MoreNPCsPreferences.PhotoSession_FaceYaw.Value.ToString(CultureInfo.InvariantCulture);
            _regionIndex = IndexForRegion(MoreNPCsPreferences.PhotoSession_RegionFilter.Value);
        }

        private static void SaveDraftToPreferences()
        {
            if (!MoreNPCsPreferences.Registered) return;
            if (!TryReadSettings(out var origin, out var columns, out var columnSpacing, out var rowSpacing,
                    out var faceYaw, out var regionFilter, out _, out _))
                return;

            MoreNPCsPreferences.PhotoSession_OriginX.Value = origin.x;
            MoreNPCsPreferences.PhotoSession_OriginY.Value = origin.y;
            MoreNPCsPreferences.PhotoSession_OriginZ.Value = origin.z;
            MoreNPCsPreferences.PhotoSession_Columns.Value = columns;
            MoreNPCsPreferences.PhotoSession_Spacing.Value = columnSpacing;
            MoreNPCsPreferences.PhotoSession_RowSpacing.Value = rowSpacing;
            MoreNPCsPreferences.PhotoSession_FaceYaw.Value = faceYaw;
            MoreNPCsPreferences.PhotoSession_RegionFilter.Value = regionFilter;
            MelonPreferences.Save();
        }

        private static void CapturePlayerOrigin()
        {
            var player = Player.Local;
            if (player?.Transform == null)
            {
                _statusMessage = "Player not found — load into the world first.";
                return;
            }

            var pos = player.Transform.position;
            _originXText = pos.x.ToString("F2", CultureInfo.InvariantCulture);
            _originYText = pos.y.ToString("F2", CultureInfo.InvariantCulture);
            _originZText = pos.z.ToString("F2", CultureInfo.InvariantCulture);
            _statusMessage = $"Origin set to your position ({pos.x:F1}, {pos.y:F1}, {pos.z:F1}).";
        }

        private static void TryArrangeFromDraft()
        {
            if (!NPC.CustomNpcsReady)
            {
                _statusMessage = "Custom NPCs not ready yet.";
                return;
            }

            if (!TryReadSettings(out var origin, out var columns, out var columnSpacing, out var rowSpacing,
                    out var faceYaw, out var regionFilter, out _, out _))
                return;

            SaveDraftToPreferences();

            try
            {
                var count = ArrangeGrid(origin, columns, columnSpacing, rowSpacing, faceYaw, regionFilter);
                _statusMessage =
                    $"Arranged {count} NPCs at ({origin.x:F1}, {origin.y:F1}, {origin.z:F1}) — {columns} columns, region '{regionFilter}'.";
                MelonLogger.Msg(_statusMessage);
            }
            catch (Exception ex)
            {
                _statusMessage = $"Failed: {ex.Message}";
                MelonLogger.Warning($"PhotoSession failed: {ex.Message}");
            }
        }

        private static bool TryReadSettings(
            out Vector3 origin,
            out int columns,
            out float columnSpacing,
            out float rowSpacing,
            out float faceYaw,
            out string regionFilter,
            out string errorField,
            out string errorMessage)
        {
            origin = default;
            columns = 6;
            columnSpacing = 1.5f;
            rowSpacing = 1.5f;
            faceYaw = 180f;
            regionFilter = "All";
            errorField = string.Empty;
            errorMessage = string.Empty;

            if (!TryParseFloat(_originXText, "Origin X", out var originX, out errorMessage)) { errorField = "Origin X"; _statusMessage = errorMessage; return false; }
            if (!TryParseFloat(_originYText, "Origin Y", out var originY, out errorMessage)) { errorField = "Origin Y"; _statusMessage = errorMessage; return false; }
            if (!TryParseFloat(_originZText, "Origin Z", out var originZ, out errorMessage)) { errorField = "Origin Z"; _statusMessage = errorMessage; return false; }
            if (!TryParseInt(_columnsText, "Columns", 1, 100, out columns, out errorMessage)) { errorField = "Columns"; _statusMessage = errorMessage; return false; }
            if (!TryParseFloat(_columnSpacingText, "Column spacing", out columnSpacing, out errorMessage)) { errorField = "Column spacing"; _statusMessage = errorMessage; return false; }
            if (!TryParseFloat(_rowSpacingText, "Row spacing", out rowSpacing, out errorMessage)) { errorField = "Row spacing"; _statusMessage = errorMessage; return false; }
            if (!TryParseFloat(_faceYawText, "Face yaw", out faceYaw, out errorMessage)) { errorField = "Face yaw"; _statusMessage = errorMessage; return false; }

            columnSpacing = Math.Max(0.5f, columnSpacing);
            rowSpacing = Math.Max(0.5f, rowSpacing);
            origin = new Vector3(originX, originY, originZ);
            regionFilter = RegionOptions[Mathf.Clamp(_regionIndex, 0, RegionOptions.Length - 1)];
            return true;
        }

        private static bool TryParseFloat(string text, string label, out float value, out string error)
        {
            if (!float.TryParse(text?.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out value)
                && !float.TryParse(text?.Trim(), NumberStyles.Float, CultureInfo.CurrentCulture, out value))
            {
                error = $"{label} must be a number.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        private static bool TryParseInt(string text, string label, int min, int max, out int value, out string error)
        {
            if (!int.TryParse(text?.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out value)
                && !int.TryParse(text?.Trim(), NumberStyles.Integer, CultureInfo.CurrentCulture, out value))
            {
                error = $"{label} must be a whole number.";
                return false;
            }

            if (value < min || value > max)
            {
                error = $"{label} must be between {min} and {max}.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        private static int IndexForRegion(string? filter)
        {
            if (string.IsNullOrWhiteSpace(filter)) return 0;
            for (int i = 0; i < RegionOptions.Length; i++)
            {
                if (RegionOptions[i].Equals(filter.Trim(), StringComparison.OrdinalIgnoreCase))
                    return i;
            }

            return 0;
        }

        private static int ArrangeGrid(
            Vector3 origin,
            int columns,
            float columnSpacing,
            float rowSpacing,
            float faceYaw,
            string regionFilter)
        {
            var all = NPC.All;
            if (all == null || all.Count == 0)
            {
                _statusMessage = "No NPCs found.";
                return 0;
            }

            var targets = new List<NPC>();
            foreach (var npc in all)
            {
                if (npc == null || !IsMoreNpcsNpc(npc)) continue;
                if (!MatchesRegionFilter(npc, regionFilter)) continue;
                targets.Add(npc);
            }

            targets.Sort((a, b) =>
            {
                int byHeight = GetNpcSortHeight(a).CompareTo(GetNpcSortHeight(b));
                if (byHeight != 0) return byHeight;
                return string.Compare(a.ID, b.ID, StringComparison.OrdinalIgnoreCase);
            });

            PinnedNpcs.Clear();

            if (targets.Count == 0)
            {
                _statusMessage = $"No mod NPCs matched region filter '{regionFilter}'.";
                return 0;
            }

            int rows = (targets.Count + columns - 1) / columns;
            float gridWidth = (columns - 1) * columnSpacing;
            float gridDepth = (rows - 1) * rowSpacing;
            var centerOffset = new Vector3(-gridWidth * 0.5f, 0f, gridDepth * 0.5f);

            for (int i = 0; i < targets.Count; i++)
            {
                int col = i % columns;
                int row = i / columns;
                var cellOffset = new Vector3(col * columnSpacing, 0f, -row * rowSpacing);
                WarpNpc(targets[i], origin + centerOffset + cellOffset, faceYaw);
                PinnedNpcs.Add(new PinnedNpc
                {
                    Npc = targets[i],
                    Position = origin + centerOffset + cellOffset,
                    Yaw = faceYaw
                });
            }

            _nextPinMaintainTime = 0f;
            return targets.Count;
        }

        public static void MaintainPinnedNpcs()
        {
            if (PinnedNpcs.Count == 0) return;
            if (Time.time < _nextPinMaintainTime) return;
            _nextPinMaintainTime = Time.time + 0.25f;

            for (int i = PinnedNpcs.Count - 1; i >= 0; i--)
            {
                var pin = PinnedNpcs[i];
                if (pin.Npc?.Transform == null)
                {
                    PinnedNpcs.RemoveAt(i);
                    continue;
                }

                ApplyPhotoSessionHold(pin.Npc, pin.Position, pin.Yaw);
            }
        }

        private static void ClearPinnedNpcs() => PinnedNpcs.Clear();

        private static float GetNpcSortHeight(NPC npc)
        {
            try
            {
                var appearance = npc.GetType().GetProperty("Appearance")?.GetValue(npc);
                var heightProp = appearance?.GetType().GetProperty("Height");
                if (heightProp != null)
                    return Convert.ToSingle(heightProp.GetValue(appearance));
            }
            catch { }

            return 1f;
        }

        private static bool IsMoreNpcsNpc(NPC npc)
        {
            var asmName = npc.GetType().Assembly.GetName().Name;
            return asmName != null && asmName.StartsWith("MoreNPCs", StringComparison.OrdinalIgnoreCase);
        }

        private static bool MatchesRegionFilter(NPC npc, string filter)
        {
            if (filter.Equals("All", StringComparison.OrdinalIgnoreCase)) return true;
            return string.Equals(npc.Region.ToString(), filter, StringComparison.OrdinalIgnoreCase);
        }

        private static void WarpNpc(NPC npc, Vector3 position, float yawDegrees) =>
            ApplyPhotoSessionHold(npc, position, yawDegrees);

        private static void ApplyPhotoSessionHold(NPC npc, Vector3 position, float yawDegrees)
        {
            TryDisableSchedule(npc);
            TryStopMovement(npc);
            TryDisableNavigation(npc);
            TryDisableImpostor(npc);
            TryShowNpcVisuals(npc);

            var transform = npc.Transform;
            if (transform != null)
            {
                transform.position = position;
                transform.rotation = Quaternion.Euler(0f, yawDegrees, 0f);
            }

            try
            {
                var movement = npc.Movement;
                if (movement == null) return;
                var warp = movement.GetType().GetMethod("Warp", BindingFlags.Public | BindingFlags.Instance);
                if (warp != null)
                {
                    var parameters = warp.GetParameters();
                    if (parameters.Length == 1 && parameters[0].ParameterType == typeof(Vector3))
                        warp.Invoke(movement, new object[] { position });
                    else if (parameters.Length == 2)
                        warp.Invoke(movement, new object[] { position, yawDegrees });
                }
                else
                {
                    movement.SetDestination(position);
                }
            }
            catch { }
        }

        private static void TryDisableNavigation(NPC npc)
        {
            try
            {
                var root = npc.Transform?.gameObject;
                if (root == null) return;

                var agentType = FindType("UnityEngine.AI.NavMeshAgent") ?? typeof(UnityEngine.AI.NavMeshAgent);
                foreach (var agent in root.GetComponentsInChildren(agentType, true))
                {
                    if (agent == null) continue;
                    var enabledProp = agent.GetType().GetProperty("enabled");
                    enabledProp?.SetValue(agent, false);
                    agent.GetType().GetMethod("ResetPath")?.Invoke(agent, null);
                }
            }
            catch { }
        }

        private static void TryDisableImpostor(NPC npc)
        {
            try
            {
                var root = npc.Transform?.gameObject;
                if (root == null) return;

                foreach (var typeName in new[]
                         {
                             "ScheduleOne.AvatarFramework.Avatar",
                             "ScheduleOne.AvatarFramework.Impostors.AvatarImpostor"
                         })
                {
                    var type = FindType(typeName);
                    if (type == null) continue;

                    foreach (var component in root.GetComponentsInChildren(type, true))
                    {
                        if (component == null) continue;
                        var disable = component.GetType().GetMethod("DisableImpostor", BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
                        disable?.Invoke(component, null);
                    }
                }
            }
            catch { }
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

        private static void TryDisableSchedule(NPC npc)
        {
            try
            {
                var schedule = npc.Schedule;
                if (schedule == null) return;

                var type = schedule.GetType();
                var disable = type.GetMethod("Disable", BindingFlags.Public | BindingFlags.Instance);
                if (disable != null && disable.GetParameters().Length == 0)
                {
                    disable.Invoke(schedule, null);
                    return;
                }

                var enabledProp = type.GetProperty("Enabled", BindingFlags.Public | BindingFlags.Instance);
                if (enabledProp?.CanWrite == true)
                    enabledProp.SetValue(schedule, false);
            }
            catch { }
        }

        private static void TryStopMovement(NPC npc)
        {
            try
            {
                var movement = npc.Movement;
                if (movement == null) return;

                var type = movement.GetType();
                foreach (var methodName in new[] { "Stop", "Halt", "ClearDestination" })
                {
                    var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
                    if (method != null)
                    {
                        method.Invoke(movement, null);
                        return;
                    }
                }
            }
            catch { }
        }

        private static void TryShowNpcVisuals(NPC npc)
        {
            try
            {
                var root = npc.Transform?.gameObject;
                if (root == null) return;

                if (!root.activeSelf) root.SetActive(true);

                foreach (var name in new[] { "BodyContainer", "Avatar" })
                    SetActiveDeep(root.transform, name, true);

                try
                {
                    var appearance = npc.GetType().GetProperty("Appearance")?.GetValue(npc);
                    appearance?.GetType().GetMethod("Build")?.Invoke(appearance, null);
                }
                catch { }
            }
            catch { }
        }

        private static void SetActiveDeep(Transform root, string childName, bool active)
        {
            foreach (var t in root.GetComponentsInChildren<Transform>(true))
            {
                if (t.name.Equals(childName, StringComparison.OrdinalIgnoreCase))
                    t.gameObject.SetActive(active);
            }
        }
    }
}
