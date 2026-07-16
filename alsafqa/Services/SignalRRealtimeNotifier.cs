using alsafqa.Hubs;
using alsafqa.Service.DTOs;
using alsafqa.Service.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace alsafqa.Services;

public class SignalRRealtimeNotifier : IRealtimeNotifier
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public SignalRRealtimeNotifier(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyNewContactMessageAsync(ContactMessageDto message)
    {
        await _hubContext.Clients
            .Group(NotificationHub.AdminsGroup)
            .SendAsync("ReceiveContactMessage", message);
    }
}
