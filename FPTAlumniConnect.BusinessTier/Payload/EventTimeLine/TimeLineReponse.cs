namespace FPTAlumniConnect.BusinessTier.Payload.EventTimeLine
{
    public class TimeLineReponse
    {
        public int EventTimeLineId { get; set; }
        public int EventId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string StartTime { get; set; } // Format: HH:mm:ss
        public string EndTime { get; set; } // Format: HH:mm:ss
    }
}
