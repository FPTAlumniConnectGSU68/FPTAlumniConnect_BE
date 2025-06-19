using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPTAlumniConnect.BusinessTier.Payload.UserJoinEvent
{
    public class UserJoinEventFilter
    {
        public int? UserId { get; set; }
        public int? EventId { get; set; }
        public int? Rating { get; set; }
    }

}
