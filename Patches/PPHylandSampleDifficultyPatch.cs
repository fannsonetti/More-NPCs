using System;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using HarmonyLib;
using MelonLoader;
using ScheduleOne.Economy;

namespace MoreNPCs.Patches
{
    /// <summary>
    /// P.P. Hyland only: meth-only samples must reach <see cref="RequiredMethUnits"/> meth units from <c>ProductItemInstance.AppliedPackaging</c> (baggie 1, jar 5, brick 20 per stack qty), then × <see cref="SampleSuccessMultiplier"/>.
    /// Uses <c>object</c> for items (no <c>ItemInstance</c> type) so the mod does not need a <c>ScheduleOne.Core</c> reference for <c>BaseItemInstance</c>.
    /// </summary>
    internal static class PpHylandSampleDifficulty
    {
        internal const string NpcId = "pp_hyland";

        /// <summary>Applied to meth-only sample success after quantity gate (lower = harder).</summary>
        internal const float SampleSuccessMultiplier = 0.2f;

        /// <summary>Sum of meth-unit contributions from packaged meth (see <see cref="GetTotalMethSampleUnits"/>).</summary>
        internal const int RequiredMethUnits = 20;

        /// <summary>Allow tiny float noise, but never let 19 units pass as 20.</summary>
        private const float QuantityCompareEpsilon = 0.001f;

        internal const float MethUnitsPerBaggie = 1f;
        internal const float MethUnitsPerJar = 5f;
        internal const float MethUnitsPerBrick = 20f;

        internal static bool IsPpHylandCustomer(Customer customer)
        {
            if (customer == null) return false;
            try
            {
                if (ObjectHasMatchingNpcId(customer))
                    return true;

                var t = customer.GetType();
                foreach (var f in t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    object? v;
                    try { v = f.GetValue(customer); }
                    catch { continue; }
                    if (v == null) continue;
                    if (ObjectHasMatchingNpcId(v))
                        return true;
                }

                foreach (var p in t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (!p.CanRead || p.GetIndexParameters().Length > 0) continue;
                    object? v;
                    try { v = p.GetValue(customer, null); }
                    catch { continue; }
                    if (v == null) continue;
                    if (ObjectHasMatchingNpcId(v))
                        return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>True if <paramref name="o"/> (NPC, wrapper, etc.) exposes our mod NPC id string.</summary>
        private static bool ObjectHasMatchingNpcId(object o)
        {
            var t = o.GetType();
            foreach (var name in new[] { "ID", "Id", "NpcId", "NPCID", "npcID" })
            {
                var prop = t.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var fld = t.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var s = prop?.GetValue(o) as string ?? fld?.GetValue(o) as string;
                if (string.Equals(s, NpcId, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        /// <summary>True if handover has at least one item and every item is methamphetamine.</summary>
        internal static bool SampleContainsOnlyMethamphetamine(object? itemsObj)
        {
            if (itemsObj is not IEnumerable enumerable) return false;

            var any = false;
            foreach (var item in enumerable)
            {
                if (item == null) continue;
                any = true;
                if (!ItemIsMethamphetamine(item))
                    return false;
            }

            return any;
        }

        /// <summary>
        /// Meth units: <see cref="MethUnitsPerBaggie"/> / <see cref="MethUnitsPerJar"/> / <see cref="MethUnitsPerBrick"/> per stack line, from <c>AppliedPackaging.ID</c> on <c>ProductItemInstance</c>.
        /// </summary>
        internal static float GetTotalMethSampleUnits(object? itemsObj)
        {
            if (itemsObj is not IEnumerable enumerable) return 0f;

            float sum = 0f;
            foreach (var item in enumerable)
            {
                if (item == null) continue;
                if (!ItemIsMethamphetamine(item)) continue;
                sum += GetMethUnitsForPackagedItem(item);
            }

            return sum;
        }

        internal static bool MeetsHylandMethQuantityRequirement(object? itemsObj)
        {
            return GetTotalMethSampleUnits(itemsObj) + QuantityCompareEpsilon >= RequiredMethUnits;
        }

        /// <summary>Uses <c>ProductItemInstance.AppliedPackaging</c> (<see cref="PackagingDefinition"/>) via reflection — no compile-time link to ScheduleOne.Core item types.</summary>
        private static float GetMethUnitsForPackagedItem(object item)
        {
            var q = GetItemQuantity(item);
            var applied = TryGetAppliedPackaging(item);
            if (applied == null)
                return q * MethUnitsPerBaggie;

            var id = GetPackagingDefinitionId(applied);
            var per = GetMethUnitsPerPackagingId(id);
            if (per <= 0f)
                per = MethUnitsPerBaggie;

            return per * q;
        }

        private static object? TryGetAppliedPackaging(object item)
        {
            for (var t = item.GetType(); t != null; t = t.BaseType)
            {
                var p = t.GetProperty("AppliedPackaging", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (p == null) continue;
                var v = p.GetValue(item);
                if (v != null) return v;
            }

            return null;
        }

        private static string? GetPackagingDefinitionId(object packagingDef)
        {
            var t = packagingDef.GetType();
            var id = t.GetProperty("ID", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(packagingDef) as string
                     ?? t.GetField("ID", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(packagingDef) as string;
            if (!string.IsNullOrEmpty(id)) return id;

            var name = t.GetProperty("Name", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(packagingDef)?.ToString();
            return string.IsNullOrEmpty(name) ? null : name;
        }

        private static float GetMethUnitsPerPackagingId(string? id)
        {
            if (string.IsNullOrEmpty(id)) return 0f;
            id = id.Trim().ToLowerInvariant();
            if (id == "brick") return MethUnitsPerBrick;
            if (id == "jar") return MethUnitsPerJar;
            if (id == "baggie") return MethUnitsPerBaggie;
            if (id.IndexOf("brick", StringComparison.OrdinalIgnoreCase) >= 0) return MethUnitsPerBrick;
            if (id.IndexOf("jar", StringComparison.OrdinalIgnoreCase) >= 0) return MethUnitsPerJar;
            if (id.IndexOf("baggie", StringComparison.OrdinalIgnoreCase) >= 0) return MethUnitsPerBaggie;

            return 0f;
        }

        private static object? GetDefinition(object item)
        {
            var t = item.GetType();
            foreach (var propName in new[] { "Definition", "ProductDefinition", "Asset", "ReferencedProduct", "Product", "Data", "ItemDefinition" })
            {
                var p = t.GetProperty(propName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var def = p?.GetValue(item);
                if (def != null) return def;
            }

            return null;
        }

        private static float GetItemQuantity(object item)
        {
            foreach (var name in new[] { "Quantity", "Amount", "StackQuantity", "StackCount", "Count" })
            {
                var p = item.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var f = item.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var v = p?.GetValue(item) ?? f?.GetValue(item);
                switch (v)
                {
                    case float fl: return fl;
                    case double d: return (float)d;
                    case int i: return i;
                    case uint ui: return ui;
                    case long l: return l;
                    case byte b: return b;
                }
            }

            return 1f;
        }

        private static bool ItemIsMethamphetamine(object item)
        {
            foreach (var root in EnumerateItemAndDefinitions(item))
            {
                if (root == null) continue;
                if (DrugTypeObjectIndicatesMeth(TryGetDrugTypeObject(root)))
                    return true;
                if (ReflectedStringsSuggestMethProduct(root))
                    return true;
                if (ReflectedVeryHighTierWithMethAcrossFields(root))
                    return true;
            }

            return false;
        }

        private static IEnumerable<object?> EnumerateItemAndDefinitions(object item)
        {
            yield return item;
            var def = GetDefinition(item);
            if (def != null)
                yield return def;
        }

        private static object? TryGetDrugTypeObject(object host)
        {
            var t = host.GetType();
            foreach (var name in new[] { "DrugType", "EDrugType", "eDrugType", "Drug", "ProductDrugType" })
            {
                var p = t.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var f = t.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var v = p?.GetValue(host) ?? f?.GetValue(host);
                if (v != null) return v;
            }

            foreach (var m in t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if ((m.Name != "GetDrugType" && m.Name != "GetDrug") || m.GetParameters().Length != 0
                    || m.ReturnType == typeof(void))
                    continue;
                try
                {
                    var v = m.Invoke(host, null);
                    if (v != null) return v;
                }
                catch
                {
                    /* ignore */
                }
            }

            return null;
        }

        private static bool DrugTypeObjectIndicatesMeth(object? dt)
        {
            if (dt == null) return false;

            var s = dt.ToString() ?? "";
            if (s.IndexOf("Methamphetamine", StringComparison.OrdinalIgnoreCase) >= 0) return true;
            if (string.Equals(s, "Meth", StringComparison.OrdinalIgnoreCase)) return true;
            if (Regex.IsMatch(s, @"\bMeth\b", RegexOptions.IgnoreCase)) return true;

            if (dt.GetType().IsEnum)
            {
                var n = Enum.GetName(dt.GetType(), dt);
                if (!string.IsNullOrEmpty(n))
                {
                    if (n.IndexOf("Methamphetamine", StringComparison.OrdinalIgnoreCase) >= 0) return true;
                    if (Regex.IsMatch(n, @"\bMeth\b", RegexOptions.IgnoreCase)) return true;
                }
            }

            return false;
        }

        /// <summary>When <see cref="DrugType"/> is missing, infer from string fields (single field may contain both quality and drug).</summary>
        private static bool ReflectedStringsSuggestMethProduct(object o)
        {
            const BindingFlags inst = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var t = o.GetType();
            foreach (var p in t.GetProperties(inst))
            {
                if (!p.CanRead || p.GetIndexParameters().Length > 0) continue;
                if (p.PropertyType != typeof(string) && p.PropertyType != typeof(object)) continue;
                string? s;
                try { s = p.GetValue(o, null)?.ToString(); }
                catch { continue; }
                if (ProductStringImpliesMeth(s)) return true;
            }

            foreach (var f in t.GetFields(inst))
            {
                if (f.FieldType != typeof(string) && f.FieldType != typeof(object)) continue;
                string? s;
                try { s = f.GetValue(o)?.ToString(); }
                catch { continue; }
                if (ProductStringImpliesMeth(s)) return true;
            }

            return false;
        }

        private static bool ProductStringImpliesMeth(string? s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            if (s.IndexOf("Methamphetamine", StringComparison.OrdinalIgnoreCase) >= 0) return true;
            if (Regex.IsMatch(s, @"\bMeth\b", RegexOptions.IgnoreCase)) return true;
            // UI "Heavenly" quality vs code VeryHigh — match both when paired with meth tokens.
            if (s.IndexOf("HeavenlyMeth", StringComparison.OrdinalIgnoreCase) >= 0) return true;
            if (s.IndexOf("VeryHighMeth", StringComparison.OrdinalIgnoreCase) >= 0) return true;
            if (s.IndexOf("Heavenly", StringComparison.OrdinalIgnoreCase) >= 0
                && s.IndexOf("Meth", StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
            if (s.IndexOf("VeryHigh", StringComparison.OrdinalIgnoreCase) >= 0
                && s.IndexOf("Meth", StringComparison.OrdinalIgnoreCase) >= 0)
                return true;

            return false;
        }

        /// <summary>Quality tier is often <c>VeryHigh</c> on one member and drug name on another (Heavenly in UI). Enums stringify as <c>VeryHigh</c>, not only string fields.</summary>
        private static bool ReflectedVeryHighTierWithMethAcrossFields(object o)
        {
            var tier = false;
            var meth = false;
            const BindingFlags inst = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var t = o.GetType();
            foreach (var p in t.GetProperties(inst))
            {
                if (!p.CanRead || p.GetIndexParameters().Length > 0) continue;
                object? v;
                try { v = p.GetValue(o, null); }
                catch { continue; }
                if (v == null) continue;
                var s = v as string ?? v.ToString();
                if (string.IsNullOrEmpty(s)) continue;
                if (s.IndexOf("VeryHigh", StringComparison.OrdinalIgnoreCase) >= 0
                    || s.IndexOf("Heavenly", StringComparison.OrdinalIgnoreCase) >= 0)
                    tier = true;
                if (DrugStringLooksLikeMeth(s))
                    meth = true;
            }

            foreach (var f in t.GetFields(inst))
            {
                object? v;
                try { v = f.GetValue(o); }
                catch { continue; }
                if (v == null) continue;
                var s = v as string ?? v.ToString();
                if (string.IsNullOrEmpty(s)) continue;
                if (s.IndexOf("VeryHigh", StringComparison.OrdinalIgnoreCase) >= 0
                    || s.IndexOf("Heavenly", StringComparison.OrdinalIgnoreCase) >= 0)
                    tier = true;
                if (DrugStringLooksLikeMeth(s))
                    meth = true;
            }

            return tier && meth;
        }

        private static bool DrugStringLooksLikeMeth(string? s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            if (s.IndexOf("Methamphetamine", StringComparison.OrdinalIgnoreCase) >= 0) return true;
            return Regex.IsMatch(s, @"\bMeth\b", RegexOptions.IgnoreCase);
        }
    }

    [HarmonyPatch]
    internal static class PPHylandGetSampleSuccessPatch
    {
        private static MethodBase? _target;

        /// <summary>True when the game uses <c>GetSampleSuccess(float, IEnumerable)</c> instead of items-first.</summary>
        private static bool _swapSampleSuccessArgs;

        private static bool Prepare()
        {
            _swapSampleSuccessArgs = false;
            _target = null;
            for (var ty = typeof(Customer); ty != null; ty = ty.BaseType)
            {
                foreach (var m in ty.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                {
                    if (m.Name != "GetSampleSuccess") continue;
                    var ps = m.GetParameters();
                    if (ps.Length != 2) continue;
                    var a = ps[0].ParameterType;
                    var b = ps[1].ParameterType;
                    if (typeof(IEnumerable).IsAssignableFrom(a) && b == typeof(float))
                    {
                        _target = m;
                        return true;
                    }

                    if (a == typeof(float) && typeof(IEnumerable).IsAssignableFrom(b))
                    {
                        _target = m;
                        _swapSampleSuccessArgs = true;
                        return true;
                    }
                }
            }

            MelonLogger.Warning("[MoreNPCs] PPHyland: Customer.GetSampleSuccess(IEnumerable,float) not found (game update?).");
            return false;
        }

        private static MethodBase TargetMethod() => _target!;

        /// <summary>Original parameters in order, then return value as <c>ref float __result</c>.</summary>
        private static void Postfix(Customer __instance, object __0, object __1, ref float __result)
        {
            if (!PpHylandSampleDifficulty.IsPpHylandCustomer(__instance)) return;

            object? items = _swapSampleSuccessArgs ? __1 : __0;

            if (items is not IEnumerable enumerable)
            {
                MelonLogger.Warning("[MoreNPCs] PPHyland: GetSampleSuccess args did not resolve to IEnumerable; skipping.");
                return;
            }

            if (!PpHylandSampleDifficulty.SampleContainsOnlyMethamphetamine(enumerable))
            {
                __result = 0f;
                return;
            }

            if (!PpHylandSampleDifficulty.MeetsHylandMethQuantityRequirement(enumerable))
            {
                __result = 0f;
                return;
            }

            __result *= PpHylandSampleDifficulty.SampleSuccessMultiplier;
        }
    }
}
