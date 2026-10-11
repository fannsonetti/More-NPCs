using System;
using System.Collections.Generic;
using System.Reflection;
using MelonLoader;
using UnityEngine;

namespace MoreNPCs.Utils
{
    /// <summary>
    /// Adds NPCEnterableBuilding and StaticDoor to custom buildings. Uses TallOfficeBuilding/StaticDoor as template.
    /// </summary>
    internal sealed class BuildingSetup
    {
        private const float RetryIntervalSeconds = 0.05f;
        private const string SourceDoorPath = "Map/Hyland Point/Region_Downtown/Towers/TallOfficeBuilding/StaticDoor";

        /// <summary>World euler for copied TallOffice StaticDoor when parented; template is ~0,180,0.</summary>
        private static readonly Vector3 DefaultDoorWorldEuler = new Vector3(0f, 180f, 0f);

        /// <summary>Red Docks shipping container static doors (both).</summary>
        private static readonly Vector3 RedDocksContainerDoorEuler = new Vector3(0f, 330f, 0f);

        /// <summary>Doors that align correctly at identity world rotation (see Apt 2 + Grocery backdoor).</summary>
        private static readonly Vector3 DoorWorldEulerIdentity = Vector3.zero;

        /// <summary>
        /// Apt 3–4: same as Short Tower / Westville room doors (~180° Y). Apt 2 uses <see cref="DoorWorldEulerIdentity"/>.
        /// </summary>
        private static readonly Vector3 DowntownApartmentDoorWorldEuler = DefaultDoorWorldEuler;

        private static readonly (string BuildingPath, Vector3 DoorPosition, Vector3 DoorWorldEuler, string BuildingName, string? ContainerName, string? DoorSubRoot, string? ExplicitDoorName, string Notes)[] Configs =
        {
            ("Map/Hyland Point/Region_Downtown/Towers/ShortOfficeBuilding", new Vector3(57.1401f, 0.495f, 58.8771f), DefaultDoorWorldEuler, "Small Tower", null, null, null, "downtown, ShortOfficeBuilding"),
            ("Map/Hyland Point/Region_Docks/Red Shipping Container", new Vector3(-54.0721f, -2.25f, -77.5079f), RedDocksContainerDoorEuler, "Red Docks Shipping Container", null, null, null, "in docks, not a hangout spot, for sleeping"),
            ("Map/Hyland Point/Region_Docks/Red Shipping Container (1)", new Vector3(-57.9253f, -2.25f, -77.7083f), RedDocksContainerDoorEuler, "Red Docks Shipping Container 2", null, null, null, "in docks, not a hangout spot, for sleeping"),
            ("Map/Hyland Point/Region_Downtown", new Vector3(-0.7038f, 0.7125f, 72.4522f), DoorWorldEulerIdentity, "Apartment Building 2", "ApartmentBuilding2", null, null, "downtown, custom apartment building 2 container"),
            ("Map/Hyland Point/Region_Downtown", new Vector3(-7.6395f, 0.425f, 57.4574f), DowntownApartmentDoorWorldEuler, "Apartment Building 3", "ApartmentBuilding3", null, null, "downtown, custom apartment building 3"),
            ("Map/Hyland Point/Region_Downtown", new Vector3(-10.2952f, 0.4616f, 66.7971f), DowntownApartmentDoorWorldEuler, "Apartment Building 4", "ApartmentBuilding4", null, null, "downtown, custom apartment building 4"),
            ("Map/Hyland Point/Region_Downtown", new Vector3(136.0216f, -0.6856f, 34.2468f), new Vector3(0f, 330f, 0f), "PPGrave", "PPGrave", null, null, "downtown, memorial — not a hangout"),
            ("Map/Hyland Point/Region_Westville/Apartment Building", new Vector3(-165.8861f, -0.7f, 94.5868f), DefaultDoorWorldEuler, "Room 4", "Room4", null, null, "in westville, door in Room4 container"),
            ("Map/Hyland Point/Region_Westville/Apartment Building", new Vector3(-165.8861f, 2.4f, 94.5868f), DefaultDoorWorldEuler, "Room 5", "Room5", null, null, "in westville, door in Room5 container"),
            ("Map/Hyland Point/Region_Westville/Apartment Building", new Vector3(-172.8831f, 2.4f, 94.5868f), DefaultDoorWorldEuler, "Room 6", "Room6", null, null, "in westville, door in Room6 container"),
            ("Map/Hyland Point/Region_Westville/ChemicalPlant/Warehouse01", new Vector3(-101.6376f, -3.7f, 89.5072f), new Vector3(0f, 270f, 0f), "Chemical Plant B", null, null, null, "in westville, not a hangout spot, workplace"),
            ("Map/Hyland Point/Region_Westville/Construction Site/LaborerHouse", new Vector3(-128.4608f, -3.5185f, 93.01f), new Vector3(0f, 290f, 0f), "Tool Shed", null, null, null, "in westville, not a hangout spot, workplace"),
            ("Map/Hyland Point/Region_Downtown/Gas Station/gas station", new Vector3(10.6963f, 0.3515f, -6.1312f), new Vector3(0f, 160f, 0f), "Gasmart Freezer", null, null, null, "downtown gas station freezer — dock-region dealer home"),
            ("Map/Hyland Point/Region_Downtown/Gas Station/gas station/Bodyshop/Interior/Office", new Vector3(6.5439f, 0.2741f, -6.0508f), DefaultDoorWorldEuler, "BodyShop Office", null, null, null, "in downtown, not a hangout spot, workplace but can also function as a house"),
            ("Map/Hyland Point/Cliffs/Manor Cliffs/manor tunnel/Wall/Bomb plant location", new Vector3(166.5998f, 5.6828f, -55.2601f), DoorWorldEulerIdentity, "Manor Tunnel Hatch", null, null, null, "in uptown, under manor, single person house meant for an uptown dealer"),
            ("Map/Hyland Point/Region_Downtown/GroceryStore/grocerystore/Main", new Vector3(12.4867f, 0.2614f, 68.1383f), DoorWorldEulerIdentity, "Grocery Backdoor", null, null, null, "in downtown, not a hangout spot, kinda out of the way"),
            ("Map/Hyland Point/Region_Suburbia/Green House", new Vector3(78.243f, 4.8614f, -94.9608f), DefaultDoorWorldEuler, "Green House", null, null, null, "in suburbia, family home"),
            ("Map/Hyland Point/Region_Suburbia/Long House", new Vector3(65.2065f, 4.935f, -87.5172f), DefaultDoorWorldEuler, "Long House", null, null, null, "in suburbia, family home"),
            ("Map/Hyland Point/Region_Suburbia/Long House", new Vector3(68.1922f, 4.935f, -83.8701f), new Vector3(0f, 90f, 0f), "Long House Side Door", null, "Sidedoor", "StaticDoor_Long_House_Side_Door", "suburbia, Long House side entrance"),
            ("Map/Hyland Point/Region_Northtown/Storage warehouse/Storage warehouse/Elevator", new Vector3(0.7603f, 0.2653f, 102.6636f), new Vector3(0f, 270f, 0f), "Storage Warehouse Elevator", null, null, null, "northtown, storage warehouse elevator door (world pos)"),
        };

        private readonly HashSet<string> _completed = new HashSet<string>();
        private float _nextAttemptTime;
        private bool _allComplete;

        private static GameObject? _cachedSourceDoor;
        private static Type? _cachedStaticDoorType;
        private static Type? _cachedEnterableBuildingType;
        private static Type? _cachedInteractableType;

        public void Update()
        {
            if (_allComplete) return;
            if (_completed.Count >= Configs.Length)
            {
                _allComplete = true;
                return;
            }

            if (Time.time < _nextAttemptTime) return;
            _nextAttemptTime = Time.time + RetryIntervalSeconds;

            var sourceDoor = _cachedSourceDoor != null ? _cachedSourceDoor : GameObject.Find(SourceDoorPath);
            if (sourceDoor == null) return;
            _cachedSourceDoor = sourceDoor;

            var staticDoorType = _cachedStaticDoorType ?? FindGameType("ScheduleOne.Doors.StaticDoor");
            var enterableBuildingType = _cachedEnterableBuildingType ?? FindGameType("ScheduleOne.Map.NPCEnterableBuilding");
            var interactableType = _cachedInteractableType ?? FindGameType("ScheduleOne.Interaction.InteractableObject");
            if (staticDoorType == null || enterableBuildingType == null || interactableType == null) return;
            _cachedStaticDoorType = staticDoorType;
            _cachedEnterableBuildingType = enterableBuildingType;
            _cachedInteractableType = interactableType;

            EnsureSourceBuildingIfMissing(sourceDoor, staticDoorType, enterableBuildingType);

            foreach (var (buildingPath, doorPos, doorWorldEuler, buildingName, containerName, doorSubRoot, explicitDoorName, notes) in Configs)
            {
                var key = $"{buildingPath}|{buildingName}";
                if (_completed.Contains(key)) continue;

                try
                {
                    if (TrySetupBuilding(sourceDoor, buildingPath, doorPos, doorWorldEuler, buildingName, containerName, doorSubRoot, explicitDoorName, staticDoorType, enterableBuildingType, interactableType))
                    {
                        _completed.Add(key);
                        MelonLogger.Msg("BuildingSetup: {0} at {1} rot {2} [{3}]", buildingName, doorPos, doorWorldEuler, notes);
                    }
                }
                catch (Exception ex)
                {
                    MelonLogger.Warning($"BuildingSetup {buildingName} failed: {ex.Message}");
                }
            }

            if (_completed.Count >= Configs.Length)
                _allComplete = true;
        }

        private static void EnsureSourceBuildingIfMissing(GameObject sourceDoor, Type staticDoorType, Type enterableBuildingType)
        {
            var sourceDoorComponent = sourceDoor.GetComponent(staticDoorType);
            if (sourceDoorComponent == null) return;

            var currentBuilding = GetMemberValue(sourceDoorComponent, "Building");
            if (currentBuilding != null) return;

            var sourceBuildingRoot = sourceDoor.transform.parent?.gameObject;
            if (sourceBuildingRoot == null) return;

            var sourceBuilding = EnsureEnterableBuilding(sourceBuildingRoot, enterableBuildingType, "Tall Tower", allowMultiple: false);
            if (sourceBuilding == null) return;

            SetMemberValue(sourceDoorComponent, "Building", sourceBuilding);
            sourceBuilding.GetType().GetMethod("GetDoors", BindingFlags.Public | BindingFlags.Instance)?.Invoke(sourceBuilding, null);
        }

        private static bool TrySetupBuilding(
            GameObject sourceDoor,
            string buildingPath,
            Vector3 doorPosition,
            Vector3 doorWorldEuler,
            string buildingName,
            string? containerName,
            string? doorSubRoot,
            string? explicitDoorName,
            Type staticDoorType,
            Type enterableBuildingType,
            Type interactableType)
        {
            var targetBuilding = GameObject.Find(buildingPath);
            if (targetBuilding == null) return false;

            Transform doorParent;
            GameObject buildingRoot;
            if (!string.IsNullOrEmpty(doorSubRoot))
            {
                var subRoot = targetBuilding.transform.Find(doorSubRoot)?.gameObject;
                if (subRoot == null)
                {
                    subRoot = new GameObject(doorSubRoot);
                    subRoot.transform.SetParent(targetBuilding.transform, false);
                    subRoot.transform.localPosition = Vector3.zero;
                    subRoot.transform.localRotation = Quaternion.identity;
                    subRoot.transform.localScale = Vector3.one;
                }
                doorParent = subRoot.transform;
                buildingRoot = subRoot;
            }
            else if (!string.IsNullOrEmpty(containerName))
            {
                var container = targetBuilding.transform.Find(containerName)?.gameObject;
                if (container == null)
                {
                    container = new GameObject(containerName);
                    container.transform.SetParent(targetBuilding.transform, false);
                    container.transform.localPosition = Vector3.zero;
                    container.transform.localRotation = Quaternion.identity;
                    container.transform.localScale = Vector3.one;
                }
                doorParent = container.transform;
                buildingRoot = container;
            }
            else
            {
                doorParent = targetBuilding.transform;
                buildingRoot = targetBuilding;
            }

            var doorName = !string.IsNullOrEmpty(explicitDoorName)
                ? explicitDoorName
                : buildingName == "Small Tower"
                    ? "StaticDoor"
                    : "StaticDoor_" + buildingName.Replace(" ", "_");
            if (!string.IsNullOrEmpty(containerName) && string.IsNullOrEmpty(explicitDoorName))
                doorName = "StaticDoor" + containerName.Replace("Room", "");

            var existingDoor = doorParent.Find(doorName)?.gameObject;
            GameObject targetDoor;
            if (existingDoor != null)
                targetDoor = existingDoor;
            else
            {
                targetDoor = UnityEngine.Object.Instantiate(sourceDoor);
                targetDoor.name = doorName;
            }

            // Move + world rotation first, then parent under building (order matters for correct alignment).
            targetDoor.transform.SetParent(null, true);
            targetDoor.transform.position = doorPosition;
            targetDoor.transform.rotation = Quaternion.Euler(doorWorldEuler);
            targetDoor.transform.localScale = sourceDoor.transform.localScale;
            targetDoor.transform.SetParent(doorParent, true);

            var buildingComponent = EnsureEnterableBuilding(buildingRoot, enterableBuildingType, buildingName, allowMultiple: true);
            var targetDoorComponent = targetDoor.GetComponent(staticDoorType);
            if (buildingComponent == null || targetDoorComponent == null) return false;

            var intObj = GetInteractableObject(targetDoor, interactableType);
            if (intObj == null) return false;

            SetMemberValue(targetDoorComponent, "IntObj", intObj);
            SetMemberValue(targetDoorComponent, "Building", buildingComponent);
            SetMemberValue(targetDoorComponent, "Usable", true);
            SetMemberValue(targetDoorComponent, "CanKnock", true);

            ConfigureInteractable(intObj);
            EnsureIntObjDetectable(sourceDoor, targetDoor, intObj, interactableType);

            var getDoors = buildingComponent.GetType().GetMethod("GetDoors", BindingFlags.Public | BindingFlags.Instance);
            getDoors?.Invoke(buildingComponent, null);

            return true;
        }

        private static Component EnsureEnterableBuilding(GameObject buildingRoot, Type enterableBuildingType, string buildingName, bool allowMultiple)
        {
            buildingRoot.SetActive(true);

            if (allowMultiple)
            {
                var existing = buildingRoot.GetComponents(enterableBuildingType);
                foreach (var c in existing)
                {
                    var n = GetMemberValue(c, "BuildingName") as string;
                    if (string.Equals(n, buildingName, StringComparison.OrdinalIgnoreCase))
                        return c as Component;
                }
            }
            else
            {
                var component = buildingRoot.GetComponent(enterableBuildingType);
                if (component != null) return component as Component;
            }

            var newComponent = buildingRoot.AddComponent(enterableBuildingType) as Component;
            if (newComponent == null) return null;

            var guid = Guid.NewGuid();
            var guidStr = guid.ToString();
            SetMemberValue(newComponent, "BuildingName", buildingName);
            SetMemberValue(newComponent, "BakedGUID", guidStr);
            newComponent.GetType().GetMethod("SetGUID", BindingFlags.Public | BindingFlags.Instance)?.Invoke(newComponent, new object[] { guid });

            if (newComponent is Behaviour b)
                b.enabled = true;

            return newComponent;
        }

        private static Component GetInteractableObject(GameObject targetDoor, Type interactableType)
        {
            var intObjTransform = targetDoor.transform.Find("IntObj");
            if (intObjTransform != null)
            {
                var direct = intObjTransform.GetComponent(interactableType);
                if (direct != null) return direct;
            }
            return targetDoor.GetComponentInChildren(interactableType, true) as Component;
        }

        private static void EnsureIntObjDetectable(GameObject sourceDoor, GameObject targetDoor, Component intObj, Type interactableType)
        {
            if (intObj == null) return;
            var go = intObj.gameObject;
            go.SetActive(true);
            if (intObj is Behaviour b) b.enabled = true;

            var sourceIntObj = GetInteractableObject(sourceDoor, interactableType);
            if (sourceIntObj != null && sourceIntObj.gameObject != null)
            {
                int layer = sourceIntObj.gameObject.layer;
                go.layer = layer;
                foreach (Transform t in intObj.transform.GetComponentsInChildren<Transform>(true))
                    t.gameObject.layer = layer;
            }

            foreach (var c in intObj.GetComponentsInChildren<Collider>(true))
                c.enabled = true;

            if (intObj.GetComponentInChildren<Collider>(true) == null)
            {
                var box = go.AddComponent<BoxCollider>();
                box.isTrigger = true;
                box.size = new Vector3(1.5f, 2f, 0.5f);
                box.center = Vector3.zero;
            }
        }

        private static void ConfigureInteractable(Component interactable)
        {
            if (interactable == null) return;
            var type = interactable.GetType();
            type.GetMethod("SetMessage", BindingFlags.Public | BindingFlags.Instance)?.Invoke(interactable, new object[] { "Knock" });

            var interactionType = type.GetNestedType("EInteractionType", BindingFlags.Public);
            if (interactionType != null)
            {
                var keyPress = Enum.Parse(interactionType, "Key_Press");
                type.GetMethod("SetInteractionType", BindingFlags.Public | BindingFlags.Instance)?.Invoke(interactable, new[] { keyPress });
            }

            var interactableState = type.GetNestedType("EInteractableState", BindingFlags.Public);
            if (interactableState != null)
            {
                var defaultState = Enum.Parse(interactableState, "Default");
                type.GetMethod("SetInteractableState", BindingFlags.Public | BindingFlags.Instance)?.Invoke(interactable, new[] { defaultState });
            }
        }

        private static object GetMemberValue(object instance, string name)
        {
            var type = instance.GetType();
            return type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(instance)
                ?? type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(instance);
        }

        private static void SetMemberValue(object instance, string name, object value)
        {
            var type = instance.GetType();
            var field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null) { field.SetValue(instance, value); return; }
            var property = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (property?.CanWrite == true) property.SetValue(instance, value);
        }

        private static Type FindGameType(string fullName)
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
