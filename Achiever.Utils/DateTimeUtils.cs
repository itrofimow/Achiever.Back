using System;
using System.Globalization;

namespace Achiever.Utils
{
    public static class DateTimeUtils
    {
        public static DateTime ParseIso8601(string date)
        {
            return DateTime.Parse(date, null, DateTimeStyles.RoundtripKind);
        }
    }
}