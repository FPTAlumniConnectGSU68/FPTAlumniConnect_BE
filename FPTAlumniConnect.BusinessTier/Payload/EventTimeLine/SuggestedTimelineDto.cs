using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPTAlumniConnect.BusinessTier.Payload.EventTimeLine
{
    public class SuggestedTimelineDto
    {
        public string Title { get; set; }
        public string Speaker { get; set; }
        public DateTime Day { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Description { get; set; }
    }

}
