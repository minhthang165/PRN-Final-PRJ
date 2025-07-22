using Microsoft.AspNetCore.Mvc;
using PRN_Final_Project.Service;
using PRN_Final_Project.Service.Dto;
using System.Security.Claims;

namespace PRN_Final_Project.Controllers.Employee
{
    public class ScheduleController : Controller
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("/Schedule")]
        public async Task<IActionResult> Schedule(string date = null)
        {
            try
            {
                // Get userId from claim
                int? userId = null;
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                                ?? User.FindFirst("UserId")
                                ?? User.FindFirst("Id")
                                ?? User.FindFirst(ClaimTypes.Name);

                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedId))
                {
                    userId = parsedId;
                }

                // Parse date parameter if provided
                DateOnly startDate;
                if (!string.IsNullOrEmpty(date) && DateOnly.TryParse(date, out var parsedDate))
                {
                    startDate = parsedDate;
                    // Ensure we start from Monday of the week containing the specified date
                    int offset = (int)startDate.DayOfWeek;
                    if (offset == 0) offset = 7; // Sunday is 0, but we want it to be 7
                    offset = offset - 1; // Monday is 1, but we want Monday to be 0
                    startDate = startDate.AddDays(-offset);
                }
                else
                {
                    // Calculate current week (Monday to Friday)
                    var today = DateOnly.FromDateTime(DateTime.Today);
                    int offset = (int)today.DayOfWeek;
                    if (offset == 0) offset = 7; // Sunday is 0, but we want it to be 7
                    offset = offset - 1; // Monday is 1, but we want Monday to be 0
                    startDate = today.AddDays(-offset);
                }

                var currentWeekStart = startDate;
                var currentWeekEnd = currentWeekStart.AddDays(4); // Friday

                // Get all schedules for the week
                List<ScheduleDisplayDto> schedules;

                if (userId.HasValue)
                {
                    // Get all schedules for the user
                    var allSchedules = await _scheduleService.GetSchedulesByMentorIdAsync(userId.Value, currentWeekStart, currentWeekEnd);

                    // Filter schedules for the current week
                    schedules = allSchedules
                        .Where(s => s.StartDate >= currentWeekStart && s.StartDate <= currentWeekEnd)
                        .ToList();
                }
                else
                {
                    // If no user ID, get all schedules (for admin view)
                    var allSchedules = await _scheduleService.GetSchedulesForDisplayAsync(currentWeekStart, currentWeekEnd);

                    schedules = allSchedules
                        .Where(s => s.StartDate >= currentWeekStart && s.StartDate <= currentWeekEnd)
                        .ToList();
                }

                // Organize schedules by room and time for better display
                var organizedSchedules = schedules
                    .GroupBy(s => s.RoomName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.ToList()
                    );

                var viewModel = new ScheduleViewModel
                {
                    Schedules = schedules,
                    CurrentWeekStart = currentWeekStart,
                    CurrentWeekEnd = currentWeekEnd,
                    CurrentMonthYear = $"{currentWeekStart.ToString("MMMM yyyy")}" +
                        (currentWeekStart.Month != currentWeekEnd.Month ? $" - {currentWeekEnd.ToString("MMMM yyyy")}" : "")
                };

                return View("~/Views/Employee/Schedule.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error in Schedule action: {ex.Message}");
                var viewModel = new ScheduleViewModel
                {
                    Schedules = new List<ScheduleDisplayDto>(),
                    CurrentWeekStart = DateOnly.FromDateTime(DateTime.Today),
                    CurrentWeekEnd = DateOnly.FromDateTime(DateTime.Today).AddDays(4),
                    CurrentMonthYear = DateTime.Today.ToString("MMMM yyyy")
                };

                return View("~/Views/Employee/Schedule.cshtml", viewModel);
            }
        }
    }
}
