using Microsoft.AspNetCore.Mvc;
using PRN_Final_Project.Service.Dto;
using PRN_Final_Project.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN_Final_Project.Controllers.Admin
{
    public class AdminScheduleController : Controller
    {
        private readonly IRoomService _roomService;
        private readonly IScheduleService _scheduleService;

        public AdminScheduleController(IRoomService roomService, IScheduleService scheduleService)
        {
            _roomService = roomService;
            _scheduleService = scheduleService;
        }

        [HttpGet]
        [Route("/AdminSchedule")]
        public async Task<IActionResult> Index(DateTime? date)
        {
            try
            {
                var currentDate = date ?? DateTime.Today;

                var rooms = (await _roomService.GetAllAsync())
                    .Select(r => new RoomViewModel { RoomId = r.id, RoomName = r.room_name })
                    .ToList();

                // Logic giữ nguyên: nếu không có phòng thì tạo mặc định
                if (!rooms.Any())
                {
                    var defaultRooms = new[] { "Big Data", ".NET", "Android", "Java", "Cloud", "C++", "IOS", "DevOps" };
                    for (int i = 0; i < defaultRooms.Length; i++)
                    {
                        rooms.Add(new RoomViewModel { RoomId = i + 1, RoomName = defaultRooms[i] });
                    }
                }

                var timeSlots = new List<string> { "07:00", "08:00", "09:00", "10:00", "11:00", "12:00", "13:00", "14:00", "15:00", "16:00", "17:00" };

                // Lấy và lọc lịch học (nên được tối ưu ở database)
                var allSchedules = await _scheduleService.GetSchedulesForDisplayAsync(null, null);
                var dateOnly = DateOnly.FromDateTime(currentDate);
                string currentDayOfWeek = currentDate.DayOfWeek.ToString();

                var filteredSchedules = allSchedules.Where(s =>
                    (s.StartDate <= dateOnly && s.EndDate >= dateOnly) &&
                    s.DayOfWeek.Equals(currentDayOfWeek, StringComparison.OrdinalIgnoreCase)
                ).ToList();

                var scheduleCells = new List<ScheduleCellViewModel>();

                foreach (var schedule in filteredSchedules)
                {
                    var room = rooms.FirstOrDefault(r => r.RoomId == schedule.RoomId);
                    if (room != null)
                    {
                        string startHour = schedule.StartTime.ToString("HH:00");
                        scheduleCells.Add(new ScheduleCellViewModel
                        {
                            RoomId = room.RoomId,
                            TimeSlot = startHour,
                            Subject = schedule.SubjectName,
                            ClassName = schedule.ClassName,
                            Teacher = schedule.MentorName,
                            // *** NÂNG CẤP: Tạo màu động cho mỗi lớp ***
                            Color = ColorHelper.GetColorFor(schedule.ClassName),
                            TimeRange = $"{schedule.StartTime:HH:mm} - {schedule.EndTime:HH:mm}"
                        });
                    }
                }

                var model = new AdminScheduleViewModel
                {
                    Rooms = rooms,
                    TimeSlots = timeSlots,
                    ScheduleCells = scheduleCells,
                    CurrentDate = currentDate
                };

                return View("~/Views/Admin/AdminSchedule.cshtml", model);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                Console.WriteLine($"Error in AdminScheduleController: {ex.Message}");
                // Trả về view lỗi hoặc view rỗng
                return View("~/Views/Admin/AdminSchedule.cshtml", new AdminScheduleViewModel
                {
                    Rooms = new List<RoomViewModel>(),
                    TimeSlots = new List<string> { "07:00", "08:00", "09:00", "10:00", "11:00", "12:00", "13:00", "14:00", "15:00", "16:00", "17:00" },
                    ScheduleCells = new List<ScheduleCellViewModel>(),
                    CurrentDate = date ?? DateTime.Today
                });
            }
        }
    }

    // Lớp helper để tạo màu động
    public static class ColorHelper
    {
        private static readonly List<string> ColorPalette = new List<string>
        {
            "#bfdbfe", "#a7f3d0", "#fef08a", "#fbcfe8", "#fed7aa", "#e9d5ff", "#d1fae5"
        };

        public static string GetColorFor(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return ColorPalette[0];
            }
            // Tạo một hash đơn giản từ tên lớp để chọn màu nhất quán
            int hash = Math.Abs(text.GetHashCode());
            int index = hash % ColorPalette.Count;
            return ColorPalette[index];
        }
    }
}