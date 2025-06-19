using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPTAlumniConnect.BusinessTier.Payload.User
{
    public class GetUserResponse
    {
        public int UserId { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public bool? EmailVerified { get; set; }
        public string? ProfilePicture { get; set; }

        public int? RoleId { get; set; }
        public string? RoleName { get; set; }

        public int? MajorId { get; set; }
        public string? MajorName { get; set; }

        public string? GoogleId { get; set; }

        public bool? IsMentor { get; set; }
    }
}