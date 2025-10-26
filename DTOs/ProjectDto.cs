using System.ComponentModel.DataAnnotations;

namespace ProjectManagerAPI.DTOs;

public class CreateProjectDto
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Description { get; set; }
}

public class ProjectResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<TaskResponseDto> Tasks { get; set; } = new();
}

public class CreateTaskDto
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    public DateTime? DueDate { get; set; }
}

public class UpdateTaskDto
{
    [StringLength(200)]
    public string? Title { get; set; }
    
    public DateTime? DueDate { get; set; }
    
    public bool? IsCompleted { get; set; }
}

public class TaskResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ProjectId { get; set; }
}