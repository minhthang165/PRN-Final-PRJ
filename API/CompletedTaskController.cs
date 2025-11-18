using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRN_Final_Project.Business.Data;
using PRN_Final_Project.Business.Entities;

namespace PRN_Final_Project.API
{
    [ApiController]
    [Route("api/completed-tasks")]
    public class CompletedTaskController : ControllerBase
    {
        private readonly PRNDbContext _context;

        public CompletedTaskController(PRNDbContext context)
        {
            _context = context;
        }

        // GET: api/completed-tasks/class/{classId}/task/{taskId}
        // Get all students with their submission status for a specific task in a class
        [HttpGet("class/{classId}/task/{taskId}")]
        public async Task<ActionResult> GetStudentSubmissionsByClassAndTask(int classId, int taskId)
        {
            try
            {
                // Get the task to check deadline
                var task = await _context.UserTasks
                    .FirstOrDefaultAsync(t => t.id == taskId && t.class_id == classId);

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                // Get all students (interns) in the class
                var students = await _context.users
                    .Where(u => u.class_id == classId && u.role == "INTERN" && u.is_active == true)
                    .ToListAsync();

                if (!students.Any())
                {
                    return Ok(new List<object>());
                }

                // Check if task deadline has passed
                bool isOverdue = task.end_time < DateTime.Now;

                var result = new List<object>();

                foreach (var student in students)
                {
                    // Check if student has submitted the task
                    var completedTask = await _context.Completed_Tasks
                        .Where(ct => ct.task_id == taskId && ct.user_id == student.id && ct.class_id == classId)
                        .FirstOrDefaultAsync();

                    // Auto-grade overdue tasks without submission
                    if (completedTask == null && isOverdue)
                    {
                        // Create a record with 0 mark and comment
                        completedTask = new Completed_Task
                        {
                            task_id = taskId,
                            user_id = student.id,
                            class_id = classId,
                            mark = 0,
                            comment = "Không nộp bài đúng hạn",
                            status = "OVERDUE",
                            created_at = DateTime.Now,
                            is_active = true
                        };

                        _context.Completed_Tasks.Add(completedTask);
                    }

                    result.Add(new
                    {
                        user = new
                        {
                            id = student.id,
                            first_name = student.first_name,
                            last_name = student.last_name,
                            email = student.email,
                            avatar_path = student.avatar_path
                        },
                        completedTask = completedTask != null ? new
                        {
                            file = completedTask.file,
                            status = completedTask.status,
                            mark = completedTask.mark,
                            comment = completedTask.comment,
                            createdAt = completedTask.created_at,
                            updatedAt = completedTask.updated_at
                        } : null
                    });
                }

                // Save all auto-graded tasks
                if (isOverdue)
                {
                    await _context.SaveChangesAsync();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // POST: api/completed-tasks/auto-grade/{taskId}/{classId}
        // Manually trigger auto-grading for overdue tasks
        [HttpPost("auto-grade/{taskId}/{classId}")]
        public async Task<ActionResult> AutoGradeOverdueTasks(int taskId, int classId)
        {
            try
            {
                // Get the task to check deadline
                var task = await _context.UserTasks
                    .FirstOrDefaultAsync(t => t.id == taskId && t.class_id == classId);

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                // Check if task deadline has passed
                if (task.end_time >= DateTime.Now)
                {
                    return BadRequest("Task is not overdue yet. Auto-grading can only be applied to overdue tasks.");
                }

                // Get all students in the class
                var students = await _context.users
                    .Where(u => u.class_id == classId && u.role == "INTERN" && u.is_active == true)
                    .ToListAsync();

                int gradedCount = 0;

                foreach (var student in students)
                {
                    // Check if student has submitted the task
                    var existingSubmission = await _context.Completed_Tasks
                        .FirstOrDefaultAsync(ct => ct.task_id == taskId && ct.user_id == student.id && ct.class_id == classId);

                    // Only auto-grade if no submission exists
                    if (existingSubmission == null)
                    {
                        var completedTask = new Completed_Task
                        {
                            task_id = taskId,
                            user_id = student.id,
                            class_id = classId,
                            mark = 0,
                            comment = "Không nộp bài đúng hạn",
                            status = "OVERDUE",
                            created_at = DateTime.Now,
                            is_active = true
                        };

                        _context.Completed_Tasks.Add(completedTask);
                        gradedCount++;
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = $"Auto-graded {gradedCount} overdue submissions successfully",
                    gradedCount = gradedCount,
                    totalStudents = students.Count
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET: api/completed-tasks/find/{taskId}/{userId}/{classId}
        // Get a specific student's submission
        [HttpGet("find/{taskId}/{userId}/{classId}")]
        public async Task<ActionResult> GetSubmission(int taskId, int userId, int classId)
        {
            try
            {
                var submission = await _context.Completed_Tasks
                    .Where(ct => ct.task_id == taskId && ct.user_id == userId && ct.class_id == classId)
                    .Select(ct => new
                    {
                        file = ct.file,
                        status = ct.status,
                        mark = ct.mark,
                        comment = ct.comment,
                        createdAt = ct.created_at,
                        updatedAt = ct.updated_at
                    })
                    .FirstOrDefaultAsync();

                // Return null object instead of NotFound when no submission exists
                // This is a valid state - the student simply hasn't submitted yet
                if (submission == null)
                {
                    return Ok(new
                    {
                        file = (string)null,
                        status = (string)null,
                        mark = (int?)null,
                        comment = (string)null,
                        createdAt = (DateTime?)null,
                        updatedAt = (DateTime?)null
                    });
                }

                return Ok(submission);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // POST: api/completed-tasks/create
        // Create a new submission
        [HttpPost("create")]
        public async Task<ActionResult> CreateSubmission([FromBody] CompletedTaskCreateDto dto)
        {
            try
            {
                if (dto == null || dto.Id == null)
                {
                    return BadRequest("Invalid submission data");
                }

                // Check if submission already exists
                var exists = await _context.Completed_Tasks
                    .AnyAsync(ct => ct.task_id == dto.Id.TaskId && ct.user_id == dto.Id.UserId && ct.class_id == dto.Id.ClassId);

                if (exists)
                {
                    return Conflict("Submission already exists");
                }

                var completedTask = new Completed_Task
                {
                    task_id = dto.Id.TaskId,
                    user_id = dto.Id.UserId,
                    class_id = dto.Id.ClassId,
                    file = dto.File,
                    status = dto.Status ?? "PENDING",
                    created_at = DateTime.Now,
                    is_active = dto.IsActive ?? true
                };

                _context.Completed_Tasks.Add(completedTask);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Submission created successfully",
                    data = new
                    {
                        taskId = completedTask.task_id,
                        userId = completedTask.user_id,
                        classId = completedTask.class_id,
                        file = completedTask.file,
                        status = completedTask.status,
                        createdAt = completedTask.created_at
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        // PATCH: api/completed-tasks/{taskId}/{userId}/{classId}/File
        // Update submission file
        [HttpPatch("{taskId}/{userId}/{classId}/File")]
        public async Task<ActionResult> UpdateFile(int taskId, int userId, int classId, [FromBody] string fileUrl)
        {
            try
            {
                var submission = await _context.Completed_Tasks
                    .FirstOrDefaultAsync(ct => ct.task_id == taskId && ct.user_id == userId && ct.class_id == classId);

                if (submission == null)
                {
                    return NotFound("Submission not found");
                }

                submission.file = fileUrl;
                submission.updated_at = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { message = "File updated successfully", file = submission.file });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // PATCH: api/completed-tasks/{taskId}/{userId}/{classId}/comment
        // Add or update comment
        [HttpPatch("{taskId}/{userId}/{classId}/comment")]
        public async Task<ActionResult> UpdateComment(int taskId, int userId, int classId, [FromBody] CommentDto commentDto)
        {
            try
            {
                var submission = await _context.Completed_Tasks
                    .FirstOrDefaultAsync(ct => ct.task_id == taskId && ct.user_id == userId && ct.class_id == classId);

                if (submission == null)
                {
                    // If submission doesn't exist, create a new one
                    submission = new Completed_Task
                    {
                        task_id = taskId,
                        user_id = userId,
                        class_id = classId,
                        comment = commentDto.Comment,
                        status = "PENDING",
                        created_at = DateTime.Now,
                        is_active = true
                    };
                    
                    _context.Completed_Tasks.Add(submission);
                }
                else
                {
                    // Update existing submission
                    submission.comment = commentDto.Comment;
                    submission.updated_at = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = "Comment updated successfully", comment = submission.comment });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // PATCH: api/completed-tasks/{taskId}/{userId}/{classId}/mark
        // Add or update mark/score
        [HttpPatch("{taskId}/{userId}/{classId}/mark")]
        public async Task<ActionResult> UpdateMark(int taskId, int userId, int classId, [FromBody] int mark)
        {
            try
            {
                if (mark < 0 || mark > 10)
                {
                    return BadRequest("Mark must be between 0 and 10");
                }

                var submission = await _context.Completed_Tasks
                    .FirstOrDefaultAsync(ct => ct.task_id == taskId && ct.user_id == userId && ct.class_id == classId);

                if (submission == null)
                {
                    // If submission doesn't exist, create a new one
                    submission = new Completed_Task
                    {
                        task_id = taskId,
                        user_id = userId,
                        class_id = classId,
                        mark = mark,
                        status = "PENDING",
                        created_at = DateTime.Now,
                        is_active = true
                    };
            
                    _context.Completed_Tasks.Add(submission);
                }
                else
                {
                    // Update existing submission
                    submission.mark = mark;
                    submission.updated_at = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = "Mark updated successfully", mark = submission.mark });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

    // DTO for creating completed task
    public class CompletedTaskCreateDto
    {
        public CompletedTaskIdDto Id { get; set; }
        public string File { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? IsActive { get; set; }
    }

    public class CompletedTaskIdDto
    {
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public int ClassId { get; set; }
    }

    // DTO for comment update
    public class CommentDto
    {
        public string Comment { get; set; }
    }
}
