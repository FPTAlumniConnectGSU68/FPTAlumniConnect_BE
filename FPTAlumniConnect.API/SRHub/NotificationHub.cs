using FPTAlumniConnect.API.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class NotificationHub : Hub
{
    public async Task SendNotificationToUser(string userId, string message)
    {
        await Clients.User(userId).SendAsync("ReceiveNotification", message);
    }

    public async Task BroadcastNotification(string message)
    {
        await Clients.All.SendAsync("ReceiveNotification", message);
    }
}
