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
        // GET: api/user?page=0&pageSize=10
        [HttpGet]
        public async Task<ActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var pagedResult = await _userService.GetAllPagingAsync(string.Empty, page, pageSize);
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
        // POST: api/user/create-intern
        [HttpPost("create-intern")]
        public async Task<ActionResult> CreateIntern([FromBody] user intern)
        {
            intern.role = "INTERN";
            await _userService.AddAsync(intern);
            return CreatedAtAction(nameof(GetById), new { id = intern.id }, intern);
        }

        // POST: api/user/create-employee
        [HttpPost("create-employee")]
        public async Task<ActionResult> CreateEmployee([FromBody] user employee)
        {
            employee.role = "EMPLOYEE";
            await _userService.AddAsync(employee);
            return CreatedAtAction(nameof(GetById), new { id = employee.id }, employee);
        }

        // PUT: api/user/update/5
        [HttpPut("update")]
        public async Task<ActionResult> Update([FromBody] user updatedUser)
        {
            var existingUser = await _userService.GetByEmail(updatedUser.email);
            if (existingUser == null)
                return NotFound();

            // Only update the properties that are provided in the updatedUser
            if (updatedUser.first_name != null)
                existingUser.first_name = updatedUser.first_name;
            if (updatedUser.last_name != null)
                existingUser.last_name = updatedUser.last_name;
            if (updatedUser.user_name != null)
                existingUser.user_name = updatedUser.user_name;
            if (updatedUser.phone_number != null)
                existingUser.phone_number = updatedUser.phone_number;
            if (updatedUser.gender != null)
                existingUser.gender = updatedUser.gender;
            if (updatedUser.role != null)
                existingUser.role = updatedUser.role;
            if (updatedUser.avatar_path != null)
                existingUser.avatar_path = updatedUser.avatar_path;
            if (updatedUser.is_active.HasValue)
                existingUser.is_active = updatedUser.is_active;
            // Note: We're not updating email here

            existingUser.updated_at = DateTime.Now;
            existingUser.updated_by = updatedUser.updated_by;

            await _userService.UpdateAsync(existingUser);
            return Ok(existingUser);
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
        public async Task<ActionResult> Delete(int id)
        {
            var existing = await _userService.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            await _userService.DeleteAsync(id);
            return Ok(existing);
        }

        // PATCH: api/user/activate/5
        [HttpPatch("activate/{id}")]
        public async Task<ActionResult> Activate(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            user.is_active = true;
            await _userService.UpdateAsync(user);
            return Ok(user);
        }

        // GET: api/user/role/INTERN?page=1&pageSize=10
        [HttpGet("role/{role}")]
        public async Task<ActionResult> GetByRole(string role, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var pagedResult = await _userService.GetAllPagingAsync(role, page, pageSize);
            return Ok(pagedResult);
        }

        // POST: api/user/ban/5
        [HttpPost("ban/{id}")]
        public async Task<ActionResult> BanUser(int id, [FromBody] BanUserRequest request)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            // This would need a real implementation in the service
            user.is_active = false;
            await _userService.UpdateAsync(user);

            // Store ban information (would need a real implementation)
            return Ok(new { message = $"User {id} banned for {request.Duration} days. Reason: {request.Reason}" });
        }

        // DELETE: api/user/unban/5
        [HttpDelete("unban/{id}")]
        public async Task<ActionResult> UnbanUser(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            user.is_active = true;
            await _userService.UpdateAsync(user);
            return Ok(new { message = $"User {id} unbanned" });
        }

        // GET: api/user/search?email=example
        [HttpGet("search")]
        public async Task<ActionResult> SearchByEmail([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email search term cannot be empty");

            var user = await _userService.GetByEmail(email);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // GET: api/user/classroom/5
        [HttpGet("classroom/{classId}")]
        public async Task<ActionResult> GetUsersByClassId(int classId)
        {
            // This would need implementation in the service
            // For now, return a placeholder
            return Ok(new { message = $"Users for classroom {classId} would be returned here" });
        }
    }

    public class BanUserRequest
    {
        public int Duration { get; set; } // Days
        public string Reason { get; set; }
    }
}