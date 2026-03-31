using HarmonyLib;
using ScheduleOne.DevUtilities;
using ScheduleOne.Map;
using ScheduleOne.UI.Phone.ContactsApp;

namespace MoreNPCs.Patches
{
    /// <summary>
    /// Shows <c>UnlockByRank</c> under the Contacts scroll locked panel when the selected region is
    /// Downtown, Docks, Suburbia, or Uptown. Resolves the object from <see cref="ContactsApp"/>'s transform
    /// so the path does not depend on the player root name (e.g. <c>Player (0)</c>).
    /// </summary>
    [HarmonyPatch(typeof(ContactsApp), nameof(ContactsApp.SetSelectedRegion), new[] { typeof(EMapRegion), typeof(bool) })]
    internal static class ContactsAppUnlockByRankVisibilityPatch
    {
        private const string UnlockByRankRelativePath = "Container/Scroll View/Locked/UnlockByRank";

        [HarmonyPostfix]
        private static void Postfix(ContactsApp __instance, EMapRegion region, bool selectNPC)
        {
            if (NetworkSingleton<GameManager>.Instance.IsTutorial)
                return;

            var unlockByRank = __instance.transform.Find(UnlockByRankRelativePath);
            if (unlockByRank == null)
                return;

            var show =
                region == EMapRegion.Downtown
                || region == EMapRegion.Docks
                || region == EMapRegion.Suburbia
                || region == EMapRegion.Uptown;

            if (unlockByRank.gameObject.activeSelf != show)
                unlockByRank.gameObject.SetActive(show);
        }
    }
}
