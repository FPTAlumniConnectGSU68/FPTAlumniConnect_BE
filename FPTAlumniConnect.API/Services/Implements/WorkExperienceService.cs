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
    // Service for managing user work experiences
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

        // Create a new work experience record
        public async Task<int> CreateWorkExperienceAsync(WorkExperienceInfo request)
        {
            if (request.EndDate.HasValue && request.StartDate > request.EndDate.Value)
            {
                throw new BadHttpRequestException("StartDate must be before EndDate");
            }

            WorkExperience newWorkExperience = _mapper.Map<WorkExperience>(request);
            await _unitOfWork.GetRepository<WorkExperience>().InsertAsync(newWorkExperience);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newWorkExperience.Id;
        }

        // Get a work experience record by ID
        public async Task<WorkExperienceResponse> GetWorkExperienceByIdAsync(int id)
        {
            WorkExperience workExperience = await _unitOfWork.GetRepository<WorkExperience>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id)) ??
                throw new BadHttpRequestException("WorkExperienceNotFound");

            WorkExperienceResponse response = _mapper.Map<WorkExperienceResponse>(workExperience);
            return response;
        }

        // Update an existing work experience record
        public async Task<bool> UpdateWorkExperienceAsync(int id, WorkExperienceInfo request)
        {
            WorkExperience workExperience = await _unitOfWork.GetRepository<WorkExperience>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id)) ??
                throw new BadHttpRequestException("WorkExperienceNotFound");

            UpdateWorkExperienceFields(workExperience, request);

            _unitOfWork.GetRepository<WorkExperience>().UpdateAsync(workExperience);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        // Delete a work experience record by ID
        public async Task<bool> DeleteWorkExperienceAsync(int id)
        {
            WorkExperience workExperience = await _unitOfWork.GetRepository<WorkExperience>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id)) ??
                throw new BadHttpRequestException("WorkExperienceNotFound");

            _unitOfWork.GetRepository<WorkExperience>().DeleteAsync(workExperience);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        // View all work experiences with filter and pagination
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

        // Search work experiences by keyword (company name or position)
        public async Task<IEnumerable<WorkExperienceResponse>> SearchWorkExperienceAsync(string keyword)
        {
            var list = await _unitOfWork.GetRepository<WorkExperience>().GetListAsync(
                predicate: x => (x.CompanyName.Contains(keyword) || x.Position.Contains(keyword)),
                selector: x => _mapper.Map<WorkExperienceResponse>(x)
            );

            return list;
        }

        // Helper method to update allowed fields
        private void UpdateWorkExperienceFields(WorkExperience we, WorkExperienceInfo request)
        {
            we.CompanyName = string.IsNullOrEmpty(request.CompanyName) ? we.CompanyName : request.CompanyName;
            we.Position = string.IsNullOrEmpty(request.Position) ? we.Position : request.Position;
            we.StartDate = request.StartDate == default ? we.StartDate : request.StartDate;
            we.EndDate = request.EndDate ?? we.EndDate;
            we.CompanyWebsite = string.IsNullOrEmpty(request.CompanyWebsite) ? we.CompanyWebsite : request.CompanyWebsite;
            we.Location = string.IsNullOrEmpty(request.Location) ? we.Location : request.Location;
            we.LogoUrl = request.LogoUrl ?? we.LogoUrl;
            we.UserId = request.UserId;

            // Optional: track update metadata
            // we.UpdatedAt = DateTime.Now;
            // we.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;
        }
    }
}
