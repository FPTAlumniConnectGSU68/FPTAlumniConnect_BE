using FirebaseAdmin.Auth;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface IFirebaseService
    {
        Task<FirebaseToken?> VerifyIdToken(string token);
    }
}
