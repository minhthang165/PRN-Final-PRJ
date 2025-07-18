using Microsoft.AspNetCore.Mvc;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN_Final_Project.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/user
        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<user>>> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        // GET: api/user/paging?searchKey=abc&page=1&pageSize=10
        [HttpGet("paging")]
        public async Task<ActionResult> GetPaging([FromQuery] string? searchKey = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var pagedResult = await _userService.GetAllPagingAsync(searchKey, page, pageSize);
            return Ok(pagedResult);
        }

        // GET: api/user/5
        [HttpGet("{id}")]
        public async Task<ActionResult<user>> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        // POST: api/user
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] user newUser)
        {
            await _userService.AddAsync(newUser);
            return CreatedAtAction(nameof(GetById), new { id = newUser.id }, newUser);
        }

        // PUT: api/user/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] user updatedUser)
        {
            if (id != updatedUser.id)
                return BadRequest("User ID mismatch");
            var existingUser = await _userService.GetByIdAsync(id);
            if (existingUser == null)
                return NotFound();
            await _userService.UpdateAsync(updatedUser);
            return NoContent();
        }

        // DELETE: api/user/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingUser = await _userService.GetByIdAsync(id);
            if (existingUser == null)
                return NotFound();
            await _userService.DeleteAsync(id);
            return NoContent();
        }
        // GET: api/user/role/employee
        [HttpGet("role/employee")]
        public async Task<ActionResult<IEnumerable<user>>> GetEmployees()
        {
            var employees = await _userService.GetUsersByRoleAsync("EMPLOYEE");
            if (employees == null || !employees.Any())
                return NotFound();
            return Ok(employees);
        }

        // GET: api/user/paging?searchKey=abc&page=1&pageSize=10
        [HttpGet("paging/employee")]
        public async Task<ActionResult> GetEmployeePaging([FromQuery] string? searchKey = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var pagedResult = await _userService.GetUsersByRolePagingAsync("EMPLOYEE", searchKey, page, pageSize);
            return Ok(pagedResult);
        }
    }
}