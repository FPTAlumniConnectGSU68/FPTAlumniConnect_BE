namespace FPTAlumniConnect.BusinessTier.Payload.EventTimeLine
{
    public class TimeLineInfo
    {
        public int EventId { get; set; } // Required: ID of the event
        public string? Title { get; set; } // Optional: 3-100 characters
        public string? Description { get; set; } // Optional: max 1000 characters
        public string StartTime { get; set; } // Required: Format HH:mm or HH:mm:ss
        public string EndTime { get; set; } // Required: Format HH:mm or HH:mm:ss
    }
}
