using Microsoft.AspNetCore.Mvc;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Service.Interface;

namespace PRN_Final_Project.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        // GET: api/room
        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<Room>>> GetAll()
        {
            var rooms = await _roomService.GetAllAsync();
            return Ok(rooms);
        }

        // GET: api/room/paging?searchKey=abc&page=1&pageSize=10
        [HttpGet("paging")]
        public async Task<ActionResult> GetPaging([FromQuery] string? searchKey = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var pagedResult = await _roomService.GetAllPagingAsync(searchKey, page, pageSize);
            return Ok(pagedResult);
        }

        // GET: api/room/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> GetById(int id)
        {
            var room = await _roomService.GetByIdAsync(id);
            if (room == null)
                return NotFound();
            return Ok(room);
        }

        // POST: api/room
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Room newRoom)
        {
            if (newRoom == null)
            {
                return BadRequest("Room cannot be null.");
            }
            await _roomService.AddAsync(newRoom);
            return CreatedAtAction(nameof(GetById), new { id = newRoom.id }, newRoom);
        }

        // PUT: api/room/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] Room updatedRoom)
        {
            if (updatedRoom == null || id != updatedRoom.id)
            {
                return BadRequest("Room ID mismatch or room data is null.");
            }
            await _roomService.UpdateAsync(updatedRoom);
            return NoContent();
        }

        // DELETE: api/room/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existingRoom = await _roomService.GetByIdAsync(id);
            if (existingRoom == null)
            {
                return NotFound($"Room with ID {id} not found.");
            }
            await _roomService.DeleteAsync(id);
            return NoContent();
        }

    }
}
