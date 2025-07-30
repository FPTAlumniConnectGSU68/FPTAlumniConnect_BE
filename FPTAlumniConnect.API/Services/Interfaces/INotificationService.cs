using FPTAlumniConnect.BusinessTier.Payload.Notification;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface INotificationService
    {
        Task<List<Notification>> GetUserNotificationsAsync(int userId);
        Task<bool> MarkAsReadAsync(int notificationId);
        Task<bool> SendNotificationAsync(NotificationPayload notificationPayload);
    }
}
