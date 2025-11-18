using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Service.Interface;
using Microsoft.EntityFrameworkCore;
using PRN_Final_Project.Business.Data;

namespace PRN_Final_Project.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassController : ControllerBase
    {
        private readonly IClassService _classService;
        private readonly PRNDbContext _context;

        public ClassController(IClassService classService, PRNDbContext context)
        {
            _classService = classService;
            _context = context;
        }

        // GET: api/class
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Class>>> GetAll()
        {
            var classes = await _classService.GetAllAsync();
            return Ok(classes);
        }

        // GET: api/class/paging?searchKey=abc&page=1&pageSize=10
        [HttpGet("paging")]
        public async Task<ActionResult> GetPaging([FromQuery] string? searchKey = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var pagedResult = await _classService.GetAllPagingAsync(searchKey, page, pageSize);
            return Ok(pagedResult);
        }

        // GET: api/class/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Class>> GetById(int id)
        {
            var result = await _classService.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        // POST: api/class
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Class newClass)
        {
            await _classService.AddAsync(newClass);
            return CreatedAtAction(nameof(GetById), new { id = newClass.id }, newClass);
        }

        // PUT: api/class/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] Class updatedClass)
        {
            if (id != updatedClass.id)
                return BadRequest("ID mismatch");

            var existing = await _classService.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            await _classService.UpdateAsync(updatedClass);
            return NoContent();
        }

        // PATCH: api/class/update/5
        [HttpPatch("update/{id}")]
        public async Task<ActionResult> UpdatePartial(int id, [FromBody] Class updatedClass)
        {
            var existing = await _classService.GetByIdAsync(id);
            if (existing == null)
                return NotFound("Class not found");

            // Update only the fields that are provided
            if (!string.IsNullOrEmpty(updatedClass.class_name))
                existing.class_name = updatedClass.class_name;
            
            if (updatedClass.number_of_interns.HasValue)
                existing.number_of_interns = updatedClass.number_of_interns;
            
            if (!string.IsNullOrEmpty(updatedClass.status))
                existing.status = updatedClass.status;
            
            if (updatedClass.mentor_id.HasValue)
                existing.mentor_id = updatedClass.mentor_id;
            
            existing.updated_at = DateTime.Now;

            await _classService.UpdateAsync(existing);
            return Ok(existing);
        }

        // PATCH: api/class/setIsActiveTrue/5
        [HttpPatch("setIsActiveTrue/{id}")]
        public async Task<ActionResult> SetIsActiveTrue(int id)
        {
            var existing = await _classService.GetByIdAsync(id);
            if (existing == null)
                return NotFound("Class not found");

            existing.is_active = true;
            existing.updated_at = DateTime.Now;

            await _classService.UpdateAsync(existing);
            return Ok(existing);
        }

        // DELETE: api/class/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existing = await _classService.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            await _classService.DeleteAsync(id);
            return NoContent();
        }
        [HttpPost("assign-trainer")]
        public async Task<IActionResult> AssignTrainer(int classId, int trainerId)
        {
            try
            {
                await _classService.AssignTrainerToClassAsync(classId, trainerId);
                return Ok("Trainer assigned successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("assign-trainee")]
        public async Task<IActionResult> AssignTrainee(int classId, int traineeId)
        {
            try
            {
                await _classService.AssignTraineeToClassAsync(classId, traineeId);
                return Ok("Trainee assigned successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("mentor/{id}")]
        public async Task<ActionResult> GetClassByMentorId(int id)
        {
            try
            {
                var result = await _classService.GetClassesByMentorId(id);
                
                if (result == null || !result.Any())
                {
                    return Ok(new List<Class>()); // Return empty list instead of NotFound
                }
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult> GetClassByUserId(int userId)
        {
            var result = await _classService.GetClassByUserId(userId);
            return Ok(result);
        }

        [HttpGet("task/{classId}")]
        public async Task<ActionResult> GetTasksByClassId(int classId)
        {
            try
            {
                var tasks = await _context.UserTasks
                    .Where(t => t.class_id == classId && t.is_active == true)
                    .OrderByDescending(t => t.created_at)
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
                    .ToListAsync();

                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

    }
}