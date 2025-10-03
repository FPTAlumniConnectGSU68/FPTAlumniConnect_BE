using AutoMapper;
using FPTAlumniConnect.API.Exceptions;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.Notification;
using FPTAlumniConnect.BusinessTier.Payload.RecruiterInfo;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class RecruiterInfoService : BaseService<RecruiterInfoService>, IRecruiterInfoService
    {
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;

        public RecruiterInfoService(
            IUnitOfWork<AlumniConnectContext> unitOfWork,
            ILogger<RecruiterInfoService> logger,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            INotificationService notificationService,
            IUserService userService)
            : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _notificationService = notificationService;
            _userService = userService;
        }

        // Create a new recruiter info (one per user)
        public async Task<int> CreateNewRecruiterInfo(RecruiterInfoInfo request)
        {
            // Ensure user exists
            var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: x => x.UserId == request.UserId);
            if (user == null)
                throw new BadHttpRequestException(ErrorMessages.UserNotFound);

            // Ensure user does not already have recruiter info
            var existing = await _unitOfWork.GetRepository<RecruiterInfo>().SingleOrDefaultAsync(
                predicate: x => x.UserId == request.UserId);
            if (existing != null)
                throw new BadHttpRequestException(ErrorMessages.RecruiterInfoAlreadyExists);

            // Map request to entity
            var entity = _mapper.Map<RecruiterInfo>(request);
            entity.CreatedAt = TimeHelper.NowInVietnam();
            entity.UpdatedAt = TimeHelper.NowInVietnam();

            // Insert into database
            await _unitOfWork.GetRepository<RecruiterInfo>().InsertAsync(entity);
            bool isSuccess = await _unitOfWork.CommitAsync() > 0;

            if (!isSuccess)
                throw new BadHttpRequestException(ErrorMessages.CreateFailed);

            // Send notification to user
            var notificationPayload = new NotificationPayload
            {
                UserId = request.UserId,
                Message = $"Thông tin nhà tuyển dụng của bạn đã được tạo thành công.",
                IsRead = false,
            };

            try
            {
                await _notificationService.SendNotificationAsync(notificationPayload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification for new recruiter info {RecruiterInfoId}", entity.RecruiterInfoId);
            }

            return entity.RecruiterInfoId;
        }

        // Get recruiter info by ID
        public async Task<RecruiterInfoResponse> GetRecruiterInfoById(int id)
        {
            var entity = await _unitOfWork.GetRepository<RecruiterInfo>().SingleOrDefaultAsync(
                predicate: x => x.RecruiterInfoId == id)
                ?? throw new BadHttpRequestException(ErrorMessages.RecruiterInfoNotFound);

            return _mapper.Map<RecruiterInfoResponse>(entity);
        }

        // Get recruiter info by user ID
        public async Task<RecruiterInfoResponse> GetRecruiterInfoByUserId(int userId)
        {
            var entity = await _unitOfWork.GetRepository<RecruiterInfo>().SingleOrDefaultAsync(
                predicate: x => x.UserId == userId)
                ?? throw new BadHttpRequestException(ErrorMessages.RecruiterInfoNotFound);

            return _mapper.Map<RecruiterInfoResponse>(entity);
        }

        // Update recruiter info
        public async Task<bool> UpdateRecruiterInfo(int id, RecruiterInfoInfo request)
        {
            var entity = await _unitOfWork.GetRepository<RecruiterInfo>().SingleOrDefaultAsync(
                predicate: x => x.RecruiterInfoId == id)
                ?? throw new BadHttpRequestException(ErrorMessages.RecruiterInfoNotFound);

            // Store original values for notification check
            var originalCompanyName = entity.CompanyName;
            var originalCompanyEmail = entity.CompanyEmail;
            var originalStatus = entity.Status;

            // Update fields if provided
            entity.CompanyName = request.CompanyName ?? entity.CompanyName;
            entity.CompanyEmail = request.CompanyEmail ?? entity.CompanyEmail;
            entity.CompanyPhone = request.CompanyPhone ?? entity.CompanyPhone;
            entity.CompanyLogoUrl = request.CompanyLogoUrl ?? entity.CompanyLogoUrl;
            entity.CompanyCertificateUrl = request.CompanyCertificateUrl ?? entity.CompanyCertificateUrl;
            entity.Status = request.Status ?? entity.Status;
            entity.UpdatedAt = TimeHelper.NowInVietnam();

            // Update in database
            _unitOfWork.GetRepository<RecruiterInfo>().UpdateAsync(entity);
            bool isSuccess = await _unitOfWork.CommitAsync() > 0;

            // Send notification if any key fields changed
            if (isSuccess && (
                !string.Equals(originalCompanyName, entity.CompanyName, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(originalCompanyEmail, entity.CompanyEmail, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(originalStatus, entity.Status, StringComparison.OrdinalIgnoreCase)))
            {
                var user = await _userService.GetUserById(entity.UserId);
                string message = user != null
                    ? $"Thông tin nhà tuyển dụng của {user.FirstName} đã được cập nhật."
                    : $"Thông tin nhà tuyển dụng của bạn đã được cập nhật.";

                var notificationPayload = new NotificationPayload
                {
                    UserId = entity.UserId,
                    Message = message,
                    IsRead = false
                };

                try
                {
                    await _notificationService.SendNotificationAsync(notificationPayload);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send notification for updated recruiter info {RecruiterInfoId}", entity.RecruiterInfoId);
                }
            }

            return isSuccess;
        }

        public async Task<bool> UpdateRecruiterInfoByUser(RecruiterInfoInfo request)
        {
            var entity = await _unitOfWork.GetRepository<RecruiterInfo>().SingleOrDefaultAsync(
                predicate: x => x.UserId == request.UserId)
                ?? throw new BadHttpRequestException(ErrorMessages.RecruiterInfoNotFound);

            // Store original values for notification check
            var originalCompanyName = entity.CompanyName;
            var originalCompanyEmail = entity.CompanyEmail;
            var originalStatus = entity.Status;

            // Update fields if provided
            entity.CompanyName = request.CompanyName ?? entity.CompanyName;
            entity.CompanyEmail = request.CompanyEmail ?? entity.CompanyEmail;
            entity.CompanyPhone = request.CompanyPhone ?? entity.CompanyPhone;
            entity.CompanyLogoUrl = request.CompanyLogoUrl ?? entity.CompanyLogoUrl;
            entity.CompanyCertificateUrl = request.CompanyCertificateUrl ?? entity.CompanyCertificateUrl;
            entity.Status = request.Status ?? entity.Status;
            entity.UpdatedAt = TimeHelper.NowInVietnam();

            // Update in database
            _unitOfWork.GetRepository<RecruiterInfo>().UpdateAsync(entity);
            bool isSuccess = await _unitOfWork.CommitAsync() > 0;

            // Send notification if any key fields changed
            if (isSuccess && (
                !string.Equals(originalCompanyName, entity.CompanyName, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(originalCompanyEmail, entity.CompanyEmail, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(originalStatus, entity.Status, StringComparison.OrdinalIgnoreCase)))
            {
                var user = await _userService.GetUserById(entity.UserId);
                string message = user != null
                    ? $"Thông tin nhà tuyển dụng của {user.FirstName} đã được cập nhật."
                    : $"Thông tin nhà tuyển dụng của bạn đã được cập nhật.";

                var notificationPayload = new NotificationPayload
                {
                    UserId = entity.UserId,
                    Message = message,
                    IsRead = false
                };

                try
                {
                    await _notificationService.SendNotificationAsync(notificationPayload);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send notification for updated recruiter info for UserId {UserId}", entity.UserId);
                }
            }

            return isSuccess;
        }

        // Get all recruiter info with filter & pagination
        public async Task<IPaginate<RecruiterInfoResponse>> ViewAllRecruiters(RecruiterInfoFilter filter, PagingModel paging)
        {
            var response = await _unitOfWork.GetRepository<RecruiterInfo>().GetPagingListAsync(
                selector: x => _mapper.Map<RecruiterInfoResponse>(x),
                filter: filter,
                orderBy: x => x.OrderByDescending(x => x.CreatedAt),
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
                ?? throw new BadHttpRequestException(ErrorMessages.RecruiterInfoNotFound);

            // Delete from database
            _unitOfWork.GetRepository<RecruiterInfo>().DeleteAsync(entity);
            bool isSuccess = await _unitOfWork.CommitAsync() > 0;

            // Send notification to user
            if (isSuccess)
            {
                var user = await _userService.GetUserById(entity.UserId);
                string message = user != null
                    ? $"Thông tin nhà tuyển dụng của {user.FirstName} đã bị xóa."
                    : $"Thông tin nhà tuyển dụng của bạn đã bị xóa.";

                var notificationPayload = new NotificationPayload
                {
                    UserId = entity.UserId,
                    Message = message,
                    IsRead = false
                };

                try
                {
                    await _notificationService.SendNotificationAsync(notificationPayload);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send notification for deleted recruiter info {RecruiterInfoId}", entity.RecruiterInfoId);
                }
            }

            return isSuccess;
        }

        // Optional: Count all recruiter infos in system
        public async Task<int> CountAllRecruiters()
        {
            return await _unitOfWork.GetRepository<RecruiterInfo>()
                .GetQueryable()
                .CountAsync();
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
            if (!validStatuses.Any(s => s.Equals(status, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogWarning("Invalid status provided: {Status}", status);
                throw new BadHttpRequestException(ErrorMessages.InvalidStatus);
            }

            _logger.LogInformation("Updating status for recruiter info ID: {Id} to {Status}", id, status);

            // Retrieve recruiter info
            var entity = await _unitOfWork.GetRepository<RecruiterInfo>().SingleOrDefaultAsync(
                predicate: x => x.RecruiterInfoId == id)
                ?? throw new BadHttpRequestException(ErrorMessages.RecruiterInfoNotFound);

            // Check if status changed
            var originalStatus = entity.Status;

            // Update status and audit fields
            entity.Status = status;
            entity.UpdatedAt = TimeHelper.NowInVietnam();

            // Update in database
            _unitOfWork.GetRepository<RecruiterInfo>().UpdateAsync(entity);
            bool isSuccess = await _unitOfWork.CommitAsync() > 0;

            if (!isSuccess)
            {
                _logger.LogError("Failed to update status for recruiter info ID: {Id}", id);
                throw new BadHttpRequestException(ErrorMessages.UpdateStatusFailed);
            }

            // Send notification if status changed
            if (isSuccess && !string.Equals(originalStatus, status, StringComparison.OrdinalIgnoreCase))
            {
                var user = await _userService.GetUserById(entity.UserId);
                string message = user != null
                    ? $"Trạng thái nhà tuyển dụng của {user.FirstName} đã được cập nhật thành {status}."
                    : $"Trạng thái nhà tuyển dụng của bạn đã được cập nhật thành {status}.";

                var notificationPayload = new NotificationPayload
                {
                    UserId = entity.UserId,
                    Message = message,
                    IsRead = false
                };

                try
                {
                    await _notificationService.SendNotificationAsync(notificationPayload);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send notification for status update of recruiter info {RecruiterInfoId}", entity.RecruiterInfoId);
                }
            }

            _logger.LogInformation("Successfully updated status for recruiter info ID: {Id} to {Status}", id, status);
            return isSuccess;
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
    }
}