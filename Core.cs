using MelonLoader;
using MoreNPCs.Utils;

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

        public override void OnInitializeMelon()
        {
            Instance = this;
            _unlockWatcher.Initialize();
        }

        public override void OnUpdate()
        {
            _cartelWatcher.Update();
            _unlockWatcher.Update();
            _buildingSetup.Update();
        }

        public override void OnApplicationQuit()
        {
            Instance = null;
        }
    }
}
