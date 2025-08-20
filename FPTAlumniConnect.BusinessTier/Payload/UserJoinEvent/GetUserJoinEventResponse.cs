namespace FPTAlumniConnect.BusinessTier.Payload.UserJoinEvent
{
    public class GetUserJoinEventResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty; // Combined FirstName + LastName from User
        public string? Avatar { get; set; } // ProfilePicture from User
        public string Role { get; set; } = string.Empty; // Role.RoleName from User.Role
        public string? Code { get; set; } // Code from User
        public int EventId { get; set; }
        public string? Content { get; set; }
        public int? Rating { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }

}
