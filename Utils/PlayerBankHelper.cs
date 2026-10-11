using MelonLoader;
using ScheduleOne.DevUtilities;
using ScheduleOne.Money;

namespace MoreNPCs.Utils
{
    /// <summary>Deposits payout to the player's online / credit card balance.</summary>
    internal static class PlayerBankHelper
    {
        public static bool HasOnlineBalance(float amount)
        {
            if (amount <= 0f) return true;
            try
            {
                if (!NetworkSingleton<MoneyManager>.InstanceExists)
                    return false;
                return NetworkSingleton<MoneyManager>.Instance.onlineBalance >= amount;
            }
            catch
            {
                return false;
            }
        }

        public static bool TryChargeOnline(float amount, string transactionName, string note = "")
        {
            if (amount <= 0f) return true;

            try
            {
                if (!NetworkSingleton<MoneyManager>.InstanceExists)
                {
                    MelonLogger.Warning("PlayerBankHelper: MoneyManager not ready.");
                    return false;
                }

                var money = NetworkSingleton<MoneyManager>.Instance;
                if (money.onlineBalance < amount)
                    return false;

                money.CreateOnlineTransaction(
                    transactionName,
                    -amount,
                    1f,
                    note ?? string.Empty);
                MelonLogger.Msg($"PlayerBankHelper: charged ${amount:N0} to card ({transactionName}).");
                return true;
            }
            catch (System.Exception ex)
            {
                MelonLogger.Warning($"PlayerBankHelper.TryChargeOnline: {ex.Message}");
                return false;
            }
        }

        public static bool TryDepositOnline(float amount, string transactionName, string note = "")
        {
            if (amount <= 0f) return true;

            try
            {
                if (!NetworkSingleton<MoneyManager>.InstanceExists)
                {
                    MelonLogger.Warning("PlayerBankHelper: MoneyManager not ready.");
                    return false;
                }

                NetworkSingleton<MoneyManager>.Instance.CreateOnlineTransaction(
                    transactionName,
                    amount,
                    1f,
                    note ?? string.Empty);
                MelonLogger.Msg($"PlayerBankHelper: deposited ${amount:N0} to card ({transactionName}).");
                return true;
            }
            catch (System.Exception ex)
            {
                MelonLogger.Warning($"PlayerBankHelper.TryDepositOnline: {ex.Message}");
                return false;
            }
        }
    }
}
