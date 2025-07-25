﻿namespace FPTAlumniConnect.BusinessTier.Payload.GroupChat
{
    public class GroupChatReponse
    {
        public int Id { get; set; }

        public string RoomName { get; set; } = null!;

        public int? CreatedBy { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? UpdatedBy { get; set; }

    }
}
