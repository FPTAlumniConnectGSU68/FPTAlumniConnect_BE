using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPTAlumniConnect.BusinessTier.Payload.User
{
    public class MentorFilter
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public bool? EmailVerified { get; set; }

        public int? RoleId { get; set; }

        public int? MajorId { get; set; }

        public double Rating { get; set; }
    }
}
