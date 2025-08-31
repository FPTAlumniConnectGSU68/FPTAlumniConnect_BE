namespace FPTAlumniConnect.BusinessTier.Payload.User
{
    public class LoginResponse
    {
        public string Message { get; set; }
        public string AccessToken { get; set; }
        public UserResponse UserInfo { get; set; }
    }
    public class RegisterResponse
    {
        public int UserId { get; set; }
        public string? Code { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UserResponse
    {
        public int UserId { get; set; }
        public string Email { get; set; } = null!;
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string? IsMentor { get; set; }
        public string? GoogleId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

    }
}