using System;
using NodaTime;

namespace KinoDnesApi
{
    public static class CetTime
    {
        public static DateTime Now
        {
            get
            {
                var pragueTimeZone = DateTimeZoneProviders.Tzdb["Europe/Prague"];
                return Instant.FromDateTimeUtc(DateTime.UtcNow)
                    .InZone(pragueTimeZone)
                    .ToDateTimeUnspecified();
            }
        }
    }
}