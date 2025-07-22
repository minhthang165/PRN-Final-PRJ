using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Service.Common;

namespace PRN_Final_Project.Service.Interface
{
    public interface IClassService : ICommonService<Class>
    {
        Task AssignTrainerToClassAsync(int classId, int trainerId);
        Task AssignTraineeToClassAsync(int classId, int traineeId);
        Task<List<Class>> GetClassesByMentorId(int userId);
    }
}