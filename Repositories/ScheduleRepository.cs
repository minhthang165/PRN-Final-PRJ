using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PRN_Final_Project.Business.Data;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Repositories.Common;

public class ScheduleRepository : IScheduleRepository
{
    private readonly PRNDbContext _context;
    public ScheduleRepository(PRNDbContext context)
    {
        _context = context;
    }
    public async Task AddAsync(Schedule entity)
    {
        _context.Schedules.Add(entity);
        await _context.SaveChangesAsync();
    }

    public Task DeleteAsync(int id)
    {
        var schedule = _context.Schedules.Find(id);
        if (schedule != null)
        {
            _context.Schedules.Remove(schedule);
            return _context.SaveChangesAsync();
        }
        return Task.CompletedTask; // or throw an exception if preferred
    }

    public Task<List<Schedule>> GetAllAsync()
    {
        // Include related entities to avoid multiple database queries
        return _context.Schedules
            .Include(s => s._class)
            .Include(s => s.subject)
            .Include(s => s.room)
            .Include(s => s.mentor)
            .ToListAsync(); // Remove the is_active filter to get all schedules
    }

    public Task<Page<Schedule>> GetAllPagingAsync(string? searchKey = "", int page = 1, int pageSize = 10)
    {
        // Assuming you want to implement paging functionality
        var query = _context.Schedules.AsQueryable();
        if (!string.IsNullOrEmpty(searchKey))
        {
            query = query.Where(s => s.subject.subject_name.Contains(searchKey) || s.room.room_name.Contains(searchKey));
        }
        var totalItems = query.Count();
        var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult(new Page<Schedule>
        {
            Items = items,
            TotalItems = totalItems,
            PageNumber = page,
            PageSize = pageSize
        });
    }

    public Task<Schedule?> GetByIdAsync(int id)
    {
        return _context.Schedules.FindAsync(id).AsTask();
    }

    public Task<List<Schedule>> GetSchedulesByClassIdAsync(int classId)
    {
        return _context.Schedules
            .Where(s => s.class_id == classId).ToListAsync();
    }

    public Task<List<Schedule>> GetSchedulesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return _context.Schedules
            .Where(s => s.start_date >= DateOnly.FromDateTime(startDate) && s.end_date <= DateOnly.FromDateTime(endDate))
            .ToListAsync();
    }

    public Task<List<Schedule>> GetSchedulesByRoomIdAsync(int roomId)
    {
        return _context.Schedules.Where(s => s.Equals(roomId)).ToListAsync();
    }

    public Task<List<Schedule>> GetSchedulesBySubjectIdAsync(int subjectId)
    {
        return _context.Schedules.Where(s => s.subject_id == subjectId).ToListAsync();
    }

    public Task<List<Schedule>> GetSchedulesByUserIdAsync(int userId)
    {
        return _context.Schedules.Where(s => s.mentor_id == userId).ToListAsync();
    }

    public Task<List<Schedule>> GetSchedulesInRange(DateOnly startDate, DateOnly endDate)
    {
        return _context.Schedules
            .Where(s => s.start_date >= startDate && s.end_date <= endDate)
            .ToListAsync();
    }

    public async Task UpdateAsync(Schedule entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity), "Schedule entity cannot be null.");
        }

        var existingSchedule = await _context.Schedules.FindAsync(entity.id);
        if (existingSchedule == null)
        {
            throw new KeyNotFoundException($"Schedule with ID {entity.id} not found.");
        }
        existingSchedule.class_id = entity.class_id;
        existingSchedule.subject_id = entity.subject_id;
        existingSchedule.room_id = entity.room_id;
        existingSchedule.day_of_week = entity.day_of_week;
        existingSchedule.start_time = entity.start_time;
        existingSchedule.end_time = entity.end_time;
        existingSchedule.start_date = entity.start_date;
        existingSchedule.end_date = entity.end_date;
        existingSchedule.mentor_id = entity.mentor_id;
        existingSchedule.is_active = entity.is_active;
        existingSchedule.updated_at = DateTime.Now;
        existingSchedule.updated_by = entity.updated_by;

        _context.Schedules.Update(existingSchedule);
        await _context.SaveChangesAsync();
    }
}