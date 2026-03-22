using System.Collections;
using MelonLoader;
using MoreNPCs.Supervisor;
using UnityEngine;
using UnityEngine.UI;

namespace MoreNPCs.Manager
{
    /// <summary>Manager outline indicator (orange - red/blue/purple are taken).</summary>
    public static class ManagerIndicator
    {
        private static bool _done;
        private static readonly Color ManagerColor = new Color(1f, 0.55f, 0.2f, 1f);

        public static void Initialize() => MelonCoroutines.Start(PollAndSetupRoutine());

        private static IEnumerator PollAndSetupRoutine()
        {
            var wait = new WaitForSeconds(1f);
            while (!_done)
            {
                yield return wait;
                if (TrySetup()) _done = true;
            }
        }

        private static bool TrySetup()
        {
            try
            {
                var manager = SupervisorIndicator.FindTransformByName("thomas_ashford");
                if (manager == null) return false;
                var source = manager.Find("SupplierIndicator");
                if (source == null) return false;
                if (manager.Find("ManagerIndicator") != null) { _done = true; return true; }
                var copy = UnityEngine.Object.Instantiate(source.gameObject, manager);
                copy.name = "ManagerIndicator";
                copy.SetActive(true);
                copy.transform.SetSiblingIndex(0);
                if (copy.GetComponent<Image>() is Image img) img.color = ManagerColor;
                var textT = copy.transform.Find("Text (TMP)");
                if (textT != null)
                {
                    var tmp = textT.GetComponent("TMPro.TextMeshProUGUI") ?? (textT.childCount > 0 ? textT.GetChild(0).GetComponent("TMPro.TextMeshProUGUI") : null);
                    if (tmp != null)
                    {
                        tmp.GetType().GetProperty("color")?.SetValue(tmp, ManagerColor);
                        tmp.GetType().GetProperty("text")?.SetValue(tmp, "MANAGER");
                    }
                }
                return true;
            }
            catch { return false; }
        }

    }
}
