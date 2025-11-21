using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Service.Dto;
using PRN_Final_Project.Repositories;
using PRN_Final_Project.Repositories.Interface;
using PRN_Final_Project.Repositories.Common;

public class ScheduleService : IScheduleService
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IClassRepository _classRepository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoomRepository _roomRepository;

    public ScheduleService(
        IScheduleRepository scheduleRepository,
        IClassRepository classRepository,
        ISubjectRepository subjectRepository,
        IUserRepository userRepository,
        IRoomRepository roomRepository)
    {
        _scheduleRepository = scheduleRepository;
        _classRepository = classRepository;
        _subjectRepository = subjectRepository;
        _userRepository = userRepository;
        _roomRepository = roomRepository;
    }

    public async Task<ScheduleGenerationResult> ImportAndGenerateSchedulesAsync(IFormFile file, DateOnly startDate)
    {
        DateOnly newStartDate = new DateOnly(2025, 11, 14);
        
        var result = new ScheduleGenerationResult();
        var importData = await ReadScheduleExcelAsync(file);
        var errors = await ValidateImportDataAsync(importData);
        if (errors.Any()) { result.Errors = errors; return result; }

        var allDtos = new List<ScheduleDto>();

        foreach (var dto in importData)
        {
            var schedules = GenerateCodeClassSchedules(dto, newStartDate);
            foreach (var schedule in schedules)
            {
                try
                {
                    await _scheduleRepository.AddAsync(schedule);
                    allDtos.Add(new ScheduleDto
                    {
                        Id = schedule.id,
                        ClassId = schedule.class_id,
                        SubjectId = schedule.subject_id,
                        RoomId = schedule.room_id,
                        DayOfWeek = schedule.day_of_week,
                        StartTime = schedule.start_time,
                        EndTime = schedule.end_time,
                        StartDate = schedule.start_date,
                        EndDate = schedule.end_date,
                        MentorId = schedule.mentor_id
                    });
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Lỗi lưu lịch cho class {dto.ClassId}, subject {dto.SubjectId}: {ex.Message}");
                }
            }
        }
        result.GeneratedSchedules = allDtos;
        result.TotalSchedulesCreated = allDtos.Count;
        result.WeeksGenerated = 5; 
        return result;
    }

    public async Task<List<ScheduleImportDto>> PreviewExcelDataAsync(IFormFile file)
    {
        return await ReadScheduleExcelAsync(file);
    }

    public async Task<List<string>> ValidateImportDataAsync(List<ScheduleImportDto> importData)
    {
        var errors = new List<string>();
        foreach (var data in importData)
        {
            var existingClass = await _classRepository.GetByIdAsync(data.ClassId);
            if (existingClass == null)
                errors.Add($"Class ID {data.ClassId} does not exist");
            var existingSubject = await _subjectRepository.GetByIdAsync(data.SubjectId);
            if (existingSubject == null)
                errors.Add($"Subject ID {data.SubjectId} does not exist");
            var existingMentor = await _userRepository.GetByIdAsync(data.MentorId);
            if (existingMentor == null || existingMentor.role != "EMPLOYEE")
                errors.Add($"Mentor ID {data.MentorId} does not exist or is not an Employee");
            if (data.LessonsPerWeek <= 0 || data.LessonsPerWeek > 10)
                errors.Add($"Lessons per week for class {data.ClassId} must be between 1 and 10");
        }
        return errors;
    }

    private async Task<List<ScheduleImportDto>> ReadScheduleExcelAsync(IFormFile file)
    {
        var importData = new List<ScheduleImportDto>();
        using (var stream = new MemoryStream())
        {
            await file.CopyToAsync(stream);
            stream.Position = 0;
            IWorkbook workbook = new XSSFWorkbook(stream);
            ISheet sheet = workbook.GetSheetAt(0);
            for (int row = 1; row <= sheet.LastRowNum; row++)
            {
                IRow excelRow = sheet.GetRow(row);
                if (excelRow == null) continue;
                var importDto = new ScheduleImportDto
                {
                    ClassId = int.Parse(excelRow.GetCell(0)?.ToString() ?? "0"),
                    SubjectId = int.Parse(excelRow.GetCell(1)?.ToString() ?? "0"),
                    LessonsPerWeek = int.Parse(excelRow.GetCell(2)?.ToString() ?? "0"),
                    MentorId = int.Parse(excelRow.GetCell(3)?.ToString() ?? "0")
                };
                importData.Add(importDto);
            }
        }
        return importData;
    }

    private List<Schedule> GenerateCodeClassSchedules(ScheduleImportDto dto, DateOnly startDate)
    {
        var schedules = new List<Schedule>();
        var daysOfWeek = new[] { 1, 2, 3, 4, 5 }; 
        var slotDuration = TimeSpan.FromMinutes(180); 
        var slotStartTimes = new[]
        {
            new TimeOnly(8, 00), 
            new TimeOnly(14, 00) 
        };
        var allRooms = _roomRepository.GetAllAsync().Result;
        

        var existingSchedules = _scheduleRepository.GetSchedulesInRange(startDate, startDate.AddDays(34)).Result;
        
        var scheduledRoomSlots = new List<(DateOnly Date, TimeOnly StartTime, TimeOnly EndTime, int RoomId)>();
        var scheduledMentorSlots = new List<(DateOnly Date, TimeOnly StartTime, TimeOnly EndTime, int MentorId)>();
        var scheduledClassDays = new List<(int Week, int Day)>(); 
        var scheduledClassSlots = new List<(DateOnly Date, int SlotIndex)>(); 
        
        foreach (var existingSchedule in existingSchedules)
        {
            scheduledRoomSlots.Add((existingSchedule.start_date, existingSchedule.start_time, existingSchedule.end_time, existingSchedule.room_id));
            if (existingSchedule.mentor_id.HasValue)
            {
                scheduledMentorSlots.Add((existingSchedule.start_date, existingSchedule.start_time, existingSchedule.end_time, existingSchedule.mentor_id.Value));
            }
        }

        for (int week = 0; week < 5; week++)
        {
            var weekStartDate = startDate.AddDays(week * 7);
            int lessonsScheduledThisWeek = 0;
            
            if (dto.LessonsPerWeek > 2)
            {
                int spacing = Math.Max(1, 5 / dto.LessonsPerWeek);
                
                var possibleDays = new List<int>();
                for (int i = 0; i < dto.LessonsPerWeek; i++)
                {
                    int day = 1 + (i * spacing);
                    if (day <= 5) 
                    {
                        possibleDays.Add(day);
                    }
                }
                
                foreach (var day in possibleDays)
                {
                    if (lessonsScheduledThisWeek >= dto.LessonsPerWeek)
                        break;
                    
                    foreach (var slotIndex in new[] { 0, 1 })
                    {
                        if (lessonsScheduledThisWeek >= dto.LessonsPerWeek)
                            break;
                        
                        var slotStart = slotStartTimes[slotIndex];
                        var startTime = slotStart;
                        var endTime = startTime.Add(slotDuration);
                        var scheduleDate = weekStartDate.AddDays(day - 1);
                        
 
                        bool hasMorningSession = scheduledClassSlots.Any(x => 
                            x.Date == scheduleDate && x.SlotIndex == 0);
                        
                        if (slotIndex == 1 && hasMorningSession)
                        {
                            continue;
                        }
                        
                        int? availableRoomId = null;
                        foreach (var room in allRooms)
                        {
                            bool roomConflict = scheduledRoomSlots.Any(x => 
                                x.Date == scheduleDate && 
                                x.RoomId == room.id &&
                                ((startTime < x.EndTime && endTime > x.StartTime))); 
                                
                            bool mentorConflict = scheduledMentorSlots.Any(x => 
                                x.Date == scheduleDate && 
                                x.MentorId == dto.MentorId &&
                                ((startTime < x.EndTime && endTime > x.StartTime))); 
                                
                            if (!roomConflict && !mentorConflict)
                            {
                                availableRoomId = room.id;
                                break;
                            }
                        }
                        
                        if (availableRoomId == null)
                        {
                            continue;
                        }
                        
                        var newSchedule = new Schedule
                        {
                            class_id = dto.ClassId,
                            subject_id = dto.SubjectId,
                            mentor_id = dto.MentorId,
                            room_id = availableRoomId.Value,
                            day_of_week = GetDayOfWeekString(day),
                            start_time = startTime,
                            end_time = endTime,
                            start_date = scheduleDate,
                            end_date = scheduleDate,
                            created_at = DateTime.Now,
                            is_active = true
                        };
                        
                        schedules.Add(newSchedule);
                        scheduledRoomSlots.Add((scheduleDate, startTime, endTime, availableRoomId.Value));
                        scheduledMentorSlots.Add((scheduleDate, startTime, endTime, dto.MentorId));
                        scheduledClassDays.Add((week, day));
                        scheduledClassSlots.Add((scheduleDate, slotIndex));
                        lessonsScheduledThisWeek++;
                    }
                }
            }
            else
            {
                foreach (var day in daysOfWeek)
                {
                    if (lessonsScheduledThisWeek >= dto.LessonsPerWeek)
                        break;
                    bool hasRecentSchedule = false;
                    hasRecentSchedule = scheduledClassDays.Any(x => x.Week == week && Math.Abs(x.Day - day) < 3);
                    
                    if (hasRecentSchedule)
                    {
                        continue;
                    }
                    
                    foreach (var slotIndex in new[] { 0, 1 }) 
                    {
                        if (lessonsScheduledThisWeek >= dto.LessonsPerWeek)
                            break;
                        
                        var slotStart = slotStartTimes[slotIndex];
                        var startTime = slotStart;
                        var endTime = startTime.Add(slotDuration);
                        var scheduleDate = weekStartDate.AddDays(day - 1);
                      
                        bool hasMorningSession = scheduledClassSlots.Any(x => 
                            x.Date == scheduleDate && x.SlotIndex == 0);
                        
                        if (slotIndex == 1 && hasMorningSession)
                        {
                            continue;
                        }
                        
                        int? availableRoomId = null;
                        foreach (var room in allRooms)
                        {
                            bool roomConflict = scheduledRoomSlots.Any(x => 
                                x.Date == scheduleDate && 
                                x.RoomId == room.id &&
                                ((startTime < x.EndTime && endTime > x.StartTime)));
                            bool mentorConflict = scheduledMentorSlots.Any(x => 
                                x.Date == scheduleDate && 
                                x.MentorId == dto.MentorId &&
                                ((startTime < x.EndTime && endTime > x.StartTime))); // overlap
                                
                            if (!roomConflict && !mentorConflict)
                            {
                                availableRoomId = room.id;
                                break;
                            }
                        }
                        
                        if (availableRoomId == null)
                        {
                            continue;
                        }
                        
                        var newSchedule = new Schedule
                        {
                            class_id = dto.ClassId,
                            subject_id = dto.SubjectId,
                            mentor_id = dto.MentorId,
                            room_id = availableRoomId.Value,
                            day_of_week = GetDayOfWeekString(day),
                            start_time = startTime,
                            end_time = endTime,
                            start_date = scheduleDate,
                            end_date = scheduleDate,
                            created_at = DateTime.Now,
                            is_active = true
                        };
                        
                        schedules.Add(newSchedule);
                        scheduledRoomSlots.Add((scheduleDate, startTime, endTime, availableRoomId.Value));
                        scheduledMentorSlots.Add((scheduleDate, startTime, endTime, dto.MentorId));
                        scheduledClassDays.Add((week, day));
                        scheduledClassSlots.Add((scheduleDate, slotIndex));
                        lessonsScheduledThisWeek++;
                    }
                }
            }
        }
        
        return schedules;
    }

    private int GetFirstAvailableRoomId()
    {
        var rooms = _roomRepository.GetAllAsync().Result;
        return rooms.FirstOrDefault()?.id ?? 1;
    }

    private string GetDayOfWeekString(int dayOfWeek)
    {
        return dayOfWeek switch
        {
            1 => "Monday",
            2 => "Tuesday",
            3 => "Wednesday",
            4 => "Thursday",
            5 => "Friday",
            6 => "Saturday",
            7 => "Sunday",
            _ => "Monday"
        };
    }

    public Task<List<Schedule>> GetAllAsync()
    {
        return _scheduleRepository.GetAllAsync();
    }

    public Task<Page<Schedule>> GetAllPagingAsync(string? searchKey = "", int page = 1, int pageSize = 10)
    {
        if (page < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(page), "Page number must be greater than or equal to 1");
        }
        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than or equal to 1");
        }
        return _scheduleRepository.GetAllPagingAsync(searchKey, page, pageSize);
    }

    public Task<Schedule?> GetByIdAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Invalid subject ID", nameof(id));
        }
        return _scheduleRepository.GetByIdAsync(id);
    }

    public Task AddAsync(Schedule entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity), "Schedule entity cannot be null");
        }
        return _scheduleRepository.AddAsync(entity);
    }

    public Task UpdateAsync(Schedule entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity), "Schedule entity cannot be null");
        }
        if (entity.id <= 0)
        {
            throw new ArgumentException("Invalid schedule ID", nameof(entity.id));
        }
        return _scheduleRepository.UpdateAsync(entity);
    }

    public Task DeleteAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Invalid schedule ID", nameof(id));
        }
        return _scheduleRepository.DeleteAsync(id);
    }

    public async Task<List<ScheduleDisplayDto>> GetSchedulesForDisplayAsync(DateOnly? startDate = null, DateOnly? endDate = null)
    {
        var schedules = await _scheduleRepository.GetAllAsync();
        Console.WriteLine($"Total schedules in database: {schedules.Count}");

        if (startDate.HasValue && endDate.HasValue)
        {
            schedules = schedules.Where(s =>
                (s.start_date <= endDate.Value && s.end_date >= startDate.Value)
            ).ToList();

            Console.WriteLine($"Filtered schedules for date range {startDate} to {endDate}: {schedules.Count}");
        }

        var displayDtos = new List<ScheduleDisplayDto>();

        foreach (var schedule in schedules)
        {
            try
            {
                var classEntity = await _classRepository.GetByIdAsync(schedule.class_id);
                var subjectEntity = await _subjectRepository.GetByIdAsync(schedule.subject_id);
                var roomEntity = await _roomRepository.GetByIdAsync(schedule.room_id);
                var mentorEntity = schedule.mentor_id.HasValue ? await _userRepository.GetByIdAsync(schedule.mentor_id.Value) : null;
                Console.WriteLine($"Processing schedule: ID={schedule.id}, Room={roomEntity?.room_name}, Day={schedule.day_of_week}, " +
                                  $"Time={schedule.start_time}-{schedule.end_time}, Date={schedule.start_date}-{schedule.end_date}");

                displayDtos.Add(new ScheduleDisplayDto
                {
                    Id = schedule.id,
                    ClassName = classEntity?.class_name ?? "Unknown Class",
                    SubjectName = subjectEntity?.subject_name ?? "Unknown Subject",
                    RoomName = roomEntity?.room_name ?? "Unknown Room",
                    RoomId = schedule.room_id,
                    MentorName = mentorEntity != null ? $"{mentorEntity.first_name} {mentorEntity.last_name}".Trim() : "Unknown Mentor",
                    DayOfWeek = schedule.day_of_week,
                    StartTime = schedule.start_time,
                    EndTime = schedule.end_time,
                    StartDate = schedule.start_date,
                    EndDate = schedule.end_date,
                    MentorId = schedule.mentor_id
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing schedule ID {schedule.id}: {ex.Message}");
            }
        }

        return displayDtos;
    }

    public async Task<List<ScheduleDisplayDto>> GetSchedulesByMentorIdAsync(int mentorId, DateOnly? startDate = null, DateOnly? endDate = null)
    {
        var schedules = await _scheduleRepository.GetAllAsync();

        schedules = schedules.Where(s => s.mentor_id == mentorId).ToList();

        if (startDate.HasValue && endDate.HasValue)
        {
            schedules = schedules.Where(s => s.start_date >= startDate.Value && s.start_date <= endDate.Value).ToList();
        }

        var displayDtos = new List<ScheduleDisplayDto>();

        foreach (var schedule in schedules)
        {
            var classEntity = await _classRepository.GetByIdAsync(schedule.class_id);
            var subjectEntity = await _subjectRepository.GetByIdAsync(schedule.subject_id);
            var roomEntity = await _roomRepository.GetByIdAsync(schedule.room_id);
            var mentorEntity = schedule.mentor_id.HasValue ? await _userRepository.GetByIdAsync(schedule.mentor_id.Value) : null;

            displayDtos.Add(new ScheduleDisplayDto
            {
                Id = schedule.id,
                ClassName = classEntity?.class_name ?? "Unknown Class",
                SubjectName = subjectEntity?.subject_name ?? "Unknown Subject",
                RoomName = roomEntity?.room_name ?? "Unknown Room",
                RoomId = schedule.room_id,
                MentorName = mentorEntity != null ? $"{mentorEntity.first_name} {mentorEntity.last_name}".Trim() : "Unknown Mentor",
                DayOfWeek = schedule.day_of_week,
                StartTime = schedule.start_time,
                EndTime = schedule.end_time,
                StartDate = schedule.start_date,
                EndDate = schedule.end_date,
                MentorId = schedule.mentor_id
            });
        }

        return displayDtos;
    }

    private class TimeSlot
    {
        public int DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }

    public async Task<List<ScheduleDisplayDto>> GetSchedulesByClassIdAsync(int classId, DateOnly? startDate = null, DateOnly? endDate = null)
    {
        var schedules = await _scheduleRepository.GetAllAsync();

        schedules = schedules.Where(s => s.class_id == classId).ToList();

        if (startDate.HasValue && endDate.HasValue)
        {
            schedules = schedules.Where(s => s.start_date >= startDate.Value && s.start_date <= endDate.Value).ToList();
        }

        var displayDtos = new List<ScheduleDisplayDto>();

        foreach (var schedule in schedules)
        {
            var classEntity = await _classRepository.GetByIdAsync(schedule.class_id);
            var subjectEntity = await _subjectRepository.GetByIdAsync(schedule.subject_id);
            var roomEntity = await _roomRepository.GetByIdAsync(schedule.room_id);
            var mentorEntity = schedule.mentor_id.HasValue ? await _userRepository.GetByIdAsync(schedule.mentor_id.Value) : null;

            displayDtos.Add(new ScheduleDisplayDto
            {
                Id = schedule.id,
                ClassName = classEntity?.class_name ?? "Unknown Class",
                SubjectName = subjectEntity?.subject_name ?? "Unknown Subject",
                RoomName = roomEntity?.room_name ?? "Unknown Room",
                RoomId = schedule.room_id,
                MentorName = mentorEntity != null ? $"{mentorEntity.first_name} {mentorEntity.last_name}".Trim() : "Unknown Mentor",
                DayOfWeek = schedule.day_of_week,
                StartTime = schedule.start_time,
                EndTime = schedule.end_time,
                StartDate = schedule.start_date,
                EndDate = schedule.end_date,
                MentorId = schedule.mentor_id
            });
        }

        return displayDtos;
    }
}