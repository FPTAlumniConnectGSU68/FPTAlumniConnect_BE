using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.RecruiterInfo;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class RecruiterInfoService : BaseService<RecruiterInfoService>, IRecruiterInfoService
    {
        public RecruiterInfoService(IUnitOfWork<AlumniConnectContext> unitOfWork,
            ILogger<RecruiterInfoService> logger,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        // Create a new recruiter info (one per user)
        public async Task<int> CreateNewRecruiterInfo(RecruiterInfoInfo request)
        {
            // Ensure user exists
            var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: x => x.UserId == request.UserId);
            if (user == null)
                throw new BadHttpRequestException("UserNotFound");

            // Ensure user does not already have recruiter info
            var existing = await _unitOfWork.GetRepository<RecruiterInfo>().SingleOrDefaultAsync(
                predicate: x => x.UserId == request.UserId);
            if (existing != null)
                throw new BadHttpRequestException("RecruiterInfoAlreadyExists");

            var entity = _mapper.Map<RecruiterInfo>(request);
            entity.CreatedAt = TimeHelper.NowInVietnam();
            //entity.CreatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            await _unitOfWork.GetRepository<RecruiterInfo>().InsertAsync(entity);
            bool isSuccess = await _unitOfWork.CommitAsync() > 0;

            if (!isSuccess)
                throw new BadHttpRequestException("CreateFailed");

            return entity.RecruiterInfoId;
        }

        // Get recruiter info by ID
        public async Task<RecruiterInfoResponse> GetRecruiterInfoById(int id)
        {
            var entity = await _unitOfWork.GetRepository<RecruiterInfo>().SingleOrDefaultAsync(
                predicate: x => x.RecruiterInfoId == id)
                ?? throw new BadHttpRequestException("RecruiterInfoNotFound");

            return _mapper.Map<RecruiterInfoResponse>(entity);
        }

        // Get recruiter info by user ID
        public async Task<RecruiterInfoResponse> GetRecruiterInfoByUserId(int userId)
        {
            var entity = await _unitOfWork.GetRepository<RecruiterInfo>().SingleOrDefaultAsync(
                predicate: x => x.UserId == userId)
                ?? throw new BadHttpRequestException("RecruiterInfoNotFound");

            return _mapper.Map<RecruiterInfoResponse>(entity);
        }

        // Update recruiter info
        public async Task<bool> UpdateRecruiterInfo(int id, RecruiterInfoInfo request)
        {
            var entity = await _unitOfWork.GetRepository<RecruiterInfo>().SingleOrDefaultAsync(
                predicate: x => x.RecruiterInfoId == id)
                ?? throw new BadHttpRequestException("RecruiterInfoNotFound");

            entity.CompanyName = request.CompanyName ?? entity.CompanyName;
            entity.CompanyEmail = request.CompanyEmail ?? entity.CompanyEmail;
            entity.CompanyPhone = request.CompanyPhone ?? entity.CompanyPhone;
            entity.CompanyLogoUrl = request.CompanyLogoUrl ?? entity.CompanyLogoUrl;
            entity.CompanyCertificateUrl = request.CompanyCertificateUrl ?? entity.CompanyCertificateUrl;
            entity.Status = request.Status ?? entity.Status;
            entity.UpdatedAt = DateTime.UtcNow;
            //entity.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<RecruiterInfo>().UpdateAsync(entity);
            return await _unitOfWork.CommitAsync() > 0;
        }

        // Get all recruiter info with filter & pagination
        public async Task<IPaginate<RecruiterInfoResponse>> ViewAllRecruiters(RecruiterInfoFilter filter, PagingModel paging)
        {
            var response = await _unitOfWork.GetRepository<RecruiterInfo>().GetPagingListAsync(
                selector: x => _mapper.Map<RecruiterInfoResponse>(x),
                filter: filter,
                page: paging.page,
                size: paging.size
            );

            return response;
        }

        // Delete recruiter info by ID
        public async Task<bool> DeleteRecruiterInfo(int id)
        {
            var entity = await _unitOfWork.GetRepository<RecruiterInfo>().SingleOrDefaultAsync(
                predicate: x => x.RecruiterInfoId == id)
                ?? throw new BadHttpRequestException("RecruiterInfoNotFound");

            _unitOfWork.GetRepository<RecruiterInfo>().DeleteAsync(entity);
            return await _unitOfWork.CommitAsync() > 0;
        }

        // Optional: Count all recruiter infos in system
        public async Task<int> CountAllRecruiters()
        {
            return await _unitOfWork.GetRepository<RecruiterInfo>()
                .GetQueryable()
                .CountAsync();
        }

        // Constants for error messages
        private static class ErrorMessages
        {
            public const string UserNotFound = "User with the specified ID does not exist.";
            public const string RecruiterInfoAlreadyExists = "Recruiter information already exists for this user.";
            public const string RecruiterInfoNotFound = "Recruiter information with the specified ID does not exist.";
            public const string CreateFailed = "Failed to create recruiter information.";
            public const string InvalidStatus = "Status must be one of: Active, Inactive, Pending.";
            public const string UpdateStatusFailed = "Failed to update recruiter status.";
        }

        // Update recruiter status by ID
        public async Task<bool> UpdateRecruiterStatus(int id, string status)
        {
            // Validate input
            if (id <= 0)
                throw new ArgumentException("Invalid ID. ID must be greater than 0.", nameof(id));
            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("Status cannot be null or empty.", nameof(status));

            // Validate status value
            var validStatuses = new[] { "Active", "Inactive", "Pending" };
            if (!validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Invalid status provided: {Status}", status);
                throw new BadHttpRequestException(ErrorMessages.InvalidStatus);
            }

            _logger.LogInformation("Updating status for recruiter info ID: {Id} to {Status}", id, status);

            // Retrieve recruiter info
            var entity = await _unitOfWork.GetRepository<RecruiterInfo>().SingleOrDefaultAsync(
                predicate: x => x.RecruiterInfoId == id)
                ?? throw new BadHttpRequestException(ErrorMessages.RecruiterInfoNotFound);

            // Update status and audit fields
            entity.Status = status;
            entity.UpdatedAt = TimeHelper.NowInVietnam();

            // Update the entity
            _unitOfWork.GetRepository<RecruiterInfo>().UpdateAsync(entity);
            bool isSuccess = await _unitOfWork.CommitAsync() > 0;

            if (!isSuccess)
            {
                _logger.LogError("Failed to update status for recruiter info ID: {Id}", id);
                throw new BadHttpRequestException(ErrorMessages.UpdateStatusFailed);
            }

            _logger.LogInformation("Successfully updated status for recruiter info ID: {Id} to {Status}", id, status);
            return isSuccess;
        }
    }
}
