using System;

namespace KinoDnesApi
{
    public static class CetTime
    {
        private const string TimeZone = "Central Europe Standard Time";

        public static DateTime Now => TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, TimeZone);
    }
}