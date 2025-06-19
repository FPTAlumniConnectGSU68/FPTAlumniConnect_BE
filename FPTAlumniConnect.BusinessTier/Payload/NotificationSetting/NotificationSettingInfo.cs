namespace FPTAlumniConnect.BusinessTier.Payload.NotificationSetting
{
    public class NotificationSettingInfo
    {
        public int? UserId { get; set; }
        public bool? ReceiveEmailNotifications { get; set; }
        public bool? ReceiveInAppNotifications { get; set; }
        public bool? JobNotifications { get; set; }
        public bool? MessageNotifications { get; set; }
    }
}