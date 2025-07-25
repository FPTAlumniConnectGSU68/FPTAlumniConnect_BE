namespace FPTAlumniConnect.BusinessTier.Payload.Notification
{
    public class NotificationResponse
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }
    }
}
