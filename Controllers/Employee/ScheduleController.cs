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
        public async Task<IActionResult> Schedule()
        {
            try
            {
                // Lấy userId từ claim
                int? userId = null;
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                                ?? User.FindFirst("UserId")
                                ?? User.FindFirst("Id")
                                ?? User.FindFirst(ClaimTypes.Name);

                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedId))
                {
                    userId = parsedId;
                }

                // Tính toán tuần hiện tại (thứ 2 đến thứ 6)
                var today = DateOnly.FromDateTime(DateTime.Today);
                int offset = today.DayOfWeek == DayOfWeek.Sunday ? -6 : (DayOfWeek.Monday - today.DayOfWeek);
                var currentWeekStart = today.AddDays(offset);
                var currentWeekEnd = currentWeekStart.AddDays(4); // Thứ 6

                List<ScheduleDisplayDto> schedules = new List<ScheduleDisplayDto>();
                if (userId.HasValue)
                {
                    // Lấy tất cả lịch của user
                    var allSchedules = await _scheduleService.GetSchedulesByMentorIdAsync(userId.Value);

                    // Lọc lịch chỉ trong tuần hiện tại
                    schedules = allSchedules
                        .Where(s => s.StartDate >= currentWeekStart && s.StartDate <= currentWeekEnd)
                        .ToList();
                }

                // Log ra console tất cả các lịch sẽ hiển thị
                Console.WriteLine($"[DEBUG] Schedules to display for userId {userId} (week {currentWeekStart} - {currentWeekEnd}):");
                foreach (var s in schedules)
                {
                    Console.WriteLine($"[SCHEDULE] Id: {s.Id}, Subject: {s.SubjectName}, Class: {s.ClassName}, Room: {s.RoomName}, Mentor: {s.MentorName}, Date: {s.StartDate}, {s.StartTime} - {s.EndTime}");
                }

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
                Console.WriteLine($"[DEBUG] Error in Schedule action: {ex.Message}");
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
