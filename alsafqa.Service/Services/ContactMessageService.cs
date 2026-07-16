using alsafqa.Data.Entities;
using alsafqa.Repository.Interfaces;
using alsafqa.Service.DTOs;
using alsafqa.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace alsafqa.Service.Services;

public class ContactMessageService : IContactMessageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRealtimeNotifier _realtimeNotifier;

    public ContactMessageService(IUnitOfWork unitOfWork, IRealtimeNotifier realtimeNotifier)
    {
        _unitOfWork = unitOfWork;
        _realtimeNotifier = realtimeNotifier;
    }

    public async Task<ApiResponse<ContactMessageDto>> CreateAsync(CreateContactMessageDto request)
    {
        var message = new ContactMessage
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim(),
            Phone = request.Phone.Trim(),
            Subject = request.Subject.Trim(),
            Message = request.Message.Trim(),
            IsRead = false
        };

        await _unitOfWork.Repository<ContactMessage>().AddAsync(message);
        await _unitOfWork.SaveChangesAsync();

        var dto = MapToDto(message);
        await _realtimeNotifier.NotifyNewContactMessageAsync(dto);

        return ApiResponse<ContactMessageDto>.Ok(dto, "Message sent successfully.");
    }

    public async Task<ApiResponse<IReadOnlyList<ContactMessageDto>>> GetAllAsync(bool? isRead = null)
    {
        var query = _unitOfWork.Repository<ContactMessage>().Query();

        if (isRead.HasValue)
        {
            query = query.Where(m => m.IsRead == isRead.Value);
        }

        var messages = await query
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

        return ApiResponse<IReadOnlyList<ContactMessageDto>>.Ok(messages.Select(MapToDto).ToList());
    }

    public async Task<ApiResponse<ContactMessageDto>> GetByIdAsync(Guid id)
    {
        var message = await _unitOfWork.Repository<ContactMessage>().FirstOrDefaultAsync(m => m.Id == id);
        if (message is null)
        {
            return ApiResponse<ContactMessageDto>.Fail("Message not found.");
        }

        return ApiResponse<ContactMessageDto>.Ok(MapToDto(message));
    }

    public async Task<ApiResponse<ContactMessageDto>> MarkAsReadAsync(Guid id)
    {
        var message = await _unitOfWork.Repository<ContactMessage>().FirstOrDefaultAsync(m => m.Id == id);
        if (message is null)
        {
            return ApiResponse<ContactMessageDto>.Fail("Message not found.");
        }

        message.IsRead = true;
        message.ReadAt = DateTime.UtcNow;
        message.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Repository<ContactMessage>().Update(message);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<ContactMessageDto>.Ok(MapToDto(message), "Message marked as read.");
    }

    public async Task<ApiResponse<ContactMessageDto>> MarkAsUnreadAsync(Guid id)
    {
        var message = await _unitOfWork.Repository<ContactMessage>().FirstOrDefaultAsync(m => m.Id == id);
        if (message is null)
        {
            return ApiResponse<ContactMessageDto>.Fail("Message not found.");
        }

        message.IsRead = false;
        message.ReadAt = null;
        message.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Repository<ContactMessage>().Update(message);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<ContactMessageDto>.Ok(MapToDto(message), "Message marked as unread.");
    }

    public async Task<ApiResponse<int>> GetUnreadCountAsync()
    {
        var count = await _unitOfWork.Repository<ContactMessage>().Query()
            .CountAsync(m => !m.IsRead);

        return ApiResponse<int>.Ok(count);
    }

    public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
    {
        var message = await _unitOfWork.Repository<ContactMessage>().FirstOrDefaultAsync(m => m.Id == id);
        if (message is null)
        {
            return ApiResponse<bool>.Fail("Message not found.");
        }

        message.IsDeleted = true;
        message.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Repository<ContactMessage>().Update(message);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true, "Message deleted successfully.");
    }

    private static ContactMessageDto MapToDto(ContactMessage message) => new()
    {
        Id = message.Id,
        FullName = message.FullName,
        Email = message.Email,
        Phone = message.Phone,
        Subject = message.Subject,
        Message = message.Message,
        IsRead = message.IsRead,
        CreatedAt = message.CreatedAt,
        ReadAt = message.ReadAt
    };
}
