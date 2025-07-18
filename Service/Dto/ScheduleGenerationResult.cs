using System.Collections.Generic;

public class ScheduleGenerationResult
{
    public int TotalSchedulesCreated { get; set; }
    public int WeeksGenerated { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<ScheduleDto> GeneratedSchedules { get; set; } = new();
}