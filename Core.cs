using MelonLoader;
using MoreNPCs.Manager;
using MoreNPCs.Patches;
using MoreNPCs.Utils;
using S1API.Leveling;
using UnityEngine;

[assembly: MelonInfo(typeof(MoreNPCs.Core), Constants.MOD_NAME, Constants.MOD_VERSION, Constants.MOD_AUTHOR)]
[assembly: MelonGame(Constants.Game.GAME_STUDIO, Constants.Game.GAME_NAME)]

namespace MoreNPCs
{
    public class Core : MelonMod
    {
        public static Core? Instance { get; private set; }

        private CartelStatusWatcher _cartelWatcher = new CartelStatusWatcher();
        private NPCUnlockWatcher _unlockWatcher = new NPCUnlockWatcher();
        private BuildingSetup _buildingSetup = new BuildingSetup();
        private float _nextFullRankLogAt;

        private const float FullRankLogIntervalSeconds = 30f;

        public override void OnInitializeMelon()
        {
            Instance = this;
            MoreNPCsPreferences.Register();
            new HarmonyLib.Harmony("MoreNPCs").PatchAll(typeof(CustomerNewCustomerCartelInfluencePatch).Assembly);
            _unlockWatcher.Initialize();
        }

        public override void OnUpdate()
        {
            if (Time.time >= _nextFullRankLogAt)
            {
                _nextFullRankLogAt = Time.time + FullRankLogIntervalSeconds;
                if (LevelManager.Exists)
                    MelonLogger.Msg($"Full rank: {LevelManager.CurrentRank}");
            }

            _cartelWatcher.Update();
            _unlockWatcher.Update();
            _buildingSetup.Update();
            ReOfficePropertyBusinessUnlock.Update();
        }

        public override void OnApplicationQuit()
        {
            Instance = null;
        }
    }
}
