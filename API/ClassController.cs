using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Service.Interface;

namespace PRN_Final_Project.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassController : ControllerBase
    {
        private readonly IClassService _classService;

        public ClassController(IClassService classService)
        {
            _classService = classService;
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
            var result = await _classService.GetClassesByMentorId(id);
            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult> GetClassByUserId(int userId)
        {
            var result = await _classService.GetClassByUserId(userId);
            return Ok(result);
        }

    }
}