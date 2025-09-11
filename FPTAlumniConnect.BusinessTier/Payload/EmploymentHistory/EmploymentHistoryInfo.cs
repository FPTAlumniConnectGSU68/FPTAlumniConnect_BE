namespace FPTAlumniConnect.BusinessTier.Payload.EmploymentHistory
{
    public class EmploymentHistoryInfo
    {
        public int CvId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string PrimaryDuties { get; set; } = null!;
        public string JobLevel { get; set; } = null!;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrentJob { get; set; }
    }
}