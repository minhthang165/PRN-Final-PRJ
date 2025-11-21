using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Repositories.Common;
using PRN_Final_Project.Service.Dto;

public interface IScheduleService : ICommonRepository<Schedule>
{
    Task<ScheduleGenerationResult> ImportAndGenerateSchedulesAsync(IFormFile file, DateOnly startDate);
    Task<List<ScheduleDisplayDto>> GetSchedulesForDisplayAsync(DateOnly? startDate = null, DateOnly? endDate = null);
    Task<List<ScheduleDisplayDto>> GetSchedulesByMentorIdAsync(int mentorId, DateOnly? startDate = null, DateOnly? endDate = null);
    Task<List<ScheduleDisplayDto>> GetSchedulesByClassIdAsync(int classId, DateOnly? startDate = null, DateOnly? endDate = null);
    Task<List<ScheduleImportDto>> PreviewExcelDataAsync(IFormFile file);
    Task<List<string>> ValidateImportDataAsync(List<ScheduleImportDto> importData);
}