using FPTAlumniConnect.BusinessTier.Payload.PrivacySetting;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.BusinessTier.Payload;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface IPrivacySettingService
    {
        Task<int> CreatePrivacySetting(PrivacySettingInfo request);
        Task<IPaginate<GetPrivacySettingResponse>> ViewAllPrivacySettings(PrivacySettingFilter filter, PagingModel pagingModel);
        Task<bool> UpdatePrivacySetting(int id, PrivacySettingInfo request);
        Task<GetPrivacySettingResponse> GetPrivacySettingById(int id);
    }
}