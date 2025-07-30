namespace FPTAlumniConnect.DataTier.Models
{
    public class WorkExperience
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string CompanyWebsite { get; set; }
        public string Location { get; set; }
        public string LogoUrl { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;
    }
}
