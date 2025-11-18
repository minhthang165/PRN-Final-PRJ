using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PRN_Final_Project.API.Dto;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Service.Interface;

namespace PRN_Final_Project.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecruitmentController : ControllerBase
    {
        private readonly IRecruitmentService _service;

        public RecruitmentController(IRecruitmentService service)
        {
            _service = service;
        }

         // GET: api/recruitment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recruitment>>> GetAll()
        {
            var recruitments = await _service.GetAllAsync();
            return Ok(recruitments);
        }

        // GET: api/recruitment/paging?searchKey=abc&page=1&pageSize=10
        [HttpGet("paging")]
        public async Task<ActionResult> GetPaging([FromQuery] string? searchKey = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var pagedResult = await _service.GetAllPagingAsync(searchKey, page, pageSize);
            return Ok(pagedResult);
        }

        // GET: api/recruitment/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Recruitment>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        // POST: api/recruitment
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateRecruitmentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newRecruitment = new Recruitment
            {
                name = dto.name,
                position = dto.position,
                experience_requirement = dto.experience_requirement,
                language = dto.language,
                min_GPA = dto.min_GPA,
                total_slot = dto.total_slot,
                description = dto.description,
                end_time = dto.end_time,
                class_id = dto.class_id,
                created_at = DateTime.Now,
                is_active = dto.is_active ?? true
            };

            await _service.AddAsync(newRecruitment);
            return CreatedAtAction(nameof(GetById), new { id = newRecruitment.id }, newRecruitment);
        }

        // PUT: api/recruitment/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] Recruitment updatedRecruitment)
        {
            if (id != updatedRecruitment.id)
                return BadRequest("ID mismatch");

            var existing = await _service.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            await _service.UpdateAsync(updatedRecruitment);
            return NoContent();
        }

        // DELETE: api/recruitment/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}