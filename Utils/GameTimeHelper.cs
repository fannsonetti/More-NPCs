using System;
using System.Reflection;
using S1API.GameTime;

namespace MoreNPCs.Utils
{
    internal static class GameTimeHelper
    {
        /// <summary>Current in-game clock as HHMM (e.g. 1400).</summary>
        public static int GetCurrentTime24()
        {
            try
            {
                var tm = typeof(TimeManager);
                foreach (var name in new[] { "CurrentTime", "CurrentLoopTime", "ElapsedTime" })
                {
                    var prop = tm.GetProperty(name, BindingFlags.Public | BindingFlags.Static);
                    if (prop == null) continue;
                    var val = prop.GetValue(null);
                    if (val == null) continue;
                    if (val is int i) return NormalizeTime24(i);
                    if (val is float f) return NormalizeTime24((int)f);
                }

                var totalMinutesProp = tm.GetProperty("TotalMins", BindingFlags.Public | BindingFlags.Static)
                    ?? tm.GetProperty("TotalMinutes", BindingFlags.Public | BindingFlags.Static);
                if (totalMinutesProp?.GetValue(null) is int mins)
                {
                    mins = ((mins % 1440) + 1440) % 1440;
                    return (mins / 60) * 100 + (mins % 60);
                }
            }
            catch { }

            return 900;
        }

        public static int ElapsedDays
        {
            get
            {
                try { return TimeManager.ElapsedDays; }
                catch { return 0; }
            }
        }

        public static int AddMinutesToTime24(int time24, int minutes)
        {
            int h = time24 / 100;
            int m = time24 % 100;
            int total = h * 60 + m + minutes;
            total = ((total % 1440) + 1440) % 1440;
            return (total / 60) * 100 + (total % 60);
        }

        public static string FormatTime24(int time24)
        {
            int h = time24 / 100;
            int m = time24 % 100;
            var suffix = h >= 12 ? "PM" : "AM";
            int h12 = h % 12;
            if (h12 == 0) h12 = 12;
            return $"{h12}:{m:D2} {suffix}";
        }

        public static int GetMinSum(int day, int time24)
        {
            int h = time24 / 100;
            int m = time24 % 100;
            return day * 1440 + h * 60 + m;
        }

        public static int GetCurrentMinSum() => GetMinSum(ElapsedDays, GetCurrentTime24());

        public static int GetMinutesUntil(int deadlineDay, int deadlineTime24)
        {
            return GetMinSum(deadlineDay, deadlineTime24) - GetCurrentMinSum();
        }

        private static int NormalizeTime24(int value)
        {
            if (value >= 0 && value < 2400) return value;
            if (value >= 0 && value < 1440)
            {
                int h = value / 60;
                int m = value % 60;
                return h * 100 + m;
            }
            return 900;
        }
    }
}
