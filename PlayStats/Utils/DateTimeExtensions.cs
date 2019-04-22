using System;

namespace PlayStats.Utils
{
    public static class DateTimeExtensions
    {
        public static bool IsSameDay(this DateTime obj, DateTime other)
        {
            return obj.Year == other.Year && obj.Month == other.Month && obj.Day == other.Day;
        }
    }
}
