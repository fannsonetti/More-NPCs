using System;
using System.Collections.Generic;
using MoreNPCs.Patches;
using ScheduleOne.Economy;
using ScheduleOne.UI.Handover;
using TMPro;
using UnityEngine;

namespace MoreNPCs.Utils
{
    /// <summary>
    /// Free-sample handover: keep the warning <see cref="TextMeshProUGUI"/> enabled and set copy from the **customer id**
    /// (not from slot counts). P.P. Hyland uses the 20-unit line; other NPCs use the default one-sample line unless listed in
    /// <see cref="SampleWarningTextByNpcId"/>.
    /// </summary>
    internal static class PPHylandHandoverWarning
    {
        private const string WarningRelativePath = "Container/Main/Warning";

        private const string DefaultNonPpSampleText = "Only 1 sample product is required.";

        private static string PpHylandSampleText =>
            $"{PpHylandSampleDifficulty.RequiredMethUnits} sample product is required.";

        /// <summary>Per–customer-id overrides for free-sample copy (prefab id → text). Empty → use <see cref="DefaultNonPpSampleText"/>.</summary>
        private static readonly Dictionary<string, string> SampleWarningTextByNpcId =
            new(StringComparer.OrdinalIgnoreCase);

        private const float MinSecondsBetweenPolls = 0.55f;

        private static HandoverScreen? _activeScreen;
        private static float _nextPollUnscaled = -1f;

        internal static void SetActiveScreen(HandoverScreen screen)
        {
            _activeScreen = screen;
            _nextPollUnscaled = -1f;
        }

        internal static void RefreshThrottled()
        {
            var screen = _activeScreen;
            if (screen == null || !screen)
            {
                _activeScreen = null;
                return;
            }

            if (!screen.gameObject.activeInHierarchy) return;

            var t = Time.unscaledTime;
            if (t < _nextPollUnscaled) return;
            _nextPollUnscaled = t + MinSecondsBetweenPolls;

            ApplyWarningUi(screen);
        }

        internal static void SyncWarning(HandoverScreen screen)
        {
            if (screen == null || !screen) return;
            ApplyWarningUi(screen);
        }

        private static void ApplyWarningUi(HandoverScreen screen)
        {
            var warningTr = screen.transform.Find(WarningRelativePath);
            if (warningTr == null) return;

            var tmp = warningTr.GetComponent<TextMeshProUGUI>();
            if (tmp == null) return;

            if (!PPHylandHandoverScreenHelper.IsSampleMode(screen))
                return;

            if (!warningTr.gameObject.activeSelf)
                warningTr.gameObject.SetActive(true);

            PPHylandHandoverScreenHelper.EnsureCustomerCache(screen);

            var customer = PPHylandHandoverScreenHelper.TryGetCustomer(screen);
            if (customer == null) return;

            tmp.enabled = true;
            tmp.text = ResolveSampleWarningText(customer);
        }

        private static string ResolveSampleWarningText(Customer customer)
        {
            if (PpHylandSampleDifficulty.IsPpHylandCustomer(customer))
                return PpHylandSampleText;

            var id = PPHylandHandoverScreenHelper.TryGetCustomerNpcId(customer);
            if (!string.IsNullOrEmpty(id) && SampleWarningTextByNpcId.TryGetValue(id, out var msg))
                return msg;

            return DefaultNonPpSampleText;
        }
    }
}
