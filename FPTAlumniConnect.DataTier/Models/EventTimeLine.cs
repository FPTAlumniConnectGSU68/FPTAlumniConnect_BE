using System.ComponentModel.DataAnnotations;

namespace FPTAlumniConnect.DataTier.Models
{
    public partial class EventTimeLine
    {
        [Key]
        public int EventTimeLineId { get; set; }
        public int EventId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public virtual Event Event { get; set; }
    }
}
