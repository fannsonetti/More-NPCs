using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MelonLoader;
using MoreNPCs.Persistence;
using MoreNPCs.Utils;
using UnityEngine;

namespace MoreNPCs.Supervisor
{
    /// <summary>
    /// Manages which dealers are assigned to each supervisor. Each supervisor has separate dealers and cash.
    /// </summary>
    public static class SupervisorManager
    {
        public struct AssignableDealer { public string Id; public string DisplayName; }
        public struct CollectTarget { public Vector3 Position; public string DealerId; public float Cash; }

        public static int MaxAssignedDealers =>
            !MoreNPCsPreferences.Registered ? 6 : Math.Max(1, MoreNPCsPreferences.Supervisor_MaxAssignedDealers.Value);

        public static IReadOnlyList<string> GetAssignedDealerIds(string supervisorId)
        {
            if (supervisorId == SupervisorIds.Silas) return MoreNPCsModSave.SilasCartelSupervisor.GetAssignedDealerIds();
            if (supervisorId == SupervisorIds.Dominic) return MoreNPCsModSave.DominicCartelSupervisor.GetAssignedDealerIds();
            return Array.Empty<string>();
        }

        public static float GetStoredCash(string supervisorId)
        {
            if (supervisorId == SupervisorIds.Silas) return MoreNPCsModSave.SilasCartelSupervisor.GetStoredCashTotal();
            if (supervisorId == SupervisorIds.Dominic) return MoreNPCsModSave.DominicCartelSupervisor.GetStoredCashTotal();
            return 0f;
        }

        public static void AssignDealer(string supervisorId, string dealerId)
        {
            if (supervisorId == SupervisorIds.Silas)
            {
                if (MoreNPCsModSave.Instance != null) MoreNPCsModSave.Instance.SilasAssignDealer(dealerId);
                else MoreNPCsModSave.SilasCartelSupervisor.AssignDealerRuntime(dealerId);
            }
            else if (supervisorId == SupervisorIds.Dominic)
            {
                if (MoreNPCsModSave.Instance != null) MoreNPCsModSave.Instance.DominicAssignDealer(dealerId);
                else MoreNPCsModSave.DominicCartelSupervisor.AssignDealerRuntime(dealerId);
            }
        }

        public static void UnassignDealer(string supervisorId, string dealerId)
        {
            if (supervisorId == SupervisorIds.Silas)
            {
                if (MoreNPCsModSave.Instance != null) MoreNPCsModSave.Instance.SilasUnassignDealer(dealerId);
                else MoreNPCsModSave.SilasCartelSupervisor.UnassignDealerRuntime(dealerId);
            }
            else if (supervisorId == SupervisorIds.Dominic)
            {
                if (MoreNPCsModSave.Instance != null) MoreNPCsModSave.Instance.DominicUnassignDealer(dealerId);
                else MoreNPCsModSave.DominicCartelSupervisor.UnassignDealerRuntime(dealerId);
            }
        }

        public static bool IsAssigned(string supervisorId, string dealerId)
        {
            if (supervisorId == SupervisorIds.Silas) return MoreNPCsModSave.SilasCartelSupervisor.IsAssignedStatic(dealerId);
            if (supervisorId == SupervisorIds.Dominic) return MoreNPCsModSave.DominicCartelSupervisor.IsAssignedStatic(dealerId);
            return false;
        }

        public static float TakeAllStoredCash(string supervisorId)
        {
            if (supervisorId == SupervisorIds.Silas) return MoreNPCsModSave.SilasCartelSupervisor.TakeAllStoredCashStatic();
            if (supervisorId == SupervisorIds.Dominic) return MoreNPCsModSave.DominicCartelSupervisor.TakeAllStoredCashStatic();
            return 0f;
        }

        public static void AddToStoredCash(string supervisorId, float amount)
        {
            if (supervisorId == SupervisorIds.Silas) MoreNPCsModSave.SilasCartelSupervisor.AddToStoredCashStatic(amount);
            else if (supervisorId == SupervisorIds.Dominic) MoreNPCsModSave.DominicCartelSupervisor.AddToStoredCashStatic(amount);
        }

        public static List<AssignableDealer> GetAssignableDealers(string supervisorId, bool forceRefresh = false)
        {
            var silasAssigned = GetAssignedDealerIds(SupervisorIds.Silas) ?? Array.Empty<string>();
            var domAssigned = GetAssignedDealerIds(SupervisorIds.Dominic) ?? Array.Empty<string>();
            var assignedToAny = new HashSet<string>(silasAssigned.Concat(domAssigned), StringComparer.OrdinalIgnoreCase);
            var list = new List<AssignableDealer>();
            foreach (var g in GameDealerFinder.GetRecruitedDealersFromGame(forceRefresh))
            {
                if (g != null && !string.IsNullOrEmpty(g.Id) && !assignedToAny.Contains(g.Id))
                    list.Add(new AssignableDealer { Id = g.Id, DisplayName = g.FullName ?? g.Id });
            }
            return list.OrderBy(d => d.DisplayName).ToList();
        }

        public static bool IsUsableDealer(string supervisorId, string dealerId)
        {
            if (string.IsNullOrEmpty(dealerId) || !IsAssigned(supervisorId, dealerId)) return false;
            var gameDealers = GameDealerFinder.GetRecruitedDealersFromGame(false);
            return gameDealers.Any(g => string.Equals(g?.Id, dealerId, StringComparison.OrdinalIgnoreCase));
        }

        public static List<CollectTarget> GetDealersWithCashToCollect(string supervisorId)
        {
            var list = new List<CollectTarget>();
            foreach (var dealerId in GetAssignedDealerIds(supervisorId))
            {
                if (!IsUsableDealer(supervisorId, dealerId)) continue;
                var gameDealer = GameDealerFinder.GetRecruitedDealersFromGame(false)
                    .FirstOrDefault(g => string.Equals(g?.Id, dealerId, StringComparison.OrdinalIgnoreCase));
                if (gameDealer == null) continue;
                float cash = GetDealerCash(gameDealer.DealerComponent);
                var pos = GameDealerFinder.GetDealerPosition(gameDealer);
                if (cash > 0 && pos != Vector3.zero)
                    list.Add(new CollectTarget { Position = pos, DealerId = dealerId, Cash = cash });
            }
            return list;
        }

        public static float TakeDealerCashForSupervisor(string supervisorId, string dealerId)
        {
            if (string.IsNullOrEmpty(dealerId) || !IsUsableDealer(supervisorId, dealerId)) return 0f;
            var gameDealer = GameDealerFinder.GetRecruitedDealersFromGame(false)
                .FirstOrDefault(g => string.Equals(g?.Id, dealerId, StringComparison.OrdinalIgnoreCase));
            if (gameDealer == null) return 0f;
            var comp = gameDealer.DealerComponent;
            if (comp == null) return 0f;
            var cash = GetDealerCash(comp);
            if (cash <= 0) return 0f;
            if (!TryZeroDealerCash(comp)) return 0f;
            return cash;
        }

        private static float GetDealerCash(object comp)
        {
            if (comp == null) return 0f;
            var t = comp.GetType();
            var getCash = t.GetMethod("GetCash", BindingFlags.Public | BindingFlags.Instance);
            if (getCash != null && getCash.GetParameters().Length == 0)
            { try { return Convert.ToSingle(getCash.Invoke(comp, null)); } catch { } }
            var cashProp = t.GetProperty("Cash", BindingFlags.Public | BindingFlags.Instance);
            if (cashProp != null) { try { return Convert.ToSingle(cashProp.GetValue(comp)); } catch { } }
            return 0f;
        }

        private static bool TryZeroDealerCash(object comp)
        {
            if (comp == null) return false;
            var t = comp.GetType();
            var setCash = t.GetMethod("SetCash", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(float) }, null);
            if (setCash != null) { try { setCash.Invoke(comp, new object[] { 0f }); return true; } catch { } }
            var cashProp = t.GetProperty("Cash", BindingFlags.Public | BindingFlags.Instance);
            if (cashProp != null && cashProp.CanWrite) { try { cashProp.SetValue(comp, 0f); return true; } catch { } }
            return false;
        }

        private static int LowStockThreshold =>
            !MoreNPCsPreferences.Registered ? 20 : Math.Max(0, MoreNPCsPreferences.Supervisor_LowStockDrugThreshold.Value);

        public static int GetDealersRunningLowCount(string supervisorId)
        {
            var assigned = GetAssignedDealerIds(supervisorId);
            if (assigned == null || assigned.Count == 0) return 0;
            var dealers = GameDealerFinder.GetRecruitedDealersFromGame(false);
            int count = 0;
            var threshold = LowStockThreshold;
            foreach (var id in assigned)
            {
                if (!IsUsableDealer(supervisorId, id)) continue;
                var dealer = dealers?.FirstOrDefault(g => string.Equals(g?.Id, id, StringComparison.OrdinalIgnoreCase));
                if (dealer == null) continue;
                if (GetDealerTotalDrugCount(dealer) < threshold) count++;
            }
            return count;
        }

        public static string DoDealersNeedAnything(string supervisorId)
        {
            var assigned = GetAssignedDealerIds(supervisorId);
            if (assigned == null || assigned.Count == 0)
                return "Nobody on the books yet. Get someone assigned and I'll handle it.";
            var dealers = GameDealerFinder.GetRecruitedDealersFromGame(false);
            var lowStock = new List<string>();
            var threshold = LowStockThreshold;
            foreach (var id in assigned)
            {
                if (!IsUsableDealer(supervisorId, id)) continue;
                var dealer = dealers?.FirstOrDefault(g => string.Equals(g?.Id, id, StringComparison.OrdinalIgnoreCase));
                if (dealer == null) continue;
                if (GetDealerTotalDrugCount(dealer) < threshold)
                    lowStock.Add(dealer.FullName ?? id);
            }
            if (lowStock.Count > 0)
                return $"{string.Join(", ", lowStock)} need some. Stock is running low.";
            return "All good for now. I'll holler if that changes.";
        }

        private static int GetDealerTotalDrugCount(GameDealerFinder.RecruitedGameDealer dealer)
        {
            var inv = GameDealerFinder.GetDealerInventory(dealer);
            if (inv == null) return 0;
            int total = 0;
            try
            {
                var slotsProp = inv.GetType().GetProperty("ItemSlots", BindingFlags.Public | BindingFlags.Instance);
                var slots = slotsProp?.GetValue(inv) as System.Collections.IEnumerable;
                if (slots == null) return 0;
                var productInstType = FindType("ScheduleOne.Product.ProductItemInstance");
                if (productInstType == null) return 0;
                foreach (var slot in slots)
                {
                    if (slot == null) continue;
                    var instProp = slot.GetType().GetProperty("ItemInstance");
                    var inst = instProp?.GetValue(slot);
                    if (inst == null) continue;
                    if (!productInstType.IsAssignableFrom(inst.GetType())) continue;
                    var qtyProp = slot.GetType().GetProperty("Quantity");
                    var qty = qtyProp != null ? Convert.ToInt32(qtyProp.GetValue(slot)) : 0;
                    var getTotal = inst.GetType().GetMethod("GetTotalAmount", Type.EmptyTypes);
                    total += getTotal != null ? Convert.ToInt32(getTotal.Invoke(inst, null)) : qty;
                }
            }
            catch { }
            return total;
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
