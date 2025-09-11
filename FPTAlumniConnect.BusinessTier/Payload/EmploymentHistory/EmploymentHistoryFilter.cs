namespace FPTAlumniConnect.BusinessTier.Payload.EmploymentHistory
{
    public class EmploymentHistoryFilter
    {
        public string? CompanyName { get; set; }
        public string? JobLevel { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsCurrentJob { get; set; }
    }
}