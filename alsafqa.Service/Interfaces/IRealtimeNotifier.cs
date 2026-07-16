using alsafqa.Service.DTOs;

namespace alsafqa.Service.Interfaces;

public interface IRealtimeNotifier
{
    Task NotifyNewContactMessageAsync(ContactMessageDto message);
}
