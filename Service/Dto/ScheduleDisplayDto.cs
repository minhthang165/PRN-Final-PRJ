using System;

namespace PRN_Final_Project.Service.Dto
{
    public class ScheduleDisplayDto
    {
        public int Id { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public string MentorName { get; set; } = string.Empty;
        public string DayOfWeek { get; set; } = string.Empty;
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int? MentorId { get; set; }
    }
}