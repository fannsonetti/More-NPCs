using MoreNPCs.Save;
using ScheduleOne.DevUtilities;
using ScheduleOne.Persistence;
using ScheduleOne.Persistence.Datas;
using ScheduleOne.Persistence.Loaders;
using System.Collections.Generic;
using UnityEngine;

namespace MoreNPCs.Saveables
{
    /// <summary>
    /// Per-save-slot store for police bribes (officerId -> bribed).
    /// Implements S1API save interfaces so it persists with the game's save files.
    /// </summary>
    public class PoliceBribeState : IBaseSaveable, ISaveable
    {
        public static PoliceBribeState Instance { get; } = new PoliceBribeState();

        private PoliceBribeData _data = new PoliceBribeData();

        // Our Loader instance knows how to parse and push data back into this object.
        private readonly PoliceBribeLoader _loader;

        private PoliceBribeState()
        {
            _loader = new PoliceBribeLoader(this);
        }

        // ----- IBaseSaveable / ISaveable -----

        public string SaveFolderName => "Police";
        public string SaveFileName => "BribeState";

        public bool ShouldSaveUnderFolder => false;

        public List<string> LocalExtraFiles { get; set; } = new List<string>();
        public List<string> LocalExtraFolders { get; set; } = new List<string>();

        public bool HasChanged { get; set; }

        public int LoadOrder { get; } = 10;

        public Loader Loader => _loader;

        /// <summary>
        /// Required by your ISaveable; register with SaveManager here.
        /// Call this once at startup (the engine may call it for you).
        /// </summary>
        public void InitializeSaveable()
        {
            // Safe to call multiple times (SaveManager usually ignores duplicates).
            Singleton<SaveManager>.Instance.RegisterSaveable(this);
        }

        /// <summary>
        /// SaveManager calls this when writing the save slot. Return JSON string.
        /// </summary>
        public string GetSaveString() => _data.GetJson(pretty: true);

        /// <summary>
        /// Called by our Loader after it parses the JSON content.
        /// </summary>
        public void Load(PoliceBribeData data)
        {
            _data = data ?? new PoliceBribeData();
            HasChanged = false;
        }

        // ----- Public API used by your gameplay/dialogue -----

        public bool IsBribed(string officerId)
        {
            if (string.IsNullOrWhiteSpace(officerId)) return false;
            return _data.Contains(officerId);
        }

        public void MarkBribed(string officerId)
        {
            if (string.IsNullOrWhiteSpace(officerId)) return;
            if (!_data.Contains(officerId))
            {
                _data.SetBribed(officerId);
                HasChanged = true; // tell SaveManager there are changes to write
            }
        }
    }

    /// <summary>
    /// Loader required by S1API: Load(string) returns void.
    /// It parses JSON and injects the result into the owning saveable.
    /// </summary>
    public class PoliceBribeLoader : Loader
    {
        private readonly PoliceBribeState _owner;

        public PoliceBribeLoader(PoliceBribeState owner)
        {
            _owner = owner;
        }

        public override void Load(string content)
        {
            var parsed = PoliceBribeData.FromJson(content);
            _owner.Load(parsed);
        }
    }
}
