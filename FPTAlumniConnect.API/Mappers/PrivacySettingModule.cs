using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.PrivacySetting;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class PrivacySettingModule: Profile
    {
        public PrivacySettingModule() 
        { 
            CreateMap<PrivacySetting, GetPrivacySettingResponse>();
            CreateMap<PrivacySettingInfo, PrivacySetting>();
        }
    }
}
