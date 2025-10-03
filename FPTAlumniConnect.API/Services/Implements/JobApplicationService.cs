using AutoMapper;
using FPTAlumniConnect.API.Exceptions;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.JobApplication;
using FPTAlumniConnect.BusinessTier.Payload.Notification;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class JobApplicationService : BaseService<JobApplicationService>, IJobApplicationService
    {
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;

        public JobApplicationService(
            IUnitOfWork<AlumniConnectContext> unitOfWork,
            ILogger<JobApplicationService> logger,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            INotificationService notificationService,
            IUserService userService)
            : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _notificationService = notificationService;
            _userService = userService;
        }

        public async Task<int> CreateNewJobApplication(JobApplicationInfo request)
        {
            // Validate job post existence
            JobPost jobPost = await _unitOfWork.GetRepository<JobPost>().SingleOrDefaultAsync(
                predicate: x => x.JobPostId.Equals(request.JobPostId)) ??
                throw new BadHttpRequestException("JobPostNotFound");

            // Validate CV existence
            Cv cv = await _unitOfWork.GetRepository<Cv>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(request.Cvid)) ??
                throw new BadHttpRequestException("CVNotFound");

            // Check if the user has already applied for this job
            JobApplication existingJobApply = await _unitOfWork.GetRepository<JobApplication>().SingleOrDefaultAsync(
                predicate: s => s.JobPostId == request.JobPostId && s.Cvid == request.Cvid);

            if (existingJobApply != null)
            {
                throw new BadHttpRequestException("Bạn đã nộp CV vào đây rồi!");
            }

            // Map request to JobApplication entity
            JobApplication newJobApplication = _mapper.Map<JobApplication>(request);
            newJobApplication.CreatedAt = TimeHelper.NowInVietnam();
            newJobApplication.CreatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;
            await _unitOfWork.GetRepository<JobApplication>().InsertAsync(newJobApplication);

            // Commit to database
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            // Send notification to job post author (if not the applicant)
            if (jobPost.UserId != cv.UserId)
            {
                var applicant = await _userService.GetUserById(cv.UserId.Value);
                if (applicant == null)
                {
                    _logger.LogWarning("Applicant not found for CV {CvId}", cv.Id);
                }
                else
                {
                    var notificationPayload = new NotificationPayload
                    {
                        UserId = jobPost.UserId.Value,
                        Message = $"{applicant.FirstName} đã nộp đơn ứng tuyển cho bài đăng: {jobPost.JobTitle}",
                        IsRead = false
                    };

                    try
                    {
                        await _notificationService.SendNotificationAsync(notificationPayload);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send notification for job application {ApplicationId}", newJobApplication.ApplicationId);
                    }
                }
            }

            return newJobApplication.ApplicationId;
        }

        public async Task<JobApplicationResponse> GetJobApplicationById(int id)
        {
            // Retrieve job application by ID
            JobApplication jobApplication = await _unitOfWork.GetRepository<JobApplication>().SingleOrDefaultAsync(
                predicate: x => x.ApplicationId.Equals(id)) ??
                throw new BadHttpRequestException("JobApplicationNotFound");

            JobApplicationResponse result = _mapper.Map<JobApplicationResponse>(jobApplication);
            return result;
        }

        public async Task<bool> UpdateJobApplicationInfo(int id, JobApplicationInfo request)
        {
            // Retrieve existing job application
            JobApplication jobApplication = await _unitOfWork.GetRepository<JobApplication>().SingleOrDefaultAsync(
                predicate: x => x.ApplicationId.Equals(id)) ??
                throw new BadHttpRequestException("JobApplicationNotFound");

            // Store original status for notification check
            string originalStatus = jobApplication.Status;

            // Validate and update Job Post (if provided)
            if (request.JobPostId > 0)
            {
                JobPost jobPost = await _unitOfWork.GetRepository<JobPost>().SingleOrDefaultAsync(
                    predicate: x => x.JobPostId.Equals(request.JobPostId)) ??
                    throw new BadHttpRequestException("JobPostNotFound");
                jobApplication.JobPostId = request.JobPostId;
            }

            // Validate and update CV (if provided)
            if (request.Cvid > 0)
            {
                Cv cv = await _unitOfWork.GetRepository<Cv>().SingleOrDefaultAsync(
                    predicate: x => x.Id.Equals(request.Cvid)) ??
                    throw new BadHttpRequestException("CVNotFound");
                jobApplication.Cvid = request.Cvid;
            }

            // Update fields if provided
            jobApplication.LetterCover = string.IsNullOrEmpty(request.LetterCover) ? jobApplication.LetterCover : request.LetterCover;
            jobApplication.Status = string.IsNullOrEmpty(request.Status) ? jobApplication.Status : request.Status;

            // Validate letter cover length
            if (!string.IsNullOrEmpty(jobApplication.LetterCover) && jobApplication.LetterCover.Length > 2000)
                throw new BadHttpRequestException("Letter cover cannot exceed 2000 characters.");

            jobApplication.UpdatedAt = TimeHelper.NowInVietnam();
            jobApplication.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            // Update in database
            _unitOfWork.GetRepository<JobApplication>().UpdateAsync(jobApplication);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            // Send notification if status changed
            if (isSuccessful && !string.Equals(originalStatus, jobApplication.Status, StringComparison.OrdinalIgnoreCase))
            {
                Cv cv = await _unitOfWork.GetRepository<Cv>().SingleOrDefaultAsync(
                    predicate: x => x.Id.Equals(jobApplication.Cvid));
                JobPost jobPost = await _unitOfWork.GetRepository<JobPost>().SingleOrDefaultAsync(
                    predicate: x => x.JobPostId.Equals(jobApplication.JobPostId));

                if (cv != null && jobPost != null && jobPost.UserId != cv.UserId)
                {
                    var applicant = await _userService.GetUserById(cv.UserId.Value);
                    if (applicant == null)
                    {
                        _logger.LogWarning("Applicant not found for CV {CvId}", cv.Id);
                    }
                    else
                    {
                        var notificationPayload = new NotificationPayload
                        {
                            UserId = (int)jobPost.UserId,
                            Message = $"Trạng thái đơn ứng tuyển của {applicant.FirstName} cho bài đăng: {jobPost.JobTitle} đã được cập nhật thành {jobApplication.Status}",
                            IsRead = false,
                        };

                        try
                        {
                            await _notificationService.SendNotificationAsync(notificationPayload);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to send status update notification for job application {ApplicationId}", jobApplication.ApplicationId);
                        }
                    }
                }
            }

            return isSuccessful;
        }

        public async Task<IPaginate<JobApplicationResponse>> ViewAllJobApplications(JobApplicationFilter filter, PagingModel pagingModel)
        {
            // Retrieve paginated list of job applications
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
            // Retrieve job applications by job post ID
            var list = await _unitOfWork.GetRepository<JobApplication>().GetListAsync(
                selector: x => _mapper.Map<JobApplicationResponse>(x),
                predicate: x => x.JobPostId == jobPostId
            );
            return list.ToList();
        }

        public async Task<List<JobApplicationResponse>> GetJobApplicationsByCvId(int cvId)
        {
            // Retrieve job applications by CV ID
            var list = await _unitOfWork.GetRepository<JobApplication>().GetListAsync(
                selector: x => _mapper.Map<JobApplicationResponse>(x),
                predicate: x => x.Cvid == cvId
            );
            return list.ToList();
        }

        public async Task<int> CountAllJobApplications()
        {
            // Count total job applications
            return await _unitOfWork.GetRepository<JobApplication>().CountAsync(x => true);
        }

        public async Task<bool> HasAlreadyApplied(int jobPostId, int cvId)
        {
            // Check if a job application exists for the given job post and CV
            return await _unitOfWork.GetRepository<JobApplication>().AnyAsync(
                predicate: x => x.JobPostId == jobPostId && x.Cvid == cvId
            );
        }
    }
}