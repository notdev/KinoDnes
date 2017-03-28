using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KinoDnes.Cache
{
    /// <summary>
    /// All caches are valid exactly until midnight of 
    /// </summary>
    public static class CacheTimeHelper
    {
        private const string TimeZone = "Central Europe Standard Time";

        public static DateTime CurrentCzTime => TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, TimeZone);
        public static DateTime NextCzechMidnight => CurrentCzTime.Date.AddDays(1);
        public static TimeSpan SecondsUntilNextMidnight
        {
            get
            {
                var timeSpanUntilNextMidnight = NextCzechMidnight - CurrentCzTime;
                return timeSpanUntilNextMidnight;
            }
        }
    }
}