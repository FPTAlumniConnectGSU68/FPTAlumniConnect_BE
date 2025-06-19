using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPTAlumniConnect.BusinessTier.Payload.Event
{
    public class EventFilter
    {
        public string? EventName { get; set; }

        public string? Img { get; set; }

        public string? Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? OrganizerId { get; set; }

        public string? Location { get; set; }
    }
}
