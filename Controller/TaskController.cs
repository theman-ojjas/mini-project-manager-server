using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagerAPI.Data;
using ProjectManagerAPI.DTOs;
using System.Security.Claims;

namespace ProjectManagerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TasksController(ApplicationDbContext context)
    {
        _context = context;
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

    [HttpPut("{taskId}")]
    public async Task<ActionResult<TaskResponseDto>> UpdateTask(int taskId, UpdateTaskDto dto)
    {
        var userId = GetUserId();
        var task = await _context.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == taskId && t.Project.UserId == userId);

        if (task == null)
            return NotFound();

        if (dto.Title != null)
            task.Title = dto.Title;

        if (dto.DueDate.HasValue)
            task.DueDate = dto.DueDate.Value;

        if (dto.IsCompleted.HasValue)
            task.IsCompleted = dto.IsCompleted.Value;

        await _context.SaveChangesAsync();

        return Ok(new TaskResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            DueDate = task.DueDate,
            IsCompleted = task.IsCompleted,
            CreatedAt = task.CreatedAt,
            ProjectId = task.ProjectId
        });
    }

    [HttpDelete("{taskId}")]
    public async Task<IActionResult> DeleteTask(int taskId)
    {
        var userId = GetUserId();
        var task = await _context.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == taskId && t.Project.UserId == userId);

        if (task == null)
            return NotFound();

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}