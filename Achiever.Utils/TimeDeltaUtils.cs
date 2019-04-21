using System;
using System.Collections.Generic;

namespace Achiever.Utils
{
    public static class TimeDeltaUtils
    {
        private static readonly HashSet<int>
            MinutU = new HashSet<int> {1, 21, 31, 41, 51},
            MinutY = new HashSet<int> {2, 3, 4, 22, 23, 24, 32, 33, 34, 42, 43, 44, 52, 53, 54};

        private static readonly HashSet<int>
            Hour = new HashSet<int> {1, 21},
            HourA = new HashSet<int> {2, 3, 4, 22, 23};

        private static readonly List<string> Months = new List<string>
        {
            "",
            "Января", "Февраля", "Марта",
            "Апреля", "Мая", "Июня",
            "Июля", "Августа", "Сентября",
            "Октября", "Ноября", "Декабря",
        };

        public static string ToHumanReadable(DateTime time, TimeSpan delta)
        {
            #region Seconds

            if (delta.TotalSeconds < 30) return "только что";

            if (delta.TotalSeconds < 60) return $"{((int) delta.TotalSeconds) / 5 * 5} секунд назад";

            #endregion

            #region Minutes

            var d = (int) delta.TotalMinutes;
            if (d >= 1 && d < 60)
            {
                if (MinutU.Contains(d)) return $"{d} минуту назад";
                if (MinutY.Contains(d)) return $"{d} минуты назад";

                return $"{d} минут назад";
            }

            #endregion

            #region Hours

            var h = (int) delta.TotalHours;
            if (h >= 1 && h < 24)
            {
                if (Hour.Contains(h)) return $"{h} час назад";
                if (HourA.Contains(h)) return $"{h} часа назад";

                return $"{h} часов назад";
            }

            if (h >= 24 && h < 48) return "вчера";

            #endregion

            return $"{time.Day} {Months[time.Month]} {time.Year}";
        }
    }
}