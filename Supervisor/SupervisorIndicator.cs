using System.Collections;
using System.Collections.Generic;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace MoreNPCs.Supervisor
{
    /// <summary>Supervisor outline indicators for Silas and Dominic (purple).</summary>
    public static class SupervisorIndicator
    {
        private static readonly HashSet<string> _done = new HashSet<string>();
        private static readonly Color SupervisorColor = new Color(0.655f, 0.545f, 0.98f, 1f);

        public static void Initialize() => MelonCoroutines.Start(PollAndSetupRoutine());

        private static IEnumerator PollAndSetupRoutine()
        {
            var wait = new WaitForSeconds(1f);
            while (_done.Count < 2)
            {
                yield return wait;
                TrySetupFor("silas_mercer", "SupervisorIndicator", "SUPERVISOR", SupervisorColor);
                TrySetupFor("dominic_cross", "SupervisorIndicator", "SUPERVISOR", SupervisorColor);
            }
        }

        private static void TrySetupFor(string npcName, string indicatorName, string labelText, Color color)
        {
            if (_done.Contains(npcName)) return;
            try
            {
                var npc = FindTransformByName(npcName);
                if (npc == null) return;
                var source = npc.Find("SupplierIndicator");
                if (source == null) return;
                if (npc.Find(indicatorName) != null) { _done.Add(npcName); return; }
                var copy = UnityEngine.Object.Instantiate(source.gameObject, npc);
                copy.name = indicatorName;
                copy.SetActive(true);
                copy.transform.SetSiblingIndex(0);
                if (copy.GetComponent<Image>() is Image img) img.color = color;
                var textT = copy.transform.Find("Text (TMP)");
                if (textT != null)
                {
                    var tmp = textT.GetComponent("TMPro.TextMeshProUGUI") ?? (textT.childCount > 0 ? textT.GetChild(0).GetComponent("TMPro.TextMeshProUGUI") : null);
                    if (tmp != null)
                    {
                        tmp.GetType().GetProperty("color")?.SetValue(tmp, color);
                        tmp.GetType().GetProperty("text")?.SetValue(tmp, labelText);
                    }
                }
                _done.Add(npcName);
            }
            catch { }
        }

        internal static Transform FindTransformByName(string name)
        {
            if (GameObject.Find(name) is GameObject go) return go.transform;
            foreach (var rootName in new[] { "ContactsApp", "Circles_FullGame", "Content", "Phone" })
            {
                var root = GameObject.Find(rootName);
                if (root == null) continue;
                foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
                    if (t.name == name) return t;
            }
            return null;
        }
    }
}
