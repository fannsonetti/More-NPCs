using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using ScheduleOne.Economy;
using ScheduleOne.UI.Handover;

namespace MoreNPCs.Patches
{
    /// <summary>Shared reflection for <see cref="HandoverScreen"/> (customer, sample mode, item enumerables) used by P.P. Hyland patches.</summary>
    internal static class PPHylandHandoverScreenHelper
    {
        private static Type? _cachedScreenType;
        private static FieldInfo? _cachedCustomerField;
        private static PropertyInfo? _cachedCustomerProp;

        internal static void EnsureCustomerCache(HandoverScreen screen)
        {
            var t = screen.GetType();
            if (_cachedScreenType == t) return;

            _cachedScreenType = t;
            _cachedCustomerField = null;
            _cachedCustomerProp = null;

            foreach (var f in t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (typeof(Customer).IsAssignableFrom(f.FieldType))
                {
                    _cachedCustomerField = f;
                    break;
                }
            }

            if (_cachedCustomerField == null)
            {
                foreach (var p in t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (!p.CanRead || p.GetIndexParameters().Length > 0) continue;
                    if (typeof(Customer).IsAssignableFrom(p.PropertyType))
                    {
                        _cachedCustomerProp = p;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// True only for free-sample handover — avoids false positives during normal deals (e.g. any field whose
        /// <see cref="object.ToString"/> happened to contain "Sample").
        /// </summary>
        internal static bool IsSampleMode(HandoverScreen screen)
        {
            var t = screen.GetType();
            const BindingFlags inst = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            foreach (var f in t.GetFields(inst))
            {
                if (f.Name.IndexOf("Sample", StringComparison.OrdinalIgnoreCase) < 0
                    && f.Name.IndexOf("FreeSample", StringComparison.OrdinalIgnoreCase) < 0
                    && f.Name.IndexOf("IsSample", StringComparison.OrdinalIgnoreCase) < 0)
                    continue;

                if (f.FieldType == typeof(bool))
                {
                    try
                    {
                        if (f.GetValue(screen) is true)
                            return true;
                    }
                    catch { /* ignore */ }
                }
            }

            foreach (var p in t.GetProperties(inst))
            {
                if (!p.CanRead || p.GetIndexParameters().Length > 0) continue;
                if (p.Name.IndexOf("Sample", StringComparison.OrdinalIgnoreCase) < 0
                    && p.Name.IndexOf("FreeSample", StringComparison.OrdinalIgnoreCase) < 0
                    && p.Name.IndexOf("IsSample", StringComparison.OrdinalIgnoreCase) < 0)
                    continue;

                if (p.PropertyType == typeof(bool))
                {
                    try
                    {
                        if (p.GetValue(screen, null) is true)
                            return true;
                    }
                    catch { /* ignore */ }
                }
            }

            // Handover mode is often an enum on a field named "Mode" / "OrderType" — use enum member names, not ToString().
            foreach (var f in t.GetFields(inst))
            {
                if (!f.FieldType.IsEnum) continue;
                try
                {
                    var v = f.GetValue(screen);
                    if (v == null) continue;
                    var en = Enum.GetName(f.FieldType, v);
                    if (en != null && en.IndexOf("Sample", StringComparison.OrdinalIgnoreCase) >= 0)
                        return true;
                }
                catch { /* ignore */ }
            }

            foreach (var p in t.GetProperties(inst))
            {
                if (!p.CanRead || p.GetIndexParameters().Length > 0) continue;
                if (!p.PropertyType.IsEnum) continue;
                try
                {
                    var v = p.GetValue(screen, null);
                    if (v == null) continue;
                    var en = Enum.GetName(p.PropertyType, v);
                    if (en != null && en.IndexOf("Sample", StringComparison.OrdinalIgnoreCase) >= 0)
                        return true;
                }
                catch { /* ignore */ }
            }

            return false;
        }

        internal static Customer? TryGetCustomer(HandoverScreen screen)
        {
            if (_cachedCustomerField != null)
            {
                var v = _cachedCustomerField.GetValue(screen);
                return v as Customer;
            }

            if (_cachedCustomerProp != null)
            {
                try
                {
                    return _cachedCustomerProp.GetValue(screen, null) as Customer;
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        /// <summary>Prefab id string (e.g. <c>jennifer_rivera</c>, <c>pp_hyland</c>) for handover UI copy.</summary>
        internal static string? TryGetCustomerNpcId(Customer? customer)
        {
            if (customer == null) return null;
            try
            {
                if (TryReadNpcIdFromObject(customer, out var id))
                    return id;

                var t = customer.GetType();
                foreach (var f in t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    object? v;
                    try { v = f.GetValue(customer); }
                    catch { continue; }
                    if (v == null) continue;
                    if (TryReadNpcIdFromObject(v, out id))
                        return id;
                }

                foreach (var p in t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (!p.CanRead || p.GetIndexParameters().Length > 0) continue;
                    object? v;
                    try { v = p.GetValue(customer, null); }
                    catch { continue; }
                    if (v == null) continue;
                    if (TryReadNpcIdFromObject(v, out id))
                        return id;
                }
            }
            catch
            {
                /* ignore */
            }

            return null;
        }

        private static bool TryReadNpcIdFromObject(object o, out string? id)
        {
            id = null;
            var t = o.GetType();
            foreach (var name in new[] { "ID", "Id", "NpcId", "NPCID", "npcID" })
            {
                var prop = t.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var fld = t.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var s = prop?.GetValue(o) as string ?? fld?.GetValue(o) as string;
                if (!string.IsNullOrEmpty(s))
                {
                    id = s;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Handover screens may expose several <see cref="IEnumerable"/> fields (slots vs offer list, etc.).
        /// The first match can undercount at the exact requirement; pick the collection with the highest meth-unit total.
        /// </summary>
        internal static IEnumerable? TryGetCustomerItemsEnumerable(HandoverScreen screen)
        {
            var candidates = CollectCustomerItemEnumerables(screen);
            if (candidates.Count == 0) return null;
            if (candidates.Count == 1) return candidates[0];

            IEnumerable? best = null;
            var bestUnits = -1f;
            foreach (var e in candidates)
            {
                var u = PpHylandSampleDifficulty.GetTotalMethSampleUnits(e);
                if (u > bestUnits)
                {
                    bestUnits = u;
                    best = e;
                }
            }

            return best;
        }

        /// <summary>
        /// All non-null item references from every handover list on the screen, deduped by reference (meth totals were
        /// wrong when product sat in one list while another enumerable stayed empty).
        /// </summary>
        internal static List<object> GetDistinctHandoverItems(HandoverScreen screen)
        {
            var result = new List<object>();
            var seen = new HashSet<object>(ReferenceEqualityComparerImpl.Instance);
            foreach (var e in CollectCustomerItemEnumerables(screen))
            {
                if (e == null) continue;
                foreach (var it in e)
                {
                    if (it == null) continue;
                    if (seen.Add(it))
                        result.Add(it);
                }
            }

            return result;
        }

        private sealed class ReferenceEqualityComparerImpl : IEqualityComparer<object>
        {
            internal static readonly ReferenceEqualityComparerImpl Instance = new ReferenceEqualityComparerImpl();
            public bool Equals(object x, object y) => ReferenceEquals(x, y);
            public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
        }

        private static List<IEnumerable> CollectCustomerItemEnumerables(HandoverScreen screen)
        {
            var result = new List<IEnumerable>();

            void TryAdd(IEnumerable? e)
            {
                if (e == null) return;
                if (result.Contains(e)) return;
                result.Add(e);
            }

            var t = screen.GetType();
            const BindingFlags inst = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            foreach (var namePart in new[] { "Item", "Slot", "Product", "Offer", "Stack", "Handover" })
            {
                foreach (var f in t.GetFields(inst))
                {
                    if (f.Name.IndexOf(namePart, StringComparison.OrdinalIgnoreCase) < 0) continue;
                    if (!IsItemEnumerableField(f.FieldType)) continue;
                    TryAdd(f.GetValue(screen) as IEnumerable);
                }

                foreach (var p in t.GetProperties(inst))
                {
                    if (!p.CanRead || p.GetIndexParameters().Length > 0) continue;
                    if (p.Name.IndexOf(namePart, StringComparison.OrdinalIgnoreCase) < 0) continue;
                    if (!IsItemEnumerableField(p.PropertyType)) continue;
                    try
                    {
                        TryAdd(p.GetValue(screen, null) as IEnumerable);
                    }
                    catch
                    {
                        /* ignore */
                    }
                }
            }

            foreach (var f in t.GetFields(inst))
            {
                if (!IsItemEnumerableField(f.FieldType)) continue;
                TryAdd(f.GetValue(screen) as IEnumerable);
            }

            foreach (var p in t.GetProperties(inst))
            {
                if (!p.CanRead || p.GetIndexParameters().Length > 0) continue;
                if (!IsItemEnumerableField(p.PropertyType)) continue;
                try
                {
                    TryAdd(p.GetValue(screen, null) as IEnumerable);
                }
                catch
                {
                    /* ignore */
                }
            }

            return result;
        }

        private static bool IsItemEnumerableField(Type fieldType)
        {
            if (!typeof(IEnumerable).IsAssignableFrom(fieldType) || fieldType == typeof(string))
                return false;
            if (fieldType.IsArray)
                return true;
            if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                return false;
            return true;
        }
    }
}
