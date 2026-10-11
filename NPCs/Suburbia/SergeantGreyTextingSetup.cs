using System;
using System.Reflection;
using MelonLoader;
using MoreNPCs.Utils;
using ScheduleOne.Money;
using S1API.Entities;
using S1API.Messaging;

namespace MoreNPCs.NPCs
{
    internal static class SergeantGreyTextingSetup
    {
        public static void Setup(SergeantGrey grey)
        {
            if (grey == null) return;
            try
            {
                var gameNpc = GetGameNPC(grey);
                if (gameNpc == null) return;

                gameNpc.GetType()
                    .GetMethod("EnsureMessageConversationExists", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.Invoke(gameNpc, null);

                var msgConv = gameNpc.GetType().GetProperty("MSGConversation")?.GetValue(gameNpc);
                if (msgConv == null) return;

                var createSendable = msgConv.GetType().GetMethod("CreateSendableMessage", new[] { typeof(string) });
                if (createSendable == null) return;

                AddSendable(msgConv, createSendable,
                    "I need a patrol car off the books.",
                    () => SendVehicleOffer(grey));
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"SergeantGreyTextingSetup failed: {ex.Message}");
            }
        }

        private static void SendVehicleOffer(SergeantGrey grey)
        {
            string shitboxPrice = MoneyManager.FormatAmount(PoliceVehicleHelper.ShitboxPolicePrice, false, false);
            string bruiserPrice = MoneyManager.FormatAmount(PoliceVehicleHelper.BruiserPolicePrice, false, false);

            grey.SendTextMessage(
                "Yeah. Shitbox or bruiser. Card only. Pick one.",
                new[]
                {
                    new Response
                    {
                        Label = "grey_police_shitbox",
                        Text = $"Shitbox {shitboxPrice}",
                        OnTriggered = () => DeliverVehicle(
                            grey,
                            PoliceVehicleHelper.ShitboxPoliceCode,
                            PoliceVehicleHelper.ShitboxPolicePrice,
                            "Shitbox is out front.")
                    },
                    new Response
                    {
                        Label = "grey_police_bruiser",
                        Text = $"Bruiser {bruiserPrice}",
                        OnTriggered = () => DeliverVehicle(
                            grey,
                            PoliceVehicleHelper.BruiserPoliceCode,
                            PoliceVehicleHelper.BruiserPolicePrice,
                            "Bruiser is out front.")
                    },
                    new Response
                    {
                        Label = "grey_police_nevermind",
                        Text = "Nevermind.",
                        OnTriggered = () => grey.SendTextMessage("Alright.")
                    }
                });
        }

        private static void DeliverVehicle(SergeantGrey grey, string vehicleCode, float price, string confirmation)
        {
            if (!PoliceVehicleHelper.TryPurchaseAndSpawn(vehicleCode, price, out var failure))
            {
                grey.SendTextMessage(failure);
                return;
            }

            grey.SendTextMessage(confirmation);
        }

        private static void AddSendable(object msgConv, MethodInfo createSendable, string text, Action onSent)
        {
            var sendable = createSendable.Invoke(msgConv, new object[] { text });
            if (sendable == null) return;
            sendable.GetType().GetField("onSent")?.SetValue(sendable, onSent);
        }

        private static object? GetGameNPC(NPC npc)
        {
            if (npc == null) return null;
            var s1Npc = typeof(NPC).GetField("S1NPC", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(npc);
            return s1Npc ?? npc;
        }
    }
}
