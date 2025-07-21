using Microsoft.AspNetCore.Mvc;
using PRN_Final_Project.Service;
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
            var currentDate = date ?? DateTime.Today;
            var rooms = (await _roomService.GetAllAsync())
                .Where(r => r.is_active == true)
                .Select(r => new RoomViewModel { RoomId = r.id, RoomName = r.room_name })
                .ToList();

            var timeSlots = new List<string> { "07:00", "08:00", "09:00", "10:00", "11:00", "12:00", "13:00", "14:00", "15:00", "16:00", "17:00" };

            var schedules = await _scheduleService.GetSchedulesForDisplayAsync(null, null);
            var scheduleCells = new List<ScheduleCellViewModel>();
            var uiDayOfWeek = currentDate.DayOfWeek.ToString(); // "Monday", "Tuesday", ...

            foreach (var room in rooms)
            {
                foreach (var slot in timeSlots)
                {
                    var slotTime = TimeSpan.Parse(slot + ":00");
                    var schedule = schedules.FirstOrDefault(s =>
                        s.RoomName == room.RoomName &&
                        s.StartDate <= DateOnly.FromDateTime(currentDate) &&
                        s.EndDate >= DateOnly.FromDateTime(currentDate) &&
                        s.DayOfWeek == uiDayOfWeek &&
                        ((s.StartTime as TimeOnly?)?.ToTimeSpan() <= slotTime && (s.EndTime as TimeOnly?)?.ToTimeSpan() > slotTime)
                    );
                    if (schedule != null)
                    {
                        string startStr = (schedule.StartTime as TimeOnly?)?.ToString("HH:mm") ?? "";
                        string endStr = (schedule.EndTime as TimeOnly?)?.ToString("HH:mm") ?? "";
                        scheduleCells.Add(new ScheduleCellViewModel
                        {
                            RoomId = room.RoomId,
                            TimeSlot = slot,
                            Subject = schedule.SubjectName,
                            ClassName = schedule.ClassName,
                            Teacher = schedule.MentorName,
                            Color = "#e3eaff",
                            TimeRange = $"{startStr} - {endStr}"
                        });
                    }
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
    }
}
