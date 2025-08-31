namespace FPTAlumniConnect.BusinessTier.Payload.User
{
    public class UserInfo
    {
        public string? Code { get; set; }
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string IsMentor { get; set; }
        public string? ProfilePicture { get; set; }

    }
}