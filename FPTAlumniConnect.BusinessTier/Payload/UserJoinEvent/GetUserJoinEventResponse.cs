﻿namespace FPTAlumniConnect.BusinessTier.Payload.UserJoinEvent
{
    public class GetUserJoinEventResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }
        public string? Content { get; set; }
        public int? Rating { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
