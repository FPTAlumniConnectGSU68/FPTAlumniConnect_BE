using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Configurations;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class MentorshipSettingsService : IMentorshipSettingsService
    {
        private readonly MentorshipSettings _settings;

        public MentorshipSettingsService(MentorshipSettings settings)
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
