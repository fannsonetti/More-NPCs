using System;
using System.Collections;
using System.Reflection;
using MelonLoader;
using MoreNPCs.NPCs;
using MoreNPCs.Supervisor;
using S1API.Entities;
using UnityEngine;

namespace MoreNPCs.Utils
{
    internal static class NPCIdleLocations
    {
        private const string BankBuildingName = "Hyland Bank";
        private const string SlopShopBuildingName = "Slop Shop";

        // Fallbacks when building lookup fails (observed end positions)
        private static readonly Vector3 BankFallback = new Vector3(143.279f, 4.617f, -96.9804f);
        private static readonly Vector3 SlopShopFallback = new Vector3(-71.051f, -3.907f, 146.1847f);

        public static string GetThomasIdleDialogue() => "When I'm idle, you'll usually find me at Hyland Bank.";

        public static string GetSupervisorIdleDialogue(string supervisorId)
        {
            if (string.Equals(supervisorId, SupervisorIds.Dominic, System.StringComparison.OrdinalIgnoreCase))
                return "When I'm idle, I'm usually posted up at the Slop Shop.";

            if (string.Equals(supervisorId, SupervisorIds.Silas, System.StringComparison.OrdinalIgnoreCase))
                return "When I'm idle, I'm usually at my Uptown spot.";

            return "When I'm idle, I'm usually close by.";
        }

        public static IEnumerator MoveThomasToIdleWhenReady(ThomasAshford npc)
        {
            yield return new WaitForSeconds(1f);
            if (npc == null) yield break;
            yield return EnterBuildingWhenReady(npc, BankBuildingName);
        }

        public static IEnumerator MoveSupervisorToIdleWhenReady(NPC npc, string supervisorId)
        {
            yield return new WaitForSeconds(1f);
            if (npc == null) yield break;
            if (string.Equals(supervisorId, SupervisorIds.Dominic, System.StringComparison.OrdinalIgnoreCase))
                yield return EnterBuildingWhenReady(npc, SlopShopBuildingName);
            else if (npc?.Movement != null)
                npc.Movement.SetDestination(SupervisorConfig.DefaultSpawnPosition);
        }

        public static Vector3 GetThomasIdlePosition(Vector3 fallback)
        {
            return TryGetBuildingEntryPosition(BankBuildingName) ?? BankFallback;
        }

        public static Vector3 GetSupervisorIdlePosition(string supervisorId, Vector3 fallback)
        {
            if (string.Equals(supervisorId, SupervisorIds.Dominic, System.StringComparison.OrdinalIgnoreCase))
                return TryGetBuildingEntryPosition(SlopShopBuildingName) ?? SlopShopFallback;
            return fallback;
        }

        private static IEnumerator EnterBuildingWhenReady(NPC npc, string buildingName)
        {
            if (npc?.Movement == null) yield break;

            var building = TryFindBuilding(buildingName);
            if (building == null)
            {
                var pos = buildingName == BankBuildingName ? BankFallback : SlopShopFallback;
                npc.Movement.SetDestination(pos);
                yield break;
            }

            var entryPoint = GetBuildingEntryPoint(building);
            if (entryPoint != null)
            {
                npc.Movement.SetDestination(entryPoint.position);
                float timeout = 30f;
                while (npc != null && timeout > 0 && Vector3.Distance(npc.Movement.FootPosition, entryPoint.position) > 2f)
                {
                    timeout -= Time.deltaTime;
                    yield return null;
                }
            }

            if (npc != null)
                TryEnterBuilding(npc, building);
        }

        private static object TryFindBuilding(string buildingName)
        {
            var type = FindGameType("ScheduleOne.Map.NPCEnterableBuilding");
            if (type == null) return null;

            var buildings = UnityEngine.Object.FindObjectsOfType(type) as Array;
            if (buildings == null) return null;

            foreach (var b in buildings)
            {
                var name = GetBuildingName(b);
                if (string.Equals(name, buildingName, StringComparison.OrdinalIgnoreCase))
                    return b;
            }
            return null;
        }

        private static string GetBuildingName(object building)
        {
            if (building == null) return null;
            var prop = building.GetType().GetProperty("BuildingName", BindingFlags.Public | BindingFlags.Instance);
            return prop?.GetValue(building) as string;
        }

        private static Vector3? TryGetBuildingEntryPosition(string buildingName)
        {
            var building = TryFindBuilding(buildingName);
            if (building == null) return null;
            var t = GetBuildingEntryPoint(building);
            return t != null ? t.position : (Vector3?)null;
        }

        private static Transform GetBuildingEntryPoint(object building)
        {
            if (building == null) return null;
            var doorsProp = building.GetType().GetProperty("Doors", BindingFlags.Public | BindingFlags.Instance);
            var doors = doorsProp?.GetValue(building) as Array;
            if (doors == null || doors.Length == 0) return null;

            var firstDoor = doors.GetValue(0);
            if (firstDoor == null) return null;
            var accessProp = firstDoor.GetType().GetProperty("AccessPoint", BindingFlags.Public | BindingFlags.Instance);
            return accessProp?.GetValue(firstDoor) as Transform;
        }

        private static void TryEnterBuilding(NPC npc, object building)
        {
            try
            {
                var bt = building.GetType();
                var guidStr = GetGuidString(building, bt);
                if (string.IsNullOrEmpty(guidStr)) return;

                var doorsProp = bt.GetProperty("Doors", BindingFlags.Public | BindingFlags.Instance);
                var doors = doorsProp?.GetValue(building) as Array;
                int doorIndex = (doors != null && doors.Length > 0) ? 0 : 0;

                var npcType = npc.GetType();
                var enterMethod = npcType.GetMethod("EnterBuilding", BindingFlags.Public | BindingFlags.Instance);
                if (enterMethod == null) return;

                var prms = enterMethod.GetParameters();
                object[] args;
                if (prms.Length == 2) // (string guid, int doorIndex)
                    args = new object[] { guidStr, doorIndex };
                else if (prms.Length == 3 && prms[0].ParameterType.Name.Contains("Connection"))
                    args = new object[] { null, guidStr, doorIndex };
                else
                    args = new object[] { guidStr, doorIndex };
                enterMethod.Invoke(npc, args);
            }
            catch (Exception ex) { MelonLogger.Warning($"EnterBuilding failed: {ex.Message}"); }
        }

        private static string GetGuidString(object building, Type bt)
        {
            var guidProp = bt.GetProperty("GUID", BindingFlags.Public | BindingFlags.Instance);
            var guid = guidProp?.GetValue(building);
            if (guid != null) return guid.ToString();
            var bakedProp = bt.GetProperty("BakedGUID", BindingFlags.Public | BindingFlags.Instance);
            return bakedProp?.GetValue(building) as string;
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
