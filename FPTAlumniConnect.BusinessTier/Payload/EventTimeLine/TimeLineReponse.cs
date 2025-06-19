using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPTAlumniConnect.BusinessTier.Payload.EventTimeLine
{
    public class TimeLineReponse
    {
        public int EventTimeLineId { get; set; }
        public int EventId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
