using System;
using System.Reflection;
using S1API.Entities;
using S1API.Money;
using UnityEngine;

namespace MoreNPCs.Utils
{
    /// <summary>
    /// Adds cash to the local player using S1API.Money API.
    /// </summary>
    public static class PlayerCashHelper
    {
        public static bool TrySpendCash(float amount)
        {
            if (amount <= 0) return true;
            try
            {
                var balance = S1API.Money.Money.GetCashBalance();
                if (balance < amount) return false;
                S1API.Money.Money.ChangeCashBalance(-amount, visualizeChange: true, playCashSound: true);
                return true;
            }
            catch { }
            return false;
        }

        public static float GetCashBalance()
        {
            try { return S1API.Money.Money.GetCashBalance(); }
            catch { return 0f; }
        }

        public static bool TryAddCash(float amount)
        {
            if (amount <= 0) return true;
            try
            {
                S1API.Money.Money.ChangeCashBalance(amount, visualizeChange: true, playCashSound: true);
                return true;
            }
            catch { }
            var player = Player.Local;
            if (player?.Transform == null) return false;
            // Fallbacks if S1API.Money unavailable
            var go = player.Transform.gameObject;
            foreach (var comp in go.GetComponents<Component>())
            {
                if (comp == null) continue;
                var t = comp.GetType();
                var add = t.GetMethod("AddCash", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(float) }, null);
                if (add != null) { try { add.Invoke(comp, new object[] { amount }); return true; } catch { } }
                var change = t.GetMethod("ChangeCash", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(float) }, null);
                if (change != null) { try { change.Invoke(comp, new object[] { amount }); return true; } catch { } }
                var cashProp = t.GetProperty("Cash", BindingFlags.Public | BindingFlags.Instance);
                if (cashProp != null && cashProp.CanRead && cashProp.CanWrite)
                { try { var cur = Convert.ToSingle(cashProp.GetValue(comp)); cashProp.SetValue(comp, cur + amount); return true; } catch { } }
            }
            var cashManager = FindType("ScheduleOne.Economy.CashManager");
            if (cashManager != null)
            {
                var inst = cashManager.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static)?.GetValue(null);
                if (inst != null)
                {
                    var add = cashManager.GetMethod("AddCash", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(float) }, null);
                    if (add != null) { try { add.Invoke(inst, new object[] { amount }); return true; } catch { } }
                }
            }
            return false;
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
