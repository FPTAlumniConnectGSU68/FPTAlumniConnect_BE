namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface IMentorshipSettingsService
    {
        int GetMaxPerDay();
        void UpdateMaxPerDay(int newValue);
    }
}
