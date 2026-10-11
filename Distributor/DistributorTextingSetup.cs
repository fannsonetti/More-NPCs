using System;
using System.Collections;
using System.Reflection;
using MelonLoader;
using MoreNPCs.NPCs;
using S1API.Entities;
using UnityEngine;

namespace MoreNPCs.Distributor
{
    internal static class DistributorTextingSetup
    {
        private const float MessageGapSeconds = 4f;

        public static void Setup(GrantEllison grant)
        {
            if (grant == null) return;
            try
            {
                ClearSendables(grant);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"DistributorTextingSetup failed: {ex.Message}");
            }
        }

        public static void SendDealOffer(GrantEllison grant)
        {
            if (grant == null) return;
            ClearSendables(grant);
            MelonCoroutines.Start(SendDealOfferRoutine(grant));
        }

        private static IEnumerator SendDealOfferRoutine(GrantEllison grant)
        {
            var messages = DistributorText.DealOfferMessages();
            if (messages.Length == 0) yield break;

            grant.SendTextMessage(messages[0]);
            for (int i = 1; i < messages.Length; i++)
            {
                yield return new WaitForSeconds(MessageGapSeconds);
                grant.SendTextMessage(messages[i]);
            }
        }

        private static void ClearSendables(GrantEllison grant)
        {
            var s1Npc = typeof(NPC).GetField("S1NPC", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(grant);
            if (s1Npc == null) return;

            s1Npc.GetType()
                .GetMethod("EnsureMessageConversationExists", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                ?.Invoke(s1Npc, null);

            var msgConv = s1Npc.GetType().GetField("MSGConversation", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(s1Npc);
            if (msgConv == null) return;

            if (msgConv.GetType().GetField("Sendables", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(msgConv) is IList sendables)
                sendables.Clear();
        }
    }
}
