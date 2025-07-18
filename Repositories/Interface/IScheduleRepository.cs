using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Repositories.Common;

public interface IScheduleRepository : ICommonRepository<Schedule>
{
    Task<List<Schedule>> GetSchedulesByClassIdAsync(int classId);
    Task<List<Schedule>> GetSchedulesBySubjectIdAsync(int subjectId);
    Task<List<Schedule>> GetSchedulesByUserIdAsync(int userId);
    Task<List<Schedule>> GetSchedulesByRoomIdAsync(int roomId);
    Task<List<Schedule>> GetSchedulesInRange(DateOnly startDate, DateOnly endDate);
}