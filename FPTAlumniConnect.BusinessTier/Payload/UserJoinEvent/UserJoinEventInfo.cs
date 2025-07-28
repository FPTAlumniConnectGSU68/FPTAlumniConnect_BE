namespace FPTAlumniConnect.BusinessTier.Payload.UserJoinEvent
{
    public class UserJoinEventInfo
    {
        public int UserId { get; set; }
        public int EventId { get; set; }
        public string? Content { get; set; }
        public int? Rating { get; set; }
        //public string? CreatedBy { get; set; }
    }

}
