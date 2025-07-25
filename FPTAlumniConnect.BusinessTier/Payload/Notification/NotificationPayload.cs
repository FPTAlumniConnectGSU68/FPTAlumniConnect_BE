﻿namespace FPTAlumniConnect.BusinessTier.Payload.Notification
{
    public class NotificationPayload
    {
        public int UserId { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public bool IsRead { get; set; } = false;
    }
}
