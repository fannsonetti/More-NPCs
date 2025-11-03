using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoreNPCs.Save
{
    [Serializable]
    public class PoliceBribeData
    {
        // officerId -> bribed?
        public List<string> officerIds = new List<string>();

        public bool Contains(string officerId) => officerIds.Contains(officerId);

        public void SetBribed(string officerId)
        {
            if (!officerIds.Contains(officerId))
                officerIds.Add(officerId);
        }

        public string GetJson(bool pretty = false)
            => JsonUtility.ToJson(this, pretty);

        public static PoliceBribeData FromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return new PoliceBribeData();
            try { return JsonUtility.FromJson<PoliceBribeData>(json) ?? new PoliceBribeData(); }
            catch { return new PoliceBribeData(); }
        }
    }
}
