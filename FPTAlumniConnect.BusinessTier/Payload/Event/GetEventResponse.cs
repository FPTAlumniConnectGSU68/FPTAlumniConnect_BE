namespace FPTAlumniConnect.BusinessTier.Payload.Event
{
    public class GetEventResponse
    {
        public int EventId { get; set; }

        public string EventName { get; set; } = null!;

        public string? Img { get; set; }

        public string? Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string? Location { get; set; }

        public int? OrganizerId { get; set; }

        public int? MajorId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? Status { get; set; }

        public string? CreatedBy { get; set; }

        public double? AverageRating { get; set; }

        public int UserJoinEventCount { get; set; }

    }
}
