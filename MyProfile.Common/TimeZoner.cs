using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Common
{
    public static class TimeZoner
    {
        public static DateTime GetCurrentDateTimeWithRusTimeZone()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"));
        }

        public static DateTime GetCurrentDateTimeUTC()
        {
            return DateTime.Now.ToUniversalTime();
        }
    }
}
