using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MelonLoader;
using MoreNPCs.NPCs;
using MoreNPCs.Utils;
using S1API.Entities;
using UnityEngine;

namespace MoreNPCs.Supervisor
{
    /// <summary>Supervisors distribute drugs to their assigned dealers.</summary>
    public static class SupervisorDrugDistributor
    {
        private static readonly HashSet<string> _running = new HashSet<string>();
        public static bool IsRunning => _running.Count > 0;
        public static bool IsRunningFor(string supervisorId) => _running.Contains(supervisorId);

        private struct DealerTarget { public Vector3 Position; public object Inventory; public string Id; }

        public static bool CanDistribute(NPC npc, string supervisorId)
        {
            if (_running.Count > 0 || npc == null) return false;
            var dealers = GetAssignedDealerTargets(supervisorId);
            if (dealers == null || dealers.Count == 0) return false;
            return GetDrugTotalsByType(npc)?.Count > 0;
        }

        public static bool TryStartDistribute(NPC npc, string supervisorId)
        {
            if (_running.Count > 0 || npc == null) return false;
            if (SupervisorDialogue.IsPlayerInMenu) return false;
            var dealers = GetAssignedDealerTargets(supervisorId);
            if (dealers == null || dealers.Count == 0) return false;
            var drugTotals = GetDrugTotalsByType(npc);
            if (drugTotals == null || drugTotals.Count == 0) return false;
            _running.Add(supervisorId);
            SupervisorTextingSetup.SendMessageFrom(npc, "Heads up, making a drop. I'll hit each of the dealers.");
            MelonCoroutines.Start(DistributeCoroutine(npc, supervisorId, dealers, drugTotals));
            return true;
        }

        private static List<DealerTarget> GetAssignedDealerTargets(string supervisorId)
        {
            var list = new List<DealerTarget>();
            var gameDealers = GameDealerFinder.GetRecruitedDealersFromGame();
            foreach (var id in SupervisorManager.GetAssignedDealerIds(supervisorId))
            {
                if (!SupervisorManager.IsUsableDealer(supervisorId, id)) continue;
                var gameDealer = gameDealers?.FirstOrDefault(g => string.Equals(g?.Id, id, StringComparison.OrdinalIgnoreCase));
                if (gameDealer == null) continue;
                var inv = GameDealerFinder.GetDealerInventory(gameDealer);
                if (inv != null)
                    list.Add(new DealerTarget { Position = GameDealerFinder.GetDealerPosition(gameDealer), Inventory = inv, Id = id });
            }
            return list;
        }

        private static Dictionary<string, List<(object slot, object inst, int qty)>> GetDrugTotalsByType(NPC npc)
        {
            var result = new Dictionary<string, List<(object, object, int)>>();
            try
            {
                var invType = FindType("ScheduleOne.NPCs.NPCInventory");
                var inv = npc.gameObject.GetComponent(invType) ?? npc.gameObject.GetComponentInChildren(invType, true);
                if (inv == null) return result;
                var slots = inv.GetType().GetProperty("ItemSlots")?.GetValue(inv) as System.Collections.IEnumerable;
                if (slots == null) return result;
                var productInstanceType = FindType("ScheduleOne.Product.ProductItemInstance");
                var productDefType = FindType("ScheduleOne.Product.ProductDefinition");
                if (productInstanceType == null || productDefType == null) return result;
                foreach (var slot in slots)
                {
                    if (slot == null) continue;
                    var inst = slot.GetType().GetProperty("ItemInstance")?.GetValue(slot);
                    if (inst == null || !productInstanceType.IsAssignableFrom(inst.GetType())) continue;
                    var qtyProp = slot.GetType().GetProperty("Quantity");
                    var qty = qtyProp != null ? (int)Convert.ChangeType(qtyProp.GetValue(slot), typeof(int)) : 0;
                    if (qty <= 0) continue;
                    var def = inst.GetType().GetProperty("Definition")?.GetValue(inst);
                    if (def == null || !productDefType.IsAssignableFrom(def.GetType())) continue;
                    var drugKey = GetDrugTypeFromDefinition(def) ?? GetDefinitionId(def) ?? "unknown";
                    if (!result.TryGetValue(drugKey, out var list)) { list = new List<(object, object, int)>(); result[drugKey] = list; }
                    list.Add((slot, inst, qty));
                }
            }
            catch { }
            return result;
        }

        private static string GetDefinitionId(object def) => def?.GetType().GetProperty("ID")?.GetValue(def) as string;

        private static string GetDrugTypeFromDefinition(object def)
        {
            var drugTypeContainerType = FindType("ScheduleOne.Product.DrugTypeContainer");
            if (drugTypeContainerType == null) return null;
            var drugTypes = def.GetType().GetProperty("DrugTypes")?.GetValue(def) as System.Collections.IEnumerable;
            if (drugTypes == null) return null;
            foreach (var dt in drugTypes)
            {
                if (dt == null) continue;
                var drugTypeField = drugTypeContainerType.GetField("DrugType") ?? drugTypeContainerType.GetField("drugType", BindingFlags.NonPublic | BindingFlags.Instance);
                var et = drugTypeField?.GetValue(dt);
                var s = et?.ToString();
                if (!string.IsNullOrEmpty(s)) return s;
            }
            return null;
        }

        private static IEnumerator DistributeCoroutine(NPC npc, string supervisorId, List<DealerTarget> dealers, Dictionary<string, List<(object slot, object inst, int qty)>> drugTotals)
        {
            try
            {
                int count = dealers.Count;
                if (count == 0) yield break;
                var perDealer = new Dictionary<string, int>();
                foreach (var kv in drugTotals) perDealer[kv.Key] = kv.Value.Sum(x => x.qty) / count;
                const float ArrivalRadius = 2.5f, WalkTimeout = 120f;
                for (int d = 0; d < dealers.Count; d++)
                {
                    var target = dealers[d];
                    npc.Movement.SetDestination(target.Position);
                    float timeout = WalkTimeout;
                    while (Vector3.Distance(npc.Movement.FootPosition, target.Position) > ArrivalRadius && timeout > 0)
                    {
                        if (SupervisorDialogue.IsPlayerInMenu) yield break;
                        timeout -= Time.deltaTime;
                        yield return null;
                    }
                    if (SupervisorDialogue.IsPlayerInMenu) yield break;
                    yield return new WaitForSeconds(0.5f);
                    if (count > 1)
                        SupervisorTextingSetup.SendMessageFrom(npc, d == count - 1 ? "Last drop done." : $"Dropped off. {count - d - 1} more to go.");
                    TransferToDealerInventory(target.Inventory, drugTotals, perDealer, d == count - 1);
                }
                SupervisorTextingSetup.SendMessageFrom(npc, "Finished the run. Everyone's stocked.");
            }
            finally { _running.Remove(supervisorId); }
        }

        private static void TransferToDealerInventory(object dealerInv, Dictionary<string, List<(object slot, object inst, int qty)>> drugTotals, Dictionary<string, int> perDealer, bool isLastDealer)
        {
            try
            {
                if (dealerInv == null) return;
                var itemInstanceType = FindType("ScheduleOne.ItemFramework.ItemInstance");
                var insertMethod = dealerInv.GetType().GetMethod("InsertItem", new[] { itemInstanceType, typeof(bool) });
                if (insertMethod == null) return;
                foreach (var kv in drugTotals)
                {
                    var slots = kv.Value;
                    var totalQty = slots.Sum(s => { var q = s.slot?.GetType().GetProperty("Quantity")?.GetValue(s.slot); return q != null ? (int)Convert.ChangeType(q, typeof(int)) : 0; });
                    var give = isLastDealer ? totalQty : perDealer.GetValueOrDefault(kv.Key, 0);
                    if (give <= 0 || slots.Count == 0 || slots[0].inst == null) continue;
                    var copy = slots[0].inst.GetType().GetMethod("GetCopy", new[] { typeof(int) })?.Invoke(slots[0].inst, new object[] { give });
                    if (copy == null) continue;
                    insertMethod.Invoke(dealerInv, new[] { copy, true });
                    int remaining = give;
                    foreach (var (slot, _, _) in slots)
                    {
                        if (remaining <= 0) break;
                        var qtyProp = slot?.GetType().GetProperty("Quantity");
                        int slotQty = qtyProp != null ? (int)Convert.ChangeType(qtyProp.GetValue(slot), typeof(int)) : 0;
                        if (slotQty <= 0) continue;
                        int take = Math.Min(remaining, slotQty);
                        slot?.GetType().GetMethod("ChangeQuantity", new[] { typeof(int), typeof(bool) })?.Invoke(slot, new object[] { -take, true });
                        remaining -= take;
                    }
                }
            }
            catch { }
        }

        private static Type FindType(string fullName)
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
