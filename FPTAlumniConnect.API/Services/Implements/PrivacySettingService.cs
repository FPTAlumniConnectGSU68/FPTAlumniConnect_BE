using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.PrivacySetting;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class PrivacySettingService : BaseService<PrivacySettingService>, IPrivacySettingService
    {
        public PrivacySettingService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<PrivacySettingService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<int> CreatePrivacySetting(PrivacySettingInfo request)
        {
            var newPrivacySetting = _mapper.Map<PrivacySetting>(request);
            newPrivacySetting.CreatedAt = DateTime.Now;
            newPrivacySetting.CreatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            await _unitOfWork.GetRepository<PrivacySetting>().InsertAsync(newPrivacySetting);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newPrivacySetting.Id;
        }

        public async Task<GetPrivacySettingResponse> GetPrivacySettingById(int id)
        {
            PrivacySetting privacySetting = await _unitOfWork.GetRepository<PrivacySetting>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id)) ??
                throw new BadHttpRequestException("PrivacySettingNotFound");

            GetPrivacySettingResponse result = _mapper.Map<GetPrivacySettingResponse>(privacySetting);
            return result;
        }

        public async Task<bool> UpdatePrivacySetting(int id, PrivacySettingInfo request)
        {
            PrivacySetting privacySetting = await _unitOfWork.GetRepository<PrivacySetting>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id)) ??
                throw new BadHttpRequestException("PrivacySettingNotFound");
            privacySetting.VisibleToEducationHistory = request.VisibleToEducationHistory ?? privacySetting.VisibleToEducationHistory;
            privacySetting.VisibleToMajor = request.VisibleToMajor ?? privacySetting.VisibleToMajor;
            privacySetting.VisibleToEmail = request.VisibleToEmail ?? privacySetting.VisibleToEmail;
            privacySetting.VisibleToAlumni = request.VisibleToAlumni ?? privacySetting.VisibleToAlumni;
            privacySetting.UpdatedAt = DateTime.Now;
            privacySetting.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<PrivacySetting>().UpdateAsync(privacySetting);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<IPaginate<GetPrivacySettingResponse>> ViewAllPrivacySettings(PrivacySettingFilter filter, PagingModel pagingModel)
        {
            IPaginate<GetPrivacySettingResponse> response = await _unitOfWork.GetRepository<PrivacySetting>().GetPagingListAsync(
                selector: x => _mapper.Map<GetPrivacySettingResponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
                );
            return response;
        }
    }
}