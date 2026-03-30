using System;
using System.Reflection;
using MelonLoader;
using MoreNPCs.NPCs;
using S1API.Entities;
using UnityEngine;

namespace MoreNPCs.Manager
{
    /// <summary>Texting options for Thomas: total held, earnings, busy, come to me.</summary>
    public static class ManagerTextingSetup
    {
        public static void Setup(ThomasAshford npc)
        {
            if (npc == null) return;
            try
            {
                var gameNpc = GetGameNPC(npc);
                if (gameNpc == null) return;
                gameNpc.GetType().GetMethod("EnsureMessageConversationExists", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(gameNpc, null);
                var msgConv = gameNpc.GetType().GetProperty("MSGConversation")?.GetValue(gameNpc);
                if (msgConv == null) return;
                var createSendable = msgConv.GetType().GetMethod("CreateSendableMessage", new[] { typeof(string) });
                if (createSendable == null) return;
                AddSendable(msgConv, createSendable, "What are you holding?", () => RespondHolding(npc));
                AddSendable(msgConv, createSendable, "Any earnings to collect?", () => RespondEarnings(npc));
                AddSendable(msgConv, createSendable, "Are you busy?", () => RespondBusy(npc));
                // "Come to me." added when relationship >= 5 via AddComeToMeOption
            }
            catch (Exception ex) { MelonLogger.Warning($"ManagerTextingSetup failed: {ex.Message}"); }
        }

        /// <summary>Adds "Come to me." option when relationship reaches 5. Call once per NPC.</summary>
        public static void AddComeToMeOption(ThomasAshford npc)
        {
            if (npc == null) return;
            try
            {
                var gameNpc = GetGameNPC(npc);
                if (gameNpc == null) return;
                var msgConv = gameNpc.GetType().GetProperty("MSGConversation")?.GetValue(gameNpc);
                if (msgConv == null) return;
                var createSendable = msgConv.GetType().GetMethod("CreateSendableMessage", new[] { typeof(string) });
                if (createSendable == null) return;
                AddSendable(msgConv, createSendable, "Come to me.", () => RespondComeToMe(npc));
            }
            catch (Exception ex) { MelonLogger.Warning($"ManagerTextingSetup.AddComeToMeOption failed: {ex.Message}"); }
        }

        public static void SendMessageFrom(ThomasAshford npc, string message)
        {
            if (npc == null || string.IsNullOrEmpty(message)) return;
            try { npc.SendTextMessage(message); } catch { }
        }

        private static void AddSendable(object msgConv, MethodInfo createSendable, string text, Action onSent)
        {
            var sendable = createSendable.Invoke(msgConv, new object[] { text });
            if (sendable == null) return;
            sendable.GetType().GetField("onSent")?.SetValue(sendable, onSent);
        }

        private static readonly string[] HoldingResponses = { "${0:N0} on hand.", "I'm holding ${0:N0}.", "We've got ${0:N0} in the kitty." };
        private static readonly string[] BusyResponses = { "No, just waiting.", "Yeah, making the rounds.", "Yeah, I'm at a business right now." };
        private static int _holdingIndex;

        private static void RespondHolding(ThomasAshford npc)
        {
            var amount = ManagerBusinessSave.GetStored();
            npc.SendTextMessage(string.Format(HoldingResponses[_holdingIndex++ % HoldingResponses.Length], amount));
        }

        private static void RespondEarnings(ThomasAshford npc)
        {
            var earnings = ManagerBusinessSave.GetBusinessEarnings();
            npc.SendTextMessage(earnings > 0 ? $"${earnings:N0} ready for you." : "Nothing yet.");
        }

        private static void RespondBusy(ThomasAshford npc)
        {
            int idx = ManagerLaunderingChain.IsRunning ? 1 : 0;
            npc.SendTextMessage(BusyResponses[idx]);
        }

        private static void RespondComeToMe(ThomasAshford npc)
        {
            var player = Player.Local;
            if (player?.Transform == null) { npc.SendTextMessage("Where are you?"); return; }
            var playerPos = player.Transform.position;
            var npcPos = npc.Transform?.position ?? npc.Movement?.FootPosition ?? playerPos;
            var dir = (npcPos - playerPos);
            var dest = dir.sqrMagnitude > 0.01f
                ? playerPos + dir.normalized * 2f
                : playerPos + Vector3.forward * 2f;
            if (npc?.Movement == null) { npc.SendTextMessage("Can't right now."); return; }
            npc.Movement.SetDestination(dest);
            npc.SendTextMessage("On my way.");
        }

        private static object GetGameNPC(NPC npc)
        {
            if (npc == null) return null;
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                var t = asm.GetType("ScheduleOne.NPCs.NPC");
                if (t == null) continue;
                if (t.IsInstanceOfType(npc)) return npc;
                return npc.gameObject?.GetComponent(t);
            }
            return null;
        }
    }
}
