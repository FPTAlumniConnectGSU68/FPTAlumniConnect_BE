﻿namespace FPTAlumniConnect.BusinessTier.Payload.Event
{
    public class EventInfo
    {
        public string? EventName { get; set; } = null!;

        public string? Img { get; set; }

        public string? Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? OrganizerId { get; set; }

        public int? MajorId { get; set; }

        public string? Location { get; set; }

        public string? Status { get; set; }
    }

}
