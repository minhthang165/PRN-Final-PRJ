using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using PRN_Final_Project.Service;
using PRN_Final_Project.Service.Interface;

namespace PRN_Final_Project.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoomService _roomService;
        private readonly IScheduleService _scheduleService;
        public ExcelController(IUserService userService, IRoomService roomService, IScheduleService scheduleService)
        {
            _userService = userService;
            _roomService = roomService;
            _scheduleService = scheduleService;
        }

        [HttpPost("import-employees")]
        public async Task<IActionResult> ImportEmployees(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var result = await _userService.ImportEmployeesFromExcelAsync(file);
            return Ok(result);
        }

        [HttpPost("import-room")]
        public async Task<ActionResult<ImportResult>> ImportRoomsFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File cannot be null or empty.");
            }
            try
            {
                var result = await _roomService.ImportRoomsFromExcelAsync(file);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("import-schedule")]
        public async Task<IActionResult> ImportSchedule(IFormFile file, [FromQuery] DateOnly startDate)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");
            var result = await _scheduleService.ImportAndGenerateSchedulesAsync(file, startDate);
            if (result.Errors.Count > 0)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("template")]
        public IActionResult DownloadTemplate()
        {
            // Logic to generate and download template
            var templateGenerator = new ExcelTemplateGenerator();
            var fileBytes = templateGenerator.GenerateScheduleTemplate();
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "schedule_template.xlsx");
        }

        [HttpPost("scheduling/generate")]
        public async Task<IActionResult> GenerateSchedule(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // Use July 14, 2025 as the reference date
            var startDate = new DateOnly(2025, 11, 14);

            var result = await _scheduleService.ImportAndGenerateSchedulesAsync(file, startDate);
            if (result.Errors.Count > 0)
                return BadRequest(result);
            return Ok(result);
        }
    }
}