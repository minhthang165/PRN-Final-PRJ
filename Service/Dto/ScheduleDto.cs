public class ScheduleDto
{
    public int Id { get; set; }
    public int ClassId { get; set; }
    public int SubjectId { get; set; }
    public int RoomId { get; set; }
    public string DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int? MentorId { get; set; }
}
