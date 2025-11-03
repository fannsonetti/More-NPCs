using MelonLoader;
using MoreNPCs.Saveables;
using MoreNPCs.Utils;
using ScheduleOne.DevUtilities;
using ScheduleOne.Persistence;
using UnityEngine;

[assembly: MelonInfo(typeof(MoreNPCs.Core), Constants.MOD_NAME, Constants.MOD_VERSION, Constants.MOD_AUTHOR)]
[assembly: MelonGame(Constants.Game.GAME_STUDIO, Constants.Game.GAME_NAME)]

namespace MoreNPCs
{
    public class Core : MelonMod
    {
        public static Core? Instance { get; private set; }

        private CartelStatusWatcher _cartelWatcher = new CartelStatusWatcher();

        public override void OnInitializeMelon()
        {
            Instance = this;
            MelonLogger.Msg("MoreNPCs mod initialized");
            Singleton<SaveManager>.Instance.RegisterSaveable(PoliceBribeState.Instance);
        }

        public override void OnUpdate()
        {
            _cartelWatcher.Update(); // <- this calls the watcher logic
        }

        public override void OnApplicationQuit()
        {
            Instance = null;
        }
    }
}
