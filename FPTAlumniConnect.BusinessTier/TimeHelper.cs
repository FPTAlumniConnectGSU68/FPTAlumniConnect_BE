namespace FPTAlumniConnect.API
{
    public static class TimeHelper
    {
        private static readonly TimeZoneInfo VietnamTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        public static DateTime NowInVietnam()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTimeZone);
        }
    }
}
