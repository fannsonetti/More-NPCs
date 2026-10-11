using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using MelonLoader;
using MoreNPCs.Distributor;
using MoreNPCs.Persistence;
using MoreNPCs.Quests;
using MoreNPCs.Utils;
using S1API.Entities;
using UnityEngine;

namespace MoreNPCs.NPCs
{
    /// <summary>
    /// Richard Roe — anonymous wholesale SMS contact (non-physical drop-deal buyer).
    /// </summary>
    public sealed class GrantEllison : NPC
    {
        public override bool IsPhysical => false;

        protected override void ConfigurePrefab(NPCPrefabBuilder builder)
        {
            var icon = QuestIconLoader.Load("unknown_contact.png");
            builder.WithIdentity(DistributorIds.Grant, DistributorIds.ContactFirstName, DistributorIds.ContactLastName);
            if (icon != null)
                builder.WithIcon(icon);
            builder.WithSpawnPosition(DistributorConfig.HiddenPosition)
                .WithRelationshipDefaults(r =>
                {
                    r.WithDelta(2.0f)
                        .SetUnlocked(false)
                        .SetUnlockType(NPCRelationship.UnlockType.DirectApproach);
                });
        }

        public GrantEllison() : base()
        {
        }

        protected override void OnCreated()
        {
            try
            {
                base.OnCreated();
                ConversationCanBeHidden = true;
                ApplyContactIcon();
                ClearConversationCategoriesViaReflection();
                MelonCoroutines.Start(ApplyContactStyleRoutine());

                EnsureUnlockedIfPreviouslyIntroduced();

                DistributorRegistry.Grant = this;
                DistributorTextingSetup.Setup(this);
                DistributorRunScheduler.EnsureSubscribed();
                DistributorWeeklyOrder.EnsureCurrentWeekOrder();
                DistributorQuestManager.Initialize();

                if (DistributorRunSave.IsDealActive())
                {
                    DistributorQuestManager.StartDeal(restore: true);
                    DistributorDropBox.Enable();
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"GrantEllison OnCreated failed: {ex.Message}");
            }
        }

        private IEnumerator ApplyContactStyleRoutine()
        {
            yield return new WaitForSeconds(2f);
            ApplyContactIcon();
            ClearConversationCategoriesViaReflection();
        }

        private void ApplyContactIcon()
        {
            try
            {
                var icon = QuestIconLoader.Load("unknown_contact.png");
                if (icon == null)
                {
                    MelonLogger.Warning("GrantEllison: unknown_contact.png not found in embedded resources.");
                    return;
                }

                Icon = icon;
                RefreshMessagingIcons();
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"GrantEllison.ApplyContactIcon: {ex.Message}");
            }
        }

        private void ClearConversationCategoriesViaReflection()
        {
            try
            {
                var s1NpcField = typeof(NPC).GetField("S1NPC", BindingFlags.NonPublic | BindingFlags.Instance);
                var s1Npc = s1NpcField?.GetValue(this);
                if (s1Npc == null) return;

                var catsField = s1Npc.GetType().GetField("ConversationCategories", BindingFlags.Public | BindingFlags.Instance);
                var cats = catsField?.GetValue(s1Npc) as IList;
                cats?.Clear();

                var msgConvField = s1Npc.GetType().GetField("MSGConversation", BindingFlags.Public | BindingFlags.Instance);
                var msgConv = msgConvField?.GetValue(s1Npc);
                if (msgConv == null) return;

                var t = msgConv.GetType();
                var convCats = t.GetProperty("Categories", BindingFlags.Public | BindingFlags.Instance)?.GetValue(msgConv) as IList
                    ?? t.GetField("Categories", BindingFlags.Public | BindingFlags.Instance)?.GetValue(msgConv) as IList;
                convCats?.Clear();
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"GrantEllison.ClearConversationCategories: {ex.Message}");
            }
        }

        private void EnsureUnlockedIfPreviouslyIntroduced()
        {
            if (Relationship?.IsUnlocked == true) return;
            if (!MoreNPCsModSave.NpcUnlockIntroTexts.HasBeenSentFor(DistributorIds.Grant)
                && !MoreNPCsModSave.NpcUnlockIntroTexts.HasBeenSentFor(DistributorIds.LegacyVinny))
                return;

            Relationship?.Unlock(NPCRelationship.UnlockType.DirectApproach, notify: false);
        }

        protected override void OnDestroyed()
        {
            if (DistributorRegistry.Grant == this)
                DistributorRegistry.Grant = null;
            base.OnDestroyed();
        }
    }
}
