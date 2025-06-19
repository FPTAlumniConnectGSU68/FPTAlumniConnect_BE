using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.NotificationSetting;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class NotificationSettingService : BaseService<NotificationSettingService>, INotificationSettingService
    {
        public NotificationSettingService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<NotificationSettingService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<int> CreateNotificationSetting(NotificationSettingInfo request)
        {
            var newNotificationSetting = _mapper.Map<NotificationSetting>(request);
            newNotificationSetting.CreatedAt = DateTime.Now;
            newNotificationSetting.CreatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            await _unitOfWork.GetRepository<NotificationSetting>().InsertAsync(newNotificationSetting);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newNotificationSetting.Id;
        }

        public async Task<GetNotificationSettingResponse> GetNotificationSettingById(int id)
        {
            NotificationSetting notificationSetting = await _unitOfWork.GetRepository<NotificationSetting>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id)) ??
                throw new BadHttpRequestException("NotificationSettingNotFound");

            GetNotificationSettingResponse result = _mapper.Map<GetNotificationSettingResponse>(notificationSetting);
            return result;
        }

        public async Task<bool> UpdateNotificationSetting(int id, NotificationSettingInfo request)
        {
            NotificationSetting notificationSetting = await _unitOfWork.GetRepository<NotificationSetting>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id)) ??
                throw new BadHttpRequestException("NotificationSettingNotFound");

            notificationSetting.UserId = request.UserId ?? notificationSetting.UserId;
            notificationSetting.ReceiveEmailNotifications = request.ReceiveEmailNotifications ?? notificationSetting.ReceiveEmailNotifications;
            notificationSetting.ReceiveInAppNotifications = request.ReceiveInAppNotifications ?? notificationSetting.ReceiveInAppNotifications;
            notificationSetting.JobNotifications = request.JobNotifications ?? notificationSetting.JobNotifications;
            notificationSetting.MessageNotifications = request.MessageNotifications ?? notificationSetting.MessageNotifications;
            notificationSetting.UpdatedAt = DateTime.Now;
            notificationSetting.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<NotificationSetting>().UpdateAsync(notificationSetting);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<IPaginate<GetNotificationSettingResponse>> ViewAllNotificationSettings(NotificationSettingFilter filter, PagingModel pagingModel)
        {
            IPaginate<GetNotificationSettingResponse> response = await _unitOfWork.GetRepository<NotificationSetting>().GetPagingListAsync(
                selector: x => _mapper.Map<GetNotificationSettingResponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
                );
            return response;
        }
    }
}