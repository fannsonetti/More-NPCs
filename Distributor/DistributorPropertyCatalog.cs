using System;
using System.Collections.Generic;
using System.Reflection;
using S1API.Properties;

namespace MoreNPCs.Distributor
{
    internal static class DistributorPropertyCatalog
    {
        private static List<string>? _names;

        public static IReadOnlyList<string> AllNames
        {
            get
            {
                if (_names != null) return _names;

                _names = new List<string>();
                var type = typeof(Property);
                foreach (var staticField in type.GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    if (staticField.FieldType != type) continue;
                    if (!string.IsNullOrEmpty(staticField.Name))
                        _names.Add(staticField.Name);
                }

                _names.Sort(StringComparer.OrdinalIgnoreCase);
                return _names;
            }
        }

        public static string FormatName(string name) =>
            System.Text.RegularExpressions.Regex.Replace(name, "([a-z])([A-Z])", "$1 $2");
    }
}
