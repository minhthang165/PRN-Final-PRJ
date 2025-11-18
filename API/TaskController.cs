using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRN_Final_Project.Business.Data;
using PRN_Final_Project.Business.Entities;

namespace PRN_Final_Project.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly PRNDbContext _context;

        public TaskController(PRNDbContext context)
        {
            _context = context;
        }

        // POST: api/task/create
        [HttpPost("create")]
        public async Task<ActionResult> CreateTask([FromBody] TaskCreateDto taskDto)
        {
            try
            {
                if (taskDto == null)
                {
                    return BadRequest("Task data is required");
                }

                // Validate required fields
                if (string.IsNullOrEmpty(taskDto.TaskName))
                {
                    return BadRequest("Task name is required");
                }

                if (taskDto.ClassId <= 0)
                {
                    return BadRequest("Valid class ID is required");
                }

                if (taskDto.Creator <= 0)
                {
                    return BadRequest("Valid creator ID is required");
                }

                // Check if class exists
                var classExists = await _context.Classes.AnyAsync(c => c.id == taskDto.ClassId);
                if (!classExists)
                {
                    return NotFound($"Class with ID {taskDto.ClassId} not found");
                }

                // Check if creator exists
                var creatorExists = await _context.users.AnyAsync(u => u.id == taskDto.Creator);
                if (!creatorExists)
                {
                    return NotFound($"User with ID {taskDto.Creator} not found");
                }

                // Create new task
                var newTask = new UserTask
                {
                    class_id = taskDto.ClassId,
                    task_name = taskDto.TaskName,
                    description = taskDto.Description,
                    status = taskDto.Status ?? "PENDING",
                    start_time = taskDto.StartTime,
                    end_time = taskDto.EndTime,
                    file = taskDto.FileData, // This can be null
                    created_by = taskDto.Creator,
                    created_at = DateTime.Now,
                    is_active = true
                };

                _context.UserTasks.Add(newTask);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    id = newTask.id,
                    classId = newTask.class_id,
                    taskName = newTask.task_name,
                    description = newTask.description,
                    status = newTask.status,
                    startTime = newTask.start_time,
                    endTime = newTask.end_time,
                    fileData = newTask.file,
                    createdAt = newTask.created_at,
                    message = "Task created successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        // GET: api/task/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult> GetTaskById(int id)
        {
            try
            {
                var task = await _context.UserTasks
                    .Where(t => t.id == id && t.is_active == true)
                    .Select(t => new
                    {
                        id = t.id,
                        classId = t.class_id,
                        taskName = t.task_name,
                        startTime = t.start_time,
                        endTime = t.end_time,
                        description = t.description,
                        status = t.status,
                        fileData = t.file,
                        createdAt = t.created_at
                    })
                    .FirstOrDefaultAsync();

                if (task == null)
                {
                    return NotFound($"Task with ID {id} not found");
                }

                return Ok(task);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

    // DTO for creating a task
    public class TaskCreateDto
    {
        public int ClassId { get; set; }
        public string TaskName { get; set; }
        public string? Description { get; set; } // Optional
        public string? Status { get; set; } // Optional, default to "PENDING"
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? FileData { get; set; } // Optional - file attachment
        public int Creator { get; set; }
    }
}
