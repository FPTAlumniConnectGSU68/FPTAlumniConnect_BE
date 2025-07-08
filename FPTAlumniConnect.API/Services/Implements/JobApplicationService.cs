using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.JobApplication;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class JobApplicationService : BaseService<JobApplicationService>, IJobApplicationService
    {
        public JobApplicationService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<JobApplicationService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<int> CreateNewJobApplication(JobApplicationInfo request)
        {
            JobPost jobpostId = await _unitOfWork.GetRepository<JobPost>().SingleOrDefaultAsync(
                predicate: x => x.JobPostId.Equals(request.JobPostId)) ??
                throw new BadHttpRequestException("JobPostNotFound");

            Cv cvId = await _unitOfWork.GetRepository<Cv>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(request.Cvid)) ??
                throw new BadHttpRequestException("CVNotFound");

            // Check if the user already apply this job
            JobApplication existingJobApply = await _unitOfWork.GetRepository<JobApplication>().SingleOrDefaultAsync(
                predicate: s => s.JobPostId == request.JobPostId && s.Cvid == request.Cvid);

            if (existingJobApply != null)
            {
                throw new BadHttpRequestException("Bạn đã nộp CV vào đây rồi!");
            }

            JobApplication newJobApplication = _mapper.Map<JobApplication>(request);
            await _unitOfWork.GetRepository<JobApplication>().InsertAsync(newJobApplication);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newJobApplication.ApplicationId;
        }

        public async Task<JobApplicationResponse> GetJobApplicationById(int id)
        {
            JobApplication jobApplication = await _unitOfWork.GetRepository<JobApplication>().SingleOrDefaultAsync(
                predicate: x => x.ApplicationId.Equals(id)) ??
                throw new BadHttpRequestException("JobApplicationNotFound");

            JobApplicationResponse result = _mapper.Map<JobApplicationResponse>(jobApplication);
            return result;
        }

        public async Task<bool> UpdateJobApplicationInfo(int id, JobApplicationInfo request)
        {
            JobApplication jobApplication = await _unitOfWork.GetRepository<JobApplication>().SingleOrDefaultAsync(
                predicate: x => x.ApplicationId.Equals(id)) ??
                throw new BadHttpRequestException("JobApplicationNotFound");

            // Validate and update Job Post (if provided)
            if (request.JobPostId > 0)
            {
                JobPost jobpostId = await _unitOfWork.GetRepository<JobPost>().SingleOrDefaultAsync(
                    predicate: x => x.JobPostId.Equals(request.JobPostId)) ??
                    throw new BadHttpRequestException("JobPostNotFound");
                jobApplication.JobPostId = request.JobPostId;
            }

            // Validate and update CV (if provided)
            if (request.Cvid > 0)
            {
                Cv cvId = await _unitOfWork.GetRepository<Cv>().SingleOrDefaultAsync(
                    predicate: x => x.Id.Equals(request.Cvid)) ??
                    throw new BadHttpRequestException("CVNotFound");
                jobApplication.Cvid = request.Cvid;
            }

            jobApplication.LetterCover = string.IsNullOrEmpty(request.LetterCover) ? jobApplication.LetterCover : request.LetterCover;
            jobApplication.Status = string.IsNullOrEmpty(request.Status) ? jobApplication.Status : request.Status;
            jobApplication.Type = string.IsNullOrEmpty(request.Type) ? jobApplication.Type : request.Type;

            // Additional validation for specific fields if needed
            if (!string.IsNullOrEmpty(jobApplication.LetterCover) && jobApplication.LetterCover.Length > 2000)
                throw new BadHttpRequestException("Letter cover cannot exceed 2000 characters.");

            jobApplication.UpdatedAt = DateTime.Now;
            jobApplication.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<JobApplication>().UpdateAsync(jobApplication);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<IPaginate<JobApplicationResponse>> ViewAllJobApplications(JobApplicationFilter filter, PagingModel pagingModel)
        {
            IPaginate<JobApplicationResponse> response = await _unitOfWork.GetRepository<JobApplication>().GetPagingListAsync(
                selector: x => _mapper.Map<JobApplicationResponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
                );
            return response;
        }

        public async Task<List<JobApplicationResponse>> GetJobApplicationsByJobPostId(int jobPostId)
        {
            var list = await _unitOfWork.GetRepository<JobApplication>().GetListAsync(
                selector: x => _mapper.Map<JobApplicationResponse>(x),
                predicate: x => x.JobPostId == jobPostId
            );
            return list.ToList();
        }

        public async Task<List<JobApplicationResponse>> GetJobApplicationsByCvId(int cvId)
        {
            var list = await _unitOfWork.GetRepository<JobApplication>().GetListAsync(
                selector: x => _mapper.Map<JobApplicationResponse>(x),
                predicate: x => x.Cvid == cvId
            );
            return list.ToList();
        }

        //public async Task<bool> DeleteJobApplication(int id)
        //{
        //    var application = await _unitOfWork.GetRepository<JobApplication>().SingleOrDefaultAsync(
        //        predicate: x => x.ApplicationId == id
        //    ) ?? throw new BadHttpRequestException("JobApplicationNotFound");

        //    _unitOfWork.GetRepository<JobApplication>().DeleteAsync(application);
        //    return await _unitOfWork.CommitAsync() > 0;
        //}

        public async Task<int> CountAllJobApplications()
        {
            return await _unitOfWork.GetRepository<JobApplication>().CountAsync(x => true);
        }

        public async Task<bool> HasAlreadyApplied(int jobPostId, int cvId)
        {
            return await _unitOfWork.GetRepository<JobApplication>().AnyAsync(
                predicate: x => x.JobPostId == jobPostId && x.Cvid == cvId
            );
        }


    }
}
