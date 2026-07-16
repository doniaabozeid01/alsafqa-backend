using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace alsafqa.Hubs;

[Authorize(Roles = "Admin")]
public class NotificationHub : Hub
{
    public const string AdminsGroup = "Admins";

    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, AdminsGroup);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, AdminsGroup);
        await base.OnDisconnectedAsync(exception);
    }
}
