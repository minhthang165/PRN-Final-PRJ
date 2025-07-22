using System.Collections.Generic;

namespace PRN_Final_Project.Service.Dto
{
    public class AdminScheduleViewModel
    {
        public List<RoomViewModel> Rooms { get; set; }
        public List<string> TimeSlots { get; set; }
        public List<ScheduleCellViewModel> ScheduleCells { get; set; }
        public DateTime CurrentDate { get; set; }
        public List<ScheduleDisplayDto> Schedules { get; set; }
    }

    public class RoomViewModel
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; }
    }

    public class ScheduleCellViewModel
    {
        public int RoomId { get; set; }
        public string TimeSlot { get; set; }
        public string Subject { get; set; }
        public string ClassName { get; set; }
        public string Teacher { get; set; }
        public string Color { get; set; }
        public string TimeRange { get; set; } 
    }
}