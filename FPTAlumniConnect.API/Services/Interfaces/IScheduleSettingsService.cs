namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface IScheduleSettingsService
    {
        int GetMaxPerDay();
        void UpdateMaxPerDay(int newValue);
    }
}
