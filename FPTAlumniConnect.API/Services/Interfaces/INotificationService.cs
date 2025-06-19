using FPTAlumniConnect.BusinessTier.Payload.MessageGroupChat;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.BusinessTier.Payload.Notification;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.BusinessTier.Payload.Comment;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface INotificationService
    {
        Task<List<Notification>> GetUserNotificationsAsync(int userId);
        Task<bool> MarkAsReadAsync(int notificationId);
        Task<bool> SendNotificationAsync(NotificationPayload notificationPayload);
    }
}
