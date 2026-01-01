using System;

namespace NazarMahal.Core.Common
{
    public static class PakistanTimeHelper
    {
        /// <summary>
        /// Gets the current time in Pakistan Standard Time (PKT - UTC+5)
        /// </summary>
        public static DateTime Now => DateTime.UtcNow.AddHours(5);

        /// <summary>
        /// Converts UTC time to Pakistan Standard Time
        /// </summary>
        public static DateTime ToPakistanTime(DateTime utcDateTime)
        {
            return utcDateTime.AddHours(5);
        }

        /// <summary>
        /// Converts Pakistan time to UTC
        /// </summary>
        public static DateTime ToUtcTime(DateTime pakistanDateTime)
        {
            return pakistanDateTime.AddHours(-5);
        }

        /// <summary>
        /// Gets the Pakistan timezone info
        /// </summary>
        public static TimeZoneInfo PakistanTimeZone => TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
    }
}

