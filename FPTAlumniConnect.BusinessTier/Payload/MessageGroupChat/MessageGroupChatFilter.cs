using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPTAlumniConnect.BusinessTier.Payload.MessageGroupChat
{
    public class MessageGroupChatFilter
    {
        public int? MemberId { get; set; }
        public string? Content { get; set; }
    }
}
