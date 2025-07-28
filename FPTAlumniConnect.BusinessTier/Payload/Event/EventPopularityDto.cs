using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPTAlumniConnect.BusinessTier.Payload.Event
{
    public class EventPopularityDto
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public int ParticipantCount { get; set; }
        public float PopularityScore { get; set; }
    }
}
