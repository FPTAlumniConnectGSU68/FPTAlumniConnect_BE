using FirebaseAdmin;
using FirebaseAdmin.Auth;
using FPTAlumniConnect.API.Services.Interfaces;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class FirebaseService : IFirebaseService
    {
        FirebaseApp _firebaseApp;
        FirebaseAuth _firebaseAuth;

        public FirebaseService()
        {
            _firebaseApp = FirebaseApp.GetInstance("[DEFAULT]");
            _firebaseAuth = FirebaseAuth.GetAuth(_firebaseApp);
        }

        public async Task<FirebaseToken?> VerifyIdToken(string token)
        {
            FirebaseToken? result = null;
            try
            {
                result = await _firebaseAuth.VerifyIdTokenAsync(token);
            }
            catch
            {
                result = null;
            }
            return result;
        }
    }
}
