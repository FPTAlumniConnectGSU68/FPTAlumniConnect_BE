namespace FPTAlumniConnect.BusinessTier.Payload.Event
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

        public List<EventTimeLineInfo>? TimeLines { get; set; }
    }

    public class EventTimeLineInfo
    {
        public int? EventTimeLineId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }

        // client sends "09:00" or "09:00:00" etc.
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
    }
}
