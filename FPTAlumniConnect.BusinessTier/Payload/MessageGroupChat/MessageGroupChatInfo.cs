using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPTAlumniConnect.BusinessTier.Payload.MessageGroupChat
{
    public class MessageGroupChatInfo
    {
        public int? MemberId { get; set; } 
        public string Content { get; set; } = null!; 
        public DateTime? CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; } 
        public string? CreatedBy { get; set; } 
        public string? UpdatedBy { get; set; } 
    }
}
