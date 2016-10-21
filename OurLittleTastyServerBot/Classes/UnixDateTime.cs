using System;

namespace OurLittleTastyServerBot.Classes
{
    public static class UnixDateTime
    {
        private static readonly DateTime UnixDatetimeZero = new DateTime(1970, 1, 1);

        public static DateTime FromUnixFormat(Int64 unixDateTime)
        {
            var time = UnixDatetimeZero.AddSeconds(unixDateTime);
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(time, TimeZoneInfo.Local);
            return localTime;
        }

        public static Int64 ToUnixFormat(DateTime dateTime)
        {
            var utcTime = TimeZoneInfo.ConvertTimeToUtc(dateTime);
            var unixUtcTime = (Int64)utcTime.Subtract(UnixDatetimeZero).TotalSeconds;
            return unixUtcTime;
        }
    }
}