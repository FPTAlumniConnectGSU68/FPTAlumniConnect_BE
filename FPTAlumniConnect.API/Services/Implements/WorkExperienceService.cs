using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.Post;
using FPTAlumniConnect.BusinessTier.Payload.WorkExperience;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class WorkExperienceService : BaseService<WorkExperienceService>, IWorkExperienceService
    {
        public WorkExperienceService(
            IUnitOfWork<AlumniConnectContext> unitOfWork,
            ILogger<WorkExperienceService> logger,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<int> CreateWorkExperienceAsync(WorkExperienceInfo request)
        {
            WorkExperience newWorkExperience = _mapper.Map<WorkExperience>(request);
            await _unitOfWork.GetRepository<WorkExperience>().InsertAsync(newWorkExperience);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newWorkExperience.Id;
        }

        public async Task<WorkExperienceResponse> GetWorkExperienceByIdAsync(int id)
        {
            WorkExperience workExperience = await _unitOfWork.GetRepository<WorkExperience>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id)) ??
                throw new BadHttpRequestException("WorkExperienceNotFound");

            WorkExperienceResponse response = _mapper.Map<WorkExperienceResponse>(workExperience);
            return response;
        }

        public async Task<bool> UpdateWorkExperienceAsync(int id, WorkExperienceInfo request)
        {
            WorkExperience workExperience = await _unitOfWork.GetRepository<WorkExperience>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id)) ??
                throw new BadHttpRequestException("WorkExperienceNotFound");

            workExperience.CompanyName = string.IsNullOrEmpty(request.CompanyName) ? workExperience.CompanyName : request.CompanyName;
            workExperience.Position = string.IsNullOrEmpty(request.Position) ? workExperience.Position : request.Position;
            workExperience.StartDate = request.StartDate == default ? workExperience.StartDate : request.StartDate;
            workExperience.EndDate = request.EndDate ?? workExperience.EndDate;
            workExperience.CompanyWebsite = string.IsNullOrEmpty(request.CompanyWebsite) ? workExperience.CompanyWebsite : request.CompanyWebsite;
            workExperience.Location = string.IsNullOrEmpty(request.Location) ? workExperience.Location : request.Location;
            workExperience.LogoUrl = request.LogoUrl ?? workExperience.LogoUrl;
            workExperience.UserId = request.UserId;


            _unitOfWork.GetRepository<WorkExperience>().UpdateAsync(workExperience);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<bool> DeleteWorkExperienceAsync(int id)
        {
            WorkExperience workExperience = await _unitOfWork.GetRepository<WorkExperience>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id)) ??
                throw new BadHttpRequestException("WorkExperienceNotFound");

            _unitOfWork.GetRepository<WorkExperience>().DeleteAsync(workExperience);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<IPaginate<WorkExperienceResponse>> ViewAllWorkExperiencesAsync(WorkExperienceFilter filter, PagingModel pagingModel)
        {
            IPaginate<WorkExperienceResponse> response = await _unitOfWork.GetRepository<WorkExperience>().GetPagingListAsync(
                selector: x => _mapper.Map<WorkExperienceResponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(x => x.StartDate),
                page: pagingModel.page,
                size: pagingModel.size
                );
            return response;
        }


    }
}