using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload.Schedule;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class ScheduleSettingsService : IScheduleSettingsService
    {
        private readonly ScheduleSettings _settings;

        public ScheduleSettingsService(ScheduleSettings settings)
        {
            _settings = settings;
        }

        public int GetMaxPerDay() => _settings.MaxPerDay;

        public void UpdateMaxPerDay(int newValue)
        {
            if (newValue <= 0)
                throw new ArgumentException("MaxPerDay must be greater than 0");

            _settings.MaxPerDay = newValue;
        }
    }
}
