using MelonLoader;
using MoreNPCs.Distributor;
using MoreNPCs.Manager;
using MoreNPCs.Patches;
using MoreNPCs.Persistence;
using MoreNPCs.Utils;
using UnityEngine;

[assembly: MelonInfo(typeof(MoreNPCs.Core), Constants.MOD_NAME, Constants.MOD_VERSION, Constants.MOD_AUTHOR)]
[assembly: MelonGame(Constants.Game.GAME_STUDIO, Constants.Game.GAME_NAME)]

namespace MoreNPCs
{
    public class Core : MelonMod
    {
        public static Core? Instance { get; private set; }

        private NPCUnlockWatcher _unlockWatcher = new NPCUnlockWatcher();
        private BuildingSetup _buildingSetup = new BuildingSetup();

        public override void OnInitializeMelon()
        {
            Instance = this;
            MoreNPCsPreferences.Register();
            new HarmonyLib.Harmony("MoreNPCs").PatchAll(typeof(CustomerNewCustomerCartelInfluencePatch).Assembly);
            PoliceVehicleSirenPrompt.Initialize();
        }

        public override void OnUpdate()
        {
            _unlockWatcher.Update();
            _buildingSetup.Update();
            ReOfficePropertyBusinessUnlock.Update();
            PPHylandHandoverWarning.RefreshThrottled();
            PhotoSessionTool.Update();
            MoreNPCsDebugMenu.Update();
            DistributorRuntime.Update();
            PoliceVehicleHelper.UpdateTrunkWatcher();
            PoliceVehicleHelper.UpdatePoliceVehicleInput();
            PoliceVehicleSirenPrompt.Update();
            DealersCashSave.UpdateThrottled();
        }

        public override void OnGUI()
        {
            MoreNPCsDebugMenu.OnGUI();
        }

        public override void OnApplicationQuit()
        {
            Instance = null;
        }
    }
}
