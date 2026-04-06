using System.Text;
using HarmonyLib;
using MelonLoader;
using S1API.Cartel;
using ScheduleOne.DevUtilities;
using ScheduleOne.Map;
using ScheduleOne.UI.Phone.ContactsApp;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MoreNPCs.Patches
{
    /// <summary>
    /// Shows <c>UnlockByRank</c> for Downtown–Uptown tabs, positions it, and sets copy on
    /// <c>UnlockByRank/Description</c>, <c>UnlockByRank/Description/Rank</c> (UI Text), and
    /// <c>UnlockByInfluence/Description</c>.
    /// </summary>
    [HarmonyPatch(typeof(ContactsApp), nameof(ContactsApp.SetSelectedRegion), new[] { typeof(EMapRegion), typeof(bool) })]
    internal static class ContactsAppUnlockByRankVisibilityPatch
    {
        private const string LogPrefix = "[MoreNPCs] ContactsApp UnlockByRank";
        private const string UnlockByRankRelativePath = "Container/Scroll View/Locked/UnlockByRank";
        private const string UnlockByInfluenceRelativePath = "Container/Scroll View/Locked/UnlockByInfluence";

        private static readonly Vector3 UnlockByRankLocalPosition = new Vector3(0.0488f, 138.9526f, 2.9327f);

        private const string UnlockByRankDescriptionText = "and reach rank     \n\n\nto unlock this region";

        [HarmonyPostfix]
        private static void Postfix(ContactsApp __instance, EMapRegion region, bool selectNPC)
        {
            if (NetworkSingleton<GameManager>.Instance.IsTutorial)
                return;

            var root = __instance.transform;

            var unlockByRank = root.Find(UnlockByRankRelativePath) ?? FindDeepByName(root, "UnlockByRank");
            if (unlockByRank == null)
            {
                MelonLogger.Warning($"{LogPrefix} UnlockByRank transform not found.");
                return;
            }

            var unlockByInfluence = root.Find(UnlockByInfluenceRelativePath) ?? FindDeepByName(root, "UnlockByInfluence");

            // While truced, cartel influence does not run the usual region-unlock loop; rank/influence hints are misleading.
            if (IsCartelTruced())
            {
                if (unlockByRank.gameObject.activeSelf)
                    unlockByRank.gameObject.SetActive(false);
                if (unlockByInfluence != null && unlockByInfluence.gameObject.activeSelf)
                    unlockByInfluence.gameObject.SetActive(false);
                return;
            }

            var show =
                region == EMapRegion.Downtown
                || region == EMapRegion.Docks
                || region == EMapRegion.Suburbia
                || region == EMapRegion.Uptown;

            if (unlockByRank.gameObject.activeSelf != show)
                unlockByRank.gameObject.SetActive(show);

            unlockByRank.localPosition = UnlockByRankLocalPosition;

            ApplyDescriptionText(unlockByRank.Find("Description"), UnlockByRankDescriptionText);
            if (show)
                ApplyRankText(unlockByRank.Find("Description/Rank"), GetRequiredRankLabelForTab(region));

            if (unlockByInfluence != null)
            {
                if (!unlockByInfluence.gameObject.activeSelf)
                    unlockByInfluence.gameObject.SetActive(true);
                var influenceText = BuildInfluenceDescription(region);
                ApplyDescriptionText(unlockByInfluence.Find("Description"), influenceText);
            }
            else
            {
                MelonLogger.Warning($"{LogPrefix} UnlockByInfluence transform not found (Description copy skipped).");
            }
        }

        private static bool IsCartelTruced()
        {
            var c = Cartel.Instance;
            return c != null && c.Status == CartelStatus.Truced;
        }

        private static string GetRequiredRankLabelForTab(EMapRegion tabRegion)
        {
            return tabRegion switch
            {
                EMapRegion.Downtown => "HUSTLER I",
                EMapRegion.Docks => "ENFORCER I",
                EMapRegion.Suburbia => "BLOCK BOSS I",
                EMapRegion.Uptown => "BARON I",
                _ => "",
            };
        }

        /// <summary>Sets <see cref="Text"/> on <c>UnlockByRank/Description/Rank</c> (not TMP).</summary>
        private static void ApplyRankText(Transform? rankTransform, string text)
        {
            if (rankTransform == null)
            {
                MelonLogger.Warning($"{LogPrefix} Description/Rank transform not found.");
                return;
            }

            var uiText = rankTransform.GetComponent<Text>();
            if (uiText != null)
            {
                uiText.text = text;
                return;
            }

            MelonLogger.Warning($"{LogPrefix} No UnityEngine.UI.Text on {GetTransformPath(rankTransform)}.");
        }

        private static string BuildInfluenceDescription(EMapRegion selectedRegion)
        {
            var targetRegion = GetCartelInfluenceRegionName(selectedRegion);
            // Same between-line gap as UnlockByRank; second line left empty (no threshold text).
            return $"Reduce cartel influence in {targetRegion} to\n\n\n";
        }

        /// <summary>
        /// Region where cartel influence must drop (vanilla: previous enum region) to open the selected one.
        /// </summary>
        private static string GetCartelInfluenceRegionName(EMapRegion selectedRegion)
        {
            if (selectedRegion == EMapRegion.Northtown)
                return EMapRegion.Northtown.ToString();
            return (selectedRegion - 1).ToString();
        }

        private static void ApplyDescriptionText(Transform? descriptionTransform, string text)
        {
            if (descriptionTransform == null)
            {
                MelonLogger.Warning($"{LogPrefix} Description child missing.");
                return;
            }

            var uiText = descriptionTransform.GetComponent<Text>();
            if (uiText != null)
            {
                uiText.text = text;
                return;
            }

            var tmp = descriptionTransform.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.text = text;
                return;
            }

            MelonLogger.Warning($"{LogPrefix} No Text or TextMeshProUGUI on {GetTransformPath(descriptionTransform)}.");
        }

        private static string GetTransformPath(Transform t)
        {
            return t == null ? "(null)" : BuildPath(t);
        }

        private static string BuildPath(Transform t)
        {
            var sb = new StringBuilder();
            for (var i = t; i != null; i = i.parent)
            {
                if (sb.Length > 0)
                    sb.Insert(0, "/");
                sb.Insert(0, i.name);
            }
            return sb.ToString();
        }

        private static Transform? FindDeepByName(Transform parent, string name)
        {
            if (parent == null)
                return null;
            for (var i = 0; i < parent.childCount; i++)
            {
                var c = parent.GetChild(i);
                if (c.name == name)
                    return c;
                var found = FindDeepByName(c, name);
                if (found != null)
                    return found;
            }
            return null;
        }
    }
}
