using System;

namespace KinoDnes.Time
{
    public static class TimeHelper
    {
        public static DateTime CESTTimeNow
        {
            get
            {
                var cestTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Central European Standard Time");
                return cestTime;
            }
        }

        public static TimeSpan SecondsUntilNextMidnight
        {
            get
            {
                var timeNow = CESTTimeNow;
                var timeSpanUntilNextMidnight = timeNow.Date.AddDays(1) - timeNow;
                return timeSpanUntilNextMidnight;
            }
        }
    }
}