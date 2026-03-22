using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MelonLoader;
using UnityEngine;

namespace MoreNPCs.Utils
{
    /// <summary>
    /// Finds recruited dealers via runtime component inspection.
    /// Supports: (1) CharacterClasses.[name] that extend Dealer, (2) ScheduleOne.Economy.Dealer components (Molly, mod dealers).
    /// Scans root GameObjects only. Results are cached to reduce lag.
    /// </summary>
    public static class GameDealerFinder
    {
        private static List<RecruitedGameDealer> _cache;
        private static float _cacheTime = -999f;
        private const float CacheSeconds = 2f;
        private static Type _dealerBaseType;

        /// <summary>
        /// Recruited dealer info from game components.
        /// </summary>
        public class RecruitedGameDealer
        {
            public string Id { get; set; }
            public string FullName { get; set; }
            public GameObject GameObject { get; set; }
            public object DealerComponent { get; set; }
        }

        /// <summary>
        /// Finds all recruited dealers. Uses cache unless forceRefresh is true.
        /// </summary>
        public static List<RecruitedGameDealer> GetRecruitedDealersFromGame(bool forceRefresh = false)
        {
            if (!forceRefresh && _cache != null && (UnityEngine.Time.realtimeSinceStartup - _cacheTime) < CacheSeconds)
                return _cache;

            var list = ScanForRecruitedDealers();
            _cache = list;
            _cacheTime = UnityEngine.Time.realtimeSinceStartup;
            return list;
        }

        /// <summary>Clears the cache so the next call will rescan.</summary>
        public static void InvalidateCache()
        {
            _cache = null;
        }

        private static Type FindCharacterClassType(string goName)
        {
            var typeName = $"ScheduleOne.NPCs.CharacterClasses.{goName}";
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                var t = asm.GetType(typeName);
                if (t != null) return t;
            }
            return null;
        }

        private static Type GetDealerBaseType()
        {
            if (_dealerBaseType != null) return _dealerBaseType;
            _dealerBaseType = FindType("ScheduleOne.Economy.Dealer");
            return _dealerBaseType;
        }

        private static bool IsDealerType(Type t)
        {
            var dealerType = GetDealerBaseType();
            if (dealerType == null) return false;
            return t != null && (t == dealerType || t.IsSubclassOf(dealerType));
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

        private static List<RecruitedGameDealer> ScanForRecruitedDealers()
        {
            var list = new List<RecruitedGameDealer>();
            try
            {
                var roots = GetRootTransforms();
                if (roots == null || roots.Length == 0) return list;

                var dealerType = GetDealerBaseType();
                if (dealerType == null) return list;

                var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var dealerButNotRecruited = new List<string>();

                foreach (var t in roots)
                {
                    if (t == null) continue;
                    var go = t.gameObject;
                    var goName = go.name;
                    if (string.IsNullOrEmpty(goName)) continue;

                    // Path 1: CharacterClasses.[name] that extend Dealer (Benji, Wei, etc.)
                    var charClassType = FindCharacterClassType(goName);
                    if (charClassType != null && IsDealerType(charClassType))
                    {
                        var comp = go.GetComponent(charClassType);
                        if (comp != null)
                        {
                            ProcessDealerComponent(comp, null, go, list, seen, dealerButNotRecruited);
                            continue;
                        }
                    }

                    // Path 2: ScheduleOne.Economy.Dealer directly (Molly, mod dealers)
                    var dealerComp = go.GetComponent(dealerType);
                    if (dealerComp != null)
                    {
                        ProcessDealerComponent(dealerComp, null, go, list, seen, dealerButNotRecruited);
                    }
                }

            }
            catch (Exception ex)
            {
            }
            return list.OrderBy(d => d.FullName).ToList();
        }

        private static void ProcessDealerComponent(object comp, object dealerObj, GameObject go,
            List<RecruitedGameDealer> list, HashSet<string> seen, List<string> dealerButNotRecruited)
        {
            bool recruited = IsRecruited(comp);
            var id = GetId(comp, dealerObj, go);
            var fullName = GetFullName(comp, dealerObj, go);

            if (!recruited)
            {
                if (!string.IsNullOrEmpty(fullName))
                    dealerButNotRecruited.Add($"{fullName}(Id={id})");
                return;
            }

            if (string.IsNullOrEmpty(id) || !seen.Add(id)) return;

            list.Add(new RecruitedGameDealer
            {
                Id = id,
                FullName = fullName,
                GameObject = go,
                DealerComponent = comp
            });
        }

        private static Transform[] GetRootTransforms()
        {
            var all = UnityEngine.Object.FindObjectsOfType<Transform>();
            if (all == null) return Array.Empty<Transform>();
            var roots = new List<Transform>(64);
            foreach (var t in all)
            {
                if (t != null && t.root == t)
                    roots.Add(t);
            }
            return roots.ToArray();
        }

        private static object GetDealerFromComponent(object comp)
        {
            var t = comp.GetType();
            var dealerProp = t.GetProperty("Dealer", BindingFlags.Public | BindingFlags.Instance);
            if (dealerProp != null)
                return dealerProp.GetValue(comp);
            return null;
        }

        private static bool IsRecruited(object dealerObj)
        {
            if (dealerObj == null) return false;
            var t = dealerObj.GetType();
            var prop = t.GetProperty("IsRecruited", BindingFlags.Public | BindingFlags.Instance);
            if (prop != null && prop.PropertyType == typeof(bool))
                return (bool)prop.GetValue(dealerObj);
            foreach (var m in t.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (m.Name != "IsRecruited" || m.ReturnType != typeof(bool)) continue;
                if (m.GetParameters().Length == 0)
                    return (bool)m.Invoke(dealerObj, null);
            }
            return false;
        }

        private static string GetId(object comp, object dealerObj, GameObject go)
        {
            foreach (var obj in new[] { comp, dealerObj })
            {
                if (obj == null) continue;
                var prop = obj.GetType().GetProperty("ID", BindingFlags.Public | BindingFlags.Instance);
                if (prop != null)
                {
                    var v = prop.GetValue(obj) as string;
                    if (!string.IsNullOrEmpty(v)) return v;
                }
            }
            return go.name?.ToLowerInvariant().Replace(" ", "_");
        }

        private static string GetFullName(object comp, object dealerObj, GameObject go)
        {
            foreach (var obj in new[] { comp, dealerObj })
            {
                if (obj == null) continue;
                var t = obj.GetType();
                var prop = t.GetProperty("fullName", BindingFlags.Public | BindingFlags.Instance)
                    ?? t.GetProperty("FullName", BindingFlags.Public | BindingFlags.Instance);
                if (prop != null)
                {
                    var v = prop.GetValue(obj) as string;
                    if (!string.IsNullOrEmpty(v)) return v;
                }
            }
            return go.name;
        }

        public static Vector3 GetDealerPosition(RecruitedGameDealer dealer)
        {
            if (dealer?.GameObject == null) return Vector3.zero;
            try
            {
                var comps = dealer.GameObject.GetComponents<Component>();
                foreach (var c in comps ?? Array.Empty<Component>())
                {
                    if (c == null) continue;
                    var footPosProp = c.GetType().GetProperty("FootPosition", BindingFlags.Public | BindingFlags.Instance);
                    if (footPosProp == null) continue;
                    var val = footPosProp.GetValue(c);
                    if (val is Vector3 v) return v;
                }
            }
            catch { }
            return dealer.GameObject.transform.position;
        }

        public static object GetDealerInventory(RecruitedGameDealer dealer)
        {
            if (dealer?.GameObject == null) return null;
            try
            {
                var comps = dealer.GameObject.GetComponents<Component>();
                foreach (var c in comps ?? Array.Empty<Component>())
                {
                    if (c == null) continue;
                    var insert = c.GetType().GetMethod("InsertItem", BindingFlags.Public | BindingFlags.Instance);
                    if (insert != null && insert.GetParameters().Length >= 1)
                        return c;
                }
                var children = dealer.GameObject.GetComponentsInChildren<Component>(true);
                foreach (var c in children ?? Array.Empty<Component>())
                {
                    if (c == null) continue;
                    var insert = c.GetType().GetMethod("InsertItem", BindingFlags.Public | BindingFlags.Instance);
                    if (insert != null && insert.GetParameters().Length >= 1)
                        return c;
                }
            }
            catch { }
            return null;
        }
    }
}
