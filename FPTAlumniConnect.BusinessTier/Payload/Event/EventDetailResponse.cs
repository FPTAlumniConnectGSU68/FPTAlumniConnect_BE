using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPTAlumniConnect.BusinessTier.Payload.Event
{
    public class EventDetailResponse
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = null!;
        public string? Img { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Location { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public int? OrganizerId { get; set; }
        public string? OrganizerName { get; set; }
        public int? MajorId { get; set; }
        public string? MajorName { get; set; }

        public double? AverageRating { get; set; }
        public int? Total { get; set; }

        public List<EventTimeLineResponse> EventTimeLines { get; set; } = new();
    }

    public class EventTimeLineResponse
    {
        public int EventTimeLineId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
