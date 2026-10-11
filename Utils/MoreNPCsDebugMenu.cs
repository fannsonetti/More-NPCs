using MelonLoader;
using MoreNPCs.Distributor;
using MoreNPCs.Persistence;
using MoreNPCs.Utils;
using S1API.Entities;
using UnityEngine;

namespace MoreNPCs.Utils
{
    /// <summary>F6 debug menu (off by default). Tabs: Photo, Distributor, Cheats.</summary>
    internal static class MoreNPCsDebugMenu
    {
        private const int WindowId = 0x4D4E5044; // "MNPD"

        private static readonly string[] Tabs = { "Photo", "Distributor", "Cheats" };

        private static bool _menuOpen;
        private static int _tabIndex;
        private static Rect _windowRect = new Rect(20f, 20f, 400f, 560f);
        private static string _statusMessage = "Enable Debug Menu in MelonPreferences to use F6.";

        public static void Update()
        {
            PhotoSessionTool.MaintainPinnedNpcs();

            if (!MoreNPCsPreferences.DebugMenu_Enabled.Value) return;
            if (!Input.GetKeyDown(KeyCode.F6)) return;

            _menuOpen = !_menuOpen;
            if (_menuOpen)
                _statusMessage = "MoreNPCs debug menu.";
        }

        public static void OnGUI()
        {
            if (!_menuOpen || !MoreNPCsPreferences.DebugMenu_Enabled.Value) return;

            _windowRect = GUILayout.Window(WindowId, _windowRect, DrawWindow, "MoreNPCs Debug (F6)");
        }

        private static void DrawWindow(int id)
        {
            _tabIndex = GUILayout.Toolbar(_tabIndex, Tabs);

            GUILayout.Space(6f);
            switch (_tabIndex)
            {
                case 0:
                    PhotoSessionTool.DrawDebugTab(ref _statusMessage);
                    break;
                case 1:
                    DrawDistributorTab();
                    break;
                default:
                    DrawCheatsTab();
                    break;
            }

            GUILayout.Space(6f);
            GUILayout.Label(_statusMessage, GUI.skin.box);

            if (GUILayout.Button("Close (F6)"))
                _menuOpen = false;

            GUI.DragWindow(new Rect(0f, 0f, 10000f, 20f));
        }

        private static void DrawDistributorTab()
        {
            GUILayout.Label("Richard Roe / distributor");

            GUILayout.Label($"Order: {DistributorWeeklyOrder.GetOrderSummaryLine()}");
            GUILayout.Label($"Deal active: {DistributorRunSave.IsDealActive()}");
            GUILayout.Label($"Deal expired: {DistributorRunSave.IsDealExpired()}");
            GUILayout.Label($"Sold: {DistributorRunSave.SoldThisRun}");
            GUILayout.Label($"Deadline day: {DistributorRunSave.DeadlineDay}");

            if (GUILayout.Button("Force active deal"))
            {
                var grant = DistributorRegistry.Grant;
                if (grant == null)
                {
                    _statusMessage = "Contact not spawned.";
                    return;
                }

                DistributorRunScheduler.ForceDeal(grant);
                _statusMessage = "Forced distributor deal + quest + dead drop.";
            }

            if (GUILayout.Button("Randomize order"))
            {
                DistributorWeeklyOrder.RollRandomOrderForDebug();
                _statusMessage = $"Rolled order: {DistributorWeeklyOrder.GetOrderSummaryLine()}";
            }

            if (GUILayout.Button("Reset sold flag"))
            {
                if (DistributorSave.Instance != null)
                    DistributorSave.Instance.SoldThisRun = false;
                DistributorRunSave.DebugResetSoldFlag();
                _statusMessage = "Reset distributor sold flag.";
            }

            if (GUILayout.Button("Expire active deal"))
            {
                DistributorRunScheduler.ExpireActiveDeal();
                _statusMessage = "Expired active distributor deal.";
            }
        }

        private static void DrawCheatsTab()
        {
            GUILayout.Label("Quick cheats");
            if (GUILayout.Button("Add $10,000"))
            {
                PlayerCashHelper.TryAddCash(10000f);
                _statusMessage = "Added $10,000.";
            }

            if (GUILayout.Button("Unlock Grant relationship"))
            {
                var grant = DistributorRegistry.Grant;
                grant?.Relationship?.Unlock(S1API.Entities.NPCRelationship.UnlockType.DirectApproach, notify: false);
                _statusMessage = "Grant relationship unlocked.";
            }
        }
    }
}
