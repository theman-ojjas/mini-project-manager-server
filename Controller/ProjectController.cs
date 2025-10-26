using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagerAPI.Data;
using ProjectManagerAPI.DTOs;
using ProjectManagerAPI.Models;
using System.Security.Claims;

namespace ProjectManagerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProjectsController(ApplicationDbContext context)
    {
        _context = context;
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

    [HttpGet]
    public async Task<ActionResult<List<ProjectResponseDto>>> GetProjects()
    {
        var userId = GetUserId();
        var projects = await _context.Projects
            .Where(p => p.UserId == userId)
            .Include(p => p.Tasks)
            .Select(p => new ProjectResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                Tasks = p.Tasks.Select(t => new TaskResponseDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    DueDate = t.DueDate,
                    IsCompleted = t.IsCompleted,
                    CreatedAt = t.CreatedAt,
                    ProjectId = t.ProjectId
                }).ToList()
            })
            .ToListAsync();

        return Ok(projects);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectResponseDto>> GetProject(int id)
    {
        var userId = GetUserId();
        var project = await _context.Projects
            .Where(p => p.Id == id && p.UserId == userId)
            .Include(p => p.Tasks)
            .Select(p => new ProjectResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                Tasks = p.Tasks.Select(t => new TaskResponseDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    DueDate = t.DueDate,
                    IsCompleted = t.IsCompleted,
                    CreatedAt = t.CreatedAt,
                    ProjectId = t.ProjectId
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (project == null)
            return NotFound();

        return Ok(project);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectResponseDto>> CreateProject(CreateProjectDto dto)
    {
        var userId = GetUserId();
        var project = new Project
        {
            Title = dto.Title,
            Description = dto.Description,
            UserId = userId
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, new ProjectResponseDto
        {
            Id = project.Id,
            Title = project.Title,
            Description = project.Description,
            CreatedAt = project.CreatedAt,
            Tasks = new List<TaskResponseDto>()
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var userId = GetUserId();
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

        if (project == null)
            return NotFound();

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{projectId}/tasks")]
    public async Task<ActionResult<TaskResponseDto>> CreateTask(int projectId, CreateTaskDto dto)
    {
        var userId = GetUserId();
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);

        if (project == null)
            return NotFound();

        var task = new ProjectTask
        {
            Title = dto.Title,
            DueDate = dto.DueDate,
            ProjectId = projectId
        };

        _context.Tasks.Add(task);
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
}