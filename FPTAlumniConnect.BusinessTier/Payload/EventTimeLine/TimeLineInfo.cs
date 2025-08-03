namespace FPTAlumniConnect.BusinessTier.Payload.EventTimeLine
{
    public class TimeLineInfo
    {
        public int EventId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime StartTime { get; set; } // Được tự động chuyển đổi từ chuỗi
        public DateTime EndTime { get; set; }   // Được tự động chuyển đổi từ chuỗi
    }
}
