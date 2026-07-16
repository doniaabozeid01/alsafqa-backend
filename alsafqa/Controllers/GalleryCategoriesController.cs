using alsafqa.Service.DTOs;
using alsafqa.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace alsafqa.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GalleryCategoriesController : ControllerBase
{
    private readonly IGalleryCategoryService _galleryCategoryService;

    public GalleryCategoriesController(IGalleryCategoryService galleryCategoryService)
    {
        _galleryCategoryService = galleryCategoryService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<GalleryCategoryDto>>>> GetAll([FromQuery] bool? isActive)
    {
        var result = await _galleryCategoryService.GetAllAsync(isActive);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<GalleryCategoryDto>>> GetById(Guid id)
    {
        var result = await _galleryCategoryService.GetByIdAsync(id);
        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<GalleryCategoryDto>>> Create([FromBody] CreateGalleryCategoryDto request)
    {
        var result = await _galleryCategoryService.CreateAsync(request);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<GalleryCategoryDto>>> Update(Guid id, [FromBody] UpdateGalleryCategoryDto request)
    {
        var result = await _galleryCategoryService.UpdateAsync(id, request);
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
        var result = await _galleryCategoryService.DeleteAsync(id);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
