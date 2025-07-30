namespace FPTAlumniConnect.DataTier.Models
{
    public partial class TagJob
    {
        public int TagJobId { get; set; }

        public string? Tag { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public string? UpdatedBy { get; set; }

        public int? CvID { get; set; }

        public virtual Cv? Cv { get; set; }
    }
}
