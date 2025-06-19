using System;

namespace FPTAlumniConnect.BusinessTier.Payload.NotificationSetting
{
    public class GetNotificationSettingResponse
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public bool? ReceiveEmailNotifications { get; set; }
        public bool? ReceiveInAppNotifications { get; set; }
        public bool? JobNotifications { get; set; }
        public bool? MessageNotifications { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
