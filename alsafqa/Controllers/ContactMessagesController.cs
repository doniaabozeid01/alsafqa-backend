using alsafqa.Service.DTOs;
using alsafqa.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace alsafqa.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactMessagesController : ControllerBase
{
    private readonly IContactMessageService _contactMessageService;

    public ContactMessagesController(IContactMessageService contactMessageService)
    {
        _contactMessageService = contactMessageService;
    }

    /// <summary>
    /// Public endpoint — anyone can send a contact message.
    /// </summary>
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ContactMessageDto>>> Create([FromBody] CreateContactMessageDto request)
    {
        var result = await _contactMessageService.CreateAsync(request);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ContactMessageDto>>>> GetAll([FromQuery] bool? isRead)
    {
        var result = await _contactMessageService.GetAllAsync(isRead);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("unread-count")]
    public async Task<ActionResult<ApiResponse<int>>> GetUnreadCount()
    {
        var result = await _contactMessageService.GetUnreadCountAsync();
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ContactMessageDto>>> GetById(Guid id)
    {
        var result = await _contactMessageService.GetByIdAsync(id);
        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id:guid}/read")]
    public async Task<ActionResult<ApiResponse<ContactMessageDto>>> MarkAsRead(Guid id)
    {
        var result = await _contactMessageService.MarkAsReadAsync(id);
        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id:guid}/unread")]
    public async Task<ActionResult<ApiResponse<ContactMessageDto>>> MarkAsUnread(Guid id)
    {
        var result = await _contactMessageService.MarkAsUnreadAsync(id);
        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        var result = await _contactMessageService.DeleteAsync(id);
        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
}
