using MelonLoader;
using MoreNPCs.Utils;
using S1API.PhoneCalls;

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
