namespace FPTAlumniConnect.API.Services.Implements
{
    using Microsoft.Extensions.Logging;
    using global::FPTAlumniConnect.API.Services.Interfaces;
    using global::FPTAlumniConnect.DataTier.Models;
    using global::FPTAlumniConnect.DataTier.Repository.Interfaces;
    using AutoMapper;
    using global::FPTAlumniConnect.BusinessTier.Payload.Notification;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using global::FPTAlumniConnect.BusinessTier.Payload.Comment;

    namespace FPTAlumniConnect.API.Services.Implements
    {
        
        public class NotificationService : BaseService<NotificationService>, INotificationService
        {
            private readonly IHubContext<NotificationHub> _hubContext;
            public NotificationService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<NotificationService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor, IHubContext<NotificationHub> hubContext) : base(unitOfWork, logger, mapper, httpContextAccessor)
            {
                _hubContext = hubContext;
            }

            public async Task<bool> SendNotificationAsync(NotificationPayload notificationPayload)
            {
                var notification = new Notification
                {
                    UserId = notificationPayload.UserId,
                    Message = notificationPayload.Message,
                    Timestamp = DateTime.Now,
                    IsRead = notificationPayload.IsRead
                };

                await _unitOfWork.GetRepository<Notification>().InsertAsync(notification);
                bool result = await _unitOfWork.CommitAsync() > 0;

                if (result)
                {
                    await _hubContext.Clients.User(notificationPayload.UserId.ToString())
                        .SendAsync("ReceiveNotification", notification);
                }

                return result;
            }
            public async Task<List<Notification>> GetUserNotificationsAsync(int userId)
            {
                var notifications = await _unitOfWork.GetRepository<Notification>()
                    .FindAllAsync(n => n.UserId == userId && !n.IsRead);

                return notifications.ToList();
            }

            public async Task<bool> MarkAsReadAsync(int notificationId)
            {
                var notification = await _unitOfWork.GetRepository<Notification>().SingleOrDefaultAsync(n => n.Id == notificationId, null, null);

                if (notification == null)
                {
                    return false;
                }

                notification.IsRead = true;
                _unitOfWork.GetRepository<Notification>().UpdateAsync(notification);
                return await _unitOfWork.CommitAsync() > 0;
            }
          
        }
    }

}
