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
        private const float RetryIntervalSeconds = 0.01f; // 10ms when setup pending - load as soon as GameObjects ready
        private const string SourceDoorPath = "Map/Hyland Point/Region_Downtown/Towers/TallOfficeBuilding/StaticDoor";

        private static readonly (string BuildingPath, Vector3 DoorPosition, string BuildingName, string? ContainerName, string Notes)[] Configs =
        {
            ("Map/Hyland Point/Region_Downtown/Towers/ShortOfficeBuilding", new Vector3(57.1401f, 0.495f, 58.8771f), "Small Tower", null, "downtown, ShortOfficeBuilding"),
            ("Map/Hyland Point/Region_Docks/Red Shipping Container", new Vector3(-54.0721f, -2.25f, -77.5079f), "Red Docks Shipping Container", null, "in docks, not a hangout spot, for sleeping"),
            ("Map/Hyland Point/Region_Docks/Red Shipping Container (1)", new Vector3(-57.9253f, -2.25f, -77.7083f), "Red Docks Shipping Container 2", null, "in docks, not a hangout spot, for sleeping"),
            ("Map/Hyland Point/Region_Downtown", new Vector3(-0.7038f, 0.7125f, 72.4522f), "Apartment Building 2", "ApartmentBuilding2", "downtown, custom apartment building 2 container"),
            ("Map/Hyland Point/Region_Westville/Apartment Building", new Vector3(-165.8861f, -0.7f, 94.5868f), "Room 4", "Room4", "in westville, door in Room4 container"),
            ("Map/Hyland Point/Region_Westville/Apartment Building", new Vector3(-165.8861f, 2.4f, 94.5868f), "Room 5", "Room5", "in westville, door in Room5 container"),
            ("Map/Hyland Point/Region_Westville/Apartment Building", new Vector3(-172.8831f, 2.4f, 94.5868f), "Room 6", "Room6", "in westville, door in Room6 container"),
            ("Map/Hyland Point/Region_Westville/ChemicalPlant/Warehouse01", new Vector3(-101.6376f, -3.7f, 89.5072f), "Chemical Plant B", null, "in westville, not a hangout spot, workplace"),
            ("Map/Hyland Point/Region_Westville/Construction Site/LaborerHouse", new Vector3(-128.4608f, -3.5185f, 93.01f), "Tool Shed", null, "in westville, not a hangout spot, workplace"),
            ("Map/Hyland Point/Region_Downtown/Gas Station/gas station/Bodyshop/Interior/Office", new Vector3(6.5439f, 0.2741f, -6.0508f), "BodyShop Office", null, "in downtown, not a hangout spot, workplace but can also function as a house"),
            ("Map/Hyland Point/Cliffs/Manor Cliffs/manor tunnel/Wall/Bomb plant location", new Vector3(166.5998f, 5.9337f, -55.0964f), "Manor Tunnel Hatch", null, "in uptown, under manor, single person house meant for an uptown dealer"),
            ("Map/Hyland Point/Region_Downtown/GroceryStore/grocerystore/Main", new Vector3(12.4867f, 0.2614f, 68.1383f), "Grocery Backdoor", null, "in downtown, not a hangout spot, kinda out of the way"),
            ("Map/Hyland Point/Region_Suburbia/Green House", new Vector3(78.243f, 5.0614f, -94.9608f), "Green House", null, "in suburbia, family home"),
            ("Map/Hyland Point/Region_Suburbia/Long House", new Vector3(65.2065f, 5.135f, -87.5172f), "Long House", null, "in suburbia, family home"),
            ("Map/Hyland Point/Region_Northtown/Storage warehouse/Storage warehouse/Elevator", new Vector3(0.8832f, 0.5959f, 102.711f), "Storage Warehouse Elevator", null, "northtown, storage warehouse elevator door"),
        };

        private readonly HashSet<string> _completed = new HashSet<string>();
        private float _nextAttemptTime;

        public void Update()
        {
            if (_completed.Count >= Configs.Length) return; // all done, stop polling
            if (Time.time < _nextAttemptTime) return;
            _nextAttemptTime = Time.time + RetryIntervalSeconds;

            var sourceDoor = GameObject.Find(SourceDoorPath);
            if (sourceDoor == null) return;

            var staticDoorType = FindGameType("ScheduleOne.Doors.StaticDoor");
            var enterableBuildingType = FindGameType("ScheduleOne.Map.NPCEnterableBuilding");
            var interactableType = FindGameType("ScheduleOne.Interaction.InteractableObject");
            if (staticDoorType == null || enterableBuildingType == null || interactableType == null) return;

            EnsureSourceBuildingIfMissing(sourceDoor, staticDoorType, enterableBuildingType);

            foreach (var (buildingPath, doorPos, buildingName, containerName, notes) in Configs)
            {
                var key = $"{buildingPath}|{buildingName}";
                if (_completed.Contains(key)) continue;

                try
                {
                    if (TrySetupBuilding(sourceDoor, buildingPath, doorPos, buildingName, containerName, staticDoorType, enterableBuildingType, interactableType))
                    {
                        _completed.Add(key);
                        MelonLogger.Msg("BuildingSetup: {0} at {1} [{2}]", buildingName, doorPos, notes);
                    }
                }
                catch (Exception ex)
                {
                    MelonLogger.Warning($"BuildingSetup {buildingName} failed: {ex.Message}");
                }
            }
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
            string buildingName,
            string? containerName,
            Type staticDoorType,
            Type enterableBuildingType,
            Type interactableType)
        {
            var targetBuilding = GameObject.Find(buildingPath);
            if (targetBuilding == null) return false;

            Transform doorParent;
            GameObject buildingRoot;
            if (!string.IsNullOrEmpty(containerName))
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

            var doorName = buildingName == "Small Tower" ? "StaticDoor" : "StaticDoor_" + buildingName.Replace(" ", "_");
            if (!string.IsNullOrEmpty(containerName))
                doorName = "StaticDoor" + containerName.Replace("Room", ""); // StaticDoor4, StaticDoor5, StaticDoor6
            var existingDoor = doorParent.Find(doorName)?.gameObject;
            GameObject targetDoor;
            if (existingDoor != null)
                targetDoor = existingDoor;
            else
            {
                targetDoor = UnityEngine.Object.Instantiate(sourceDoor, doorParent);
                targetDoor.name = doorName;
            }

            targetDoor.transform.SetParent(doorParent, true);
            targetDoor.transform.position = doorPosition;
            targetDoor.transform.rotation = sourceDoor.transform.rotation;
            targetDoor.transform.localScale = sourceDoor.transform.localScale;

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
