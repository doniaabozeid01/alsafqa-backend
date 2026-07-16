using alsafqa.Service.DTOs;
using alsafqa.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace alsafqa.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GalleriesController : ControllerBase
{
    private readonly IGalleryService _galleryService;

    public GalleriesController(IGalleryService galleryService)
    {
        _galleryService = galleryService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<GalleryDto>>>> GetAll([FromQuery] Guid? categoryId, [FromQuery] bool? isActive)
    {
        var result = await _galleryService.GetAllAsync(categoryId, isActive);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<GalleryDto>>> GetById(Guid id)
    {
        var result = await _galleryService.GetByIdAsync(id);
        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ApiResponse<GalleryDto>>> Create([FromForm] CreateGalleryDto request)
    {
        var result = await _galleryService.CreateAsync(request);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ApiResponse<GalleryDto>>> Update(Guid id, [FromForm] UpdateGalleryDto request)
    {
        var result = await _galleryService.UpdateAsync(id, request);
        if (!result.Success)
        {
            return result.Message == "Gallery item not found." ? NotFound(result) : BadRequest(result);
        }

        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        var result = await _galleryService.DeleteAsync(id);
        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
}
