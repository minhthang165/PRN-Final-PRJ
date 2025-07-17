using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Repositories.Interface;
using PRN_Final_Project.Service.Interface;

namespace PRN_Final_Project.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<user>>> GetAll()
        {
            var users = await _service.GetAllAsync();
            return Ok(users);
        }

        // GET: api/user/paging?searchKey=abc&page=1&pageSize=10
        [HttpGet("paging")]
        public async Task<ActionResult> GetPaging([FromQuery] string? searchKey = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var pagedResult = await _service.GetAllPagingAsync(searchKey, page, pageSize);
            return Ok(pagedResult);
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<user>> GetById(int id)
        {
            var user = await _service.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // GET: api/User/email?value=example@email.com
        [HttpGet("email")]
        public async Task<ActionResult<user>> GetByEmail([FromQuery] string value)
        {
            var user = await _service.GetByEmail(value);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] user user)
        {
            await _service.AddAsync(user);
            return CreatedAtAction(nameof(GetById), new { id = user.id }, user);
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] user user)
        {
            if (id != user.id)
                return BadRequest("User ID mismatch");

            await _service.UpdateAsync(user);
            return NoContent();
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        // GET: api/User/class/5
        [HttpGet("class/{classId}")]
        public async Task<ActionResult<List<user>>> GetTraineeByClassId(int classId)
        {
            var trainees = await _service.GetTraineeByClassId(classId);
            if (trainees == null || !trainees.Any())
                return NotFound();
            return Ok(trainees);
        }
    }
}