﻿namespace FPTAlumniConnect.BusinessTier.Payload.User
{
    public class UserFilter
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }
        public string? Code { get; set; }
        public bool? EmailVerified { get; set; }
        public string? ProfilePicture { get; set; }

        public int? RoleId { get; set; }

        public int? MajorId { get; set; }

        public string? GoogleId { get; set; }

        public bool? IsMentor { get; set; }
    }
}
