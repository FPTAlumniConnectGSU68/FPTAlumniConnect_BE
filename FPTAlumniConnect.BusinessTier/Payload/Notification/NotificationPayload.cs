
using FPTAlumniConnect.API;

namespace FPTAlumniConnect.BusinessTier.Payload.Notification
{
    public class NotificationPayload
    {
        public int UserId { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; } = TimeHelper.NowInVietnam();
        public bool IsRead { get; set; } = false;
    }
}
