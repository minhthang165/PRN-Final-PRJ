using Microsoft.AspNetCore.Mvc;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Service.Interface;

namespace PRN_Final_Project.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;
        public SubjectController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subject>>> GetAllSubjects()
        {
            var subjects = await _subjectService.GetAllAsync();
            return Ok(subjects);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Subject>> GetSubjectById(int id)
        {
            var subject = await _subjectService.GetByIdAsync(id);
            if (subject == null)
            {
                return NotFound();
            }
            return Ok(subject);
        }
        [HttpPost]
        public async Task<ActionResult> CreateSubject(Subject subject)
        {
            await _subjectService.AddAsync(subject);
            return CreatedAtAction(nameof(GetSubjectById), new { id = subject.id }, subject);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateSubject(int id, Subject subject)
        {
            if (id != subject.id)
            {
                return BadRequest();
            }
            await _subjectService.UpdateAsync(subject);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSubject(int id)
        {
            await _subjectService.DeleteAsync(id);
            return NoContent();
        }
    }
}
