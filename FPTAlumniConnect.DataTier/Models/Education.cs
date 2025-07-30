namespace FPTAlumniConnect.DataTier.Models
{
    public class Education
    {
        public int Id { get; set; }
        public string SchoolName { get; set; }
        public string Major { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string SchoolWebsite { get; set; }
        public string Achievements { get; set; }
        public string Location { get; set; }
        public string LogoUrl { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;
    }
}
