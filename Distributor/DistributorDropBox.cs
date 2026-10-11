using System;
using System.Reflection;
using MelonLoader;
using MoreNPCs.NPCs;
using MoreNPCs.Quests;
using MoreNPCs.Utils;

namespace MoreNPCs.Distributor
{
    /// <summary>Tracks deposits in the docks pier locker and completes the deal.</summary>
    internal static class DistributorDropBox
    {
        private static object? _storage;
        private static bool _active;
        private static Action? _contentsChangedHandler;

        public static bool IsActive => _active && _storage != null;

        public static void Enable()
        {
            DistributorPlaceholderProps.ShowForVenue(DistributorVenue.DocksBoat);
            DistributorTrawlerDrop.EnsurePierDrop();

            if (!DistributorTrawlerDrop.TryResolve(out _storage, out _, out _))
            {
                MelonLogger.Warning("DistributorDropBox: could not create pier locker storage.");
                DistributorPlaceholderProps.HideAll();
                return;
            }

            HookStorageChanged(_storage);
            _active = true;
        }

        public static void Disable()
        {
            UnhookStorageChanged(_storage);
            _active = false;
            DistributorTrawlerDrop.Destroy();
            DistributorPlaceholderProps.HideAll();
        }

        public static void CheckCompletion()
        {
            if (!_active || _storage == null || DistributorRunSave.SoldThisRun) return;
            if (!DistributorRunSave.IsDealActive()) return;
            if (DistributorStorageSellHelper.IsPlayerAccessing(_storage)) return;

            DistributorWeeklyOrder.EnsureCurrentWeekOrder();
            var drug = DistributorWeeklyOrder.OrderedDrug;
            int target = DistributorWeeklyOrder.TargetQuantity;
            int deposited = DistributorStorageSellHelper.CountDrugUnits(_storage, drug);
            if (deposited < target) return;

            var sale = DistributorStorageSellHelper.TrySellFromStorage(
                _storage,
                drug,
                target,
                DistributorWeeklyOrder.OrderKind);

            if (sale.TotalPayout <= 0f || sale.UnitsSold <= 0)
            {
                SendContactSms(DistributorText.DealFailedSms());
                FailDeal();
                return;
            }

            PlayerBankHelper.TryDepositOnline(sale.TotalPayout, "Wholesale Payment", DistributorIds.DisplayName);
            DistributorRunSave.MarkSoldThisRun();
            DistributorQuestManager.CompleteDeal();
            Disable();

            string extraLine = sale.OverflowUnitsSold > 0
                ? DistributorText.ExtraUnitsLine(sale.OverflowUnitsSold)
                : string.Empty;
            SendContactSms(DistributorWeeklyOrder.OrderKind == DistributorOrderKind.PremiumProperties
                ? DistributorText.PremiumCompletedSms(sale.TotalPayout, extraLine)
                : DistributorText.BulkCompletedSms(sale.TotalPayout, extraLine));
        }

        internal static void FailDeal()
        {
            DistributorQuestManager.FailDeal();
            Disable();
            DistributorRunSave.ClearRun();
        }

        private static void HookStorageChanged(object? storage)
        {
            if (storage == null) return;
            UnhookStorageChanged(storage);

            try
            {
                _contentsChangedHandler = OnStorageContentsChanged;
                var field = storage.GetType().GetField(
                    "onContentsChanged",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field == null) return;

                var current = field.GetValue(storage) as Action;
                field.SetValue(storage, Delegate.Combine(current, _contentsChangedHandler));
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"DistributorDropBox.HookStorageChanged: {ex.Message}");
            }
        }

        private static void UnhookStorageChanged(object? storage)
        {
            if (storage == null || _contentsChangedHandler == null) return;

            try
            {
                var field = storage.GetType().GetField(
                    "onContentsChanged",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field == null) return;

                var current = field.GetValue(storage) as Action;
                field.SetValue(storage, Delegate.Remove(current, _contentsChangedHandler));
            }
            catch { }

            _contentsChangedHandler = null;
        }

        private static void OnStorageContentsChanged()
        {
            if (!_active) return;
            CheckCompletion();
        }

        private static void SendContactSms(string message)
        {
            var contact = DistributorRegistry.GrantNpc;
            contact?.SendTextMessage(message);
        }
    }
}
