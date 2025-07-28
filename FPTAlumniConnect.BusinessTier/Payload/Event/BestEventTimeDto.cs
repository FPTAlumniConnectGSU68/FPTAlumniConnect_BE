using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPTAlumniConnect.BusinessTier.Payload.Event
{
    public class BestEventTimeDto
    {
        public DateTime SuggestedTime { get; set; }
        public int Score { get; set; }
        public Dictionary<DateTime, int> AlternativeTimes { get; set; }
    }
}
