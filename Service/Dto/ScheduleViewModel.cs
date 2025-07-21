using System;
using System.Collections.Generic;

namespace PRN_Final_Project.Service.Dto
{
    public class ScheduleViewModel
    {
        public List<ScheduleDisplayDto> Schedules { get; set; } = new List<ScheduleDisplayDto>();
        public DateOnly CurrentWeekStart { get; set; }
        public DateOnly CurrentWeekEnd { get; set; }
        public string CurrentMonthYear { get; set; } = string.Empty;
    }
}