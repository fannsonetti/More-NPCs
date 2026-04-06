using System;
using System.Collections;
using System.Reflection;
using HarmonyLib;
using MelonLoader;
using MoreNPCs.NPCs;
using S1API.Entities;
using UnityEngine;

namespace MoreNPCs.Utils
{
    /// <summary>
    /// Locked-state dialogue for P.P. Hyland: rotating greetings and a free-sample choice that opens the handover screen (Give Free Sample) via the game <c>Customer.SampleAccepted</c> path.
    /// When unlocked, clears dialogue override so normal customer flow runs (same refresh pattern as supervisors/manager).
    /// </summary>
    public static class PPHylandDialogue
    {
        private const string ContainerName = "pp_hyland_dialogue";

        private static readonly string[] Greetings =
        {
            "Afternoon.",
            "Hi.",
            "Hello.",
            "Good Afternoon."
        };

        public static void SetupFor(PPHyland npc)
        {
            if (npc?.Dialogue == null) return;

            npc.Dialogue.BuildAndSetDatabase(db => db.WithModuleEntry("PPHyland", ContainerName, "PPHyland"));
            npc.Dialogue.OnConversationStart(() => Refresh(npc));
            Refresh(npc);
        }

        public static void RefreshFor(PPHyland npc) => Refresh(npc);

        private static void Refresh(PPHyland npc)
        {
            var dialogue = npc.Dialogue;
            if (dialogue == null) return;

            dialogue.ClearCallbacks();
            dialogue.OnConversationStart(() => Refresh(npc));

            if (npc.Relationship?.IsUnlocked == true)
            {
                ClearOverrideContainer(npc);
                return;
            }

            string greeting = Greetings[UnityEngine.Random.Range(0, Greetings.Length)];

            dialogue.BuildAndRegisterContainer(ContainerName, c =>
            {
                c.AddNode("ENTRY", greeting, ch => ch
                    .Add("free_sample", "Can I interest you in a free sample?", "FREE_SAMPLE_ACTION"));
                c.AddNode("FREE_SAMPLE_ACTION", "", _ => { });
            });

            dialogue.OnChoiceSelected("free_sample", () =>
            {
                MelonCoroutines.Start(OpenFreeSampleAfterDialogueCloses(npc));
            });

            dialogue.UseContainerOnInteract(ContainerName);
        }

        private static IEnumerator OpenFreeSampleAfterDialogueCloses(PPHyland npc)
        {
            npc.Dialogue?.End();
            // Let dialogue / input stack settle before opening handover (avoids NRE inside game code).
            yield return null;
            yield return new WaitForEndOfFrame();
            yield return null;

            if (!TryInvokeSampleAccepted(npc))
                TryRecoverFromDialogueSoftlock();
        }

        private static Component? ResolveCustomerComponent(PPHyland npc)
        {
            if (npc?.gameObject == null) return null;

            Type? customerType = FindGameType("ScheduleOne.Economy.Customer")
                ?? FindGameType("Il2CppScheduleOne.Economy.Customer");
            if (customerType == null) return null;

            var go = npc.gameObject;
            var onRoot = go.GetComponent(customerType);
            if (onRoot != null) return onRoot;

            var children = go.GetComponentsInChildren(customerType, true);
            if (children != null && children.Length > 0)
                return children[0];

            return null;
        }

        /// <summary>Returns true if invoke completed without throwing.</summary>
        private static bool TryInvokeSampleAccepted(PPHyland npc)
        {
            try
            {
                var comp = ResolveCustomerComponent(npc);
                if (comp == null)
                {
                    MelonLogger.Warning("[MoreNPCs] PPHylandDialogue: Customer component not found on NPC (check hierarchy).");
                    return false;
                }

                var rt = comp.GetType();
                MethodInfo? boolMethod = null;
                MethodInfo? voidMethod = null;
                foreach (var mi in rt.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (mi.Name != "SampleAccepted") continue;
                    var p = mi.GetParameters();
                    if (p.Length == 0)
                        voidMethod = mi;
                    else if (p.Length == 1 && IsBoolParameter(p[0].ParameterType))
                        boolMethod = mi;
                }

                // Dialogue-initiated samples often use SampleAccepted(bool) — try before parameterless.
                if (boolMethod != null)
                {
                    boolMethod.Invoke(comp, new object[] { true });
                    return true;
                }

                if (voidMethod != null)
                {
                    voidMethod.Invoke(comp, null);
                    return true;
                }

                MelonLogger.Warning("[MoreNPCs] PPHylandDialogue: No SampleAccepted(bool) or SampleAccepted() on Customer.");
                return false;
            }
            catch (Exception ex)
            {
                var inner = ex.InnerException ?? ex;
                MelonLogger.Error($"[MoreNPCs] PPHylandDialogue SampleAccepted failed: {inner.Message}\n{inner.StackTrace}");
                return false;
            }
        }

        /// <summary>Best-effort unlock if handover never opened after dialogue ended.</summary>
        private static void TryRecoverFromDialogueSoftlock()
        {
            try
            {
                var playerType = FindGameType("ScheduleOne.PlayerScripts.Player")
                    ?? FindGameType("Il2CppScheduleOne.PlayerScripts.Player");
                if (playerType == null) return;

                var player = TryGetStaticPlayerInstance(playerType);
                if (player == null) return;

                var pt = player.GetType();
                foreach (var name in new[] { "SetCanMove", "SetPlayerEnabled", "SetMovementEnabled" })
                {
                    var m = AccessTools.Method(pt, name, new[] { typeof(bool) });
                    if (m != null)
                    {
                        m.Invoke(player, new object[] { true });
                        break;
                    }
                }

                var canMove = AccessTools.Property(pt, "CanMove");
                canMove?.SetValue(player, true);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[MoreNPCs] PPHylandDialogue softlock recovery: {ex.Message}");
            }
        }

        private static void ClearOverrideContainer(NPC npc)
        {
            try
            {
                var handlerType = FindGameType("ScheduleOne.Dialogue.DialogueHandler")
                    ?? FindGameType("Il2CppScheduleOne.Dialogue.DialogueHandler");
                var controllerType = FindGameType("ScheduleOne.Dialogue.DialogueController")
                    ?? FindGameType("Il2CppScheduleOne.Dialogue.DialogueController");
                if (handlerType == null || controllerType == null) return;

                var handler = npc.gameObject.GetComponentInChildren(handlerType, true) as Component;
                if (handler == null) return;

                var controller = handler.GetComponent(controllerType);
                if (controller == null) return;

                controllerType.GetMethod("ClearOverrideContainer", BindingFlags.Public | BindingFlags.Instance)
                    ?.Invoke(controller, null);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"PPHylandDialogue ClearOverrideContainer: {ex.Message}");
            }
        }

        private static Type? FindGameType(string fullName)
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                var t = asm.GetType(fullName);
                if (t != null) return t;
            }

            return null;
        }

        private static bool IsBoolParameter(Type t)
        {
            if (t == typeof(bool)) return true;
            var n = t.Name;
            return n == "Boolean" || (t.FullName != null && t.FullName.Contains("Boolean"));
        }

        private static object? TryGetStaticPlayerInstance(Type playerType)
        {
            foreach (var name in new[] { "Local", "Instance", "Singleton" })
            {
                var p = AccessTools.Property(playerType, name);
                if (p?.GetMethod?.IsStatic == true)
                {
                    var v = p.GetValue(null);
                    if (v != null) return v;
                }

                var f = AccessTools.Field(playerType, name);
                if (f?.IsStatic == true)
                {
                    var v = f.GetValue(null);
                    if (v != null) return v;
                }
            }

            return null;
        }
    }
}

