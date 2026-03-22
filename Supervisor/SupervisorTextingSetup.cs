using System;
using System.Reflection;
using MelonLoader;
using MoreNPCs.NPCs;
using S1API.Entities;
using UnityEngine;


namespace MoreNPCs.Supervisor
{
    /// <summary>Texting options for Silas: total take, running low, busy, come to me.</summary>
    public static class SupervisorTextingSetup
    {
        public static void Setup(SilasMercer silas) => SetupForSupervisor(silas, SupervisorIds.Silas);

        public static void SetupForDominic(NPC npc) => SetupForSupervisor(npc, SupervisorIds.Dominic);

        public static void SetupForSupervisor(NPC npc, string supervisorId)
        {
            if (npc == null || string.IsNullOrEmpty(supervisorId)) return;
            try
            {
                var gameNpc = GetGameNPC(npc);
                if (gameNpc == null) return;
                gameNpc.GetType().GetMethod("EnsureMessageConversationExists", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(gameNpc, null);
                var msgConv = gameNpc.GetType().GetProperty("MSGConversation")?.GetValue(gameNpc);
                if (msgConv == null) return;
                var createSendable = msgConv.GetType().GetMethod("CreateSendableMessage", new[] { typeof(string) });
                if (createSendable == null) return;
                AddSendable(msgConv, createSendable, "What's the total take?", () => RespondTotalTake(npc, supervisorId));
                AddSendable(msgConv, createSendable, "Anyone running low?", () => RespondRunningLow(npc, supervisorId));
                AddSendable(msgConv, createSendable, "Are you busy?", () => RespondBusy(npc, supervisorId));
                // "Come to me." added when relationship >= 5 via AddComeToMeOption
            }
            catch (Exception ex) { MelonLogger.Warning($"SupervisorTextingSetup failed: {ex.Message}"); }
        }

        private static void AddSendable(object msgConv, MethodInfo createSendable, string text, Action onSent)
        {
            var sendable = createSendable.Invoke(msgConv, new object[] { text });
            if (sendable == null) return;
            sendable.GetType().GetField("onSent")?.SetValue(sendable, onSent);
        }

        private static readonly string[] TotalTakeResponses = { "${0:N0} total.", "We're at ${0:N0}.", "Current total is ${0:N0}." };
        private static readonly string[] BusyResponses = { "No, just waiting right now.", "Yeah, picking up money.", "Yeah, I'm making a drop right now." };
        private static int _totalTakeIndex;

        private static void RespondTotalTake(NPC npc, string supervisorId)
        {
            var amount = SupervisorManager.GetStoredCash(supervisorId);
            npc.SendTextMessage(string.Format(TotalTakeResponses[_totalTakeIndex++ % TotalTakeResponses.Length], amount));
        }

        private static void RespondRunningLow(NPC npc, string supervisorId)
        {
            var lowCount = SupervisorManager.GetDealersRunningLowCount(supervisorId);
            npc.SendTextMessage(lowCount == 0 ? "Nobody right now." : lowCount <= 2 ? "One or two are low." : "Yeah, a couple do.");
        }

        private static void RespondBusy(NPC npc, string supervisorId)
        {
            int idx = SupervisorEarningsCollector.IsCollectingFor(supervisorId) ? 1 : SupervisorDrugDistributor.IsRunningFor(supervisorId) ? 2 : 0;
            npc.SendTextMessage(BusyResponses[idx]);
        }

        private static void RespondComeToMe(NPC npc)
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

        public static void SendMessageFromPrimary(SilasMercer silas, string message)
        {
            if (silas == null || string.IsNullOrEmpty(message)) return;
            try { silas.SendTextMessage(message); } catch { }
        }

        public static void SendMessageFrom(NPC npc, string message)
        {
            if (npc == null || string.IsNullOrEmpty(message)) return;
            try { npc.SendTextMessage(message); } catch { }
        }

        /// <summary>Adds "Come to me." option when relationship reaches 5. Call once per NPC.</summary>
        public static void AddComeToMeOption(NPC npc)
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
            catch (Exception ex) { MelonLogger.Warning($"SupervisorTextingSetup.AddComeToMeOption failed: {ex.Message}"); }
        }

        private static object GetGameNPC(NPC npc)
        {
            if (npc == null) return null;
            var gameNpcType = FindType("ScheduleOne.NPCs.NPC");
            if (gameNpcType == null) return null;
            if (gameNpcType.IsInstanceOfType(npc)) return npc;
            return npc.gameObject?.GetComponent(gameNpcType);
        }

        private static Type FindType(string fullName)
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                var t = asm.GetType(fullName);
                if (t != null) return t;
            }
            return null;
        }
    }
}
