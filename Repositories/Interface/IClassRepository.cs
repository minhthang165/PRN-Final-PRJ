using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Repositories.Common;

namespace PRN_Final_Project.Repositories.Interface
{
    public interface IClassRepository : ICommonRepository<Class>
    {
        Task AssignTrainerToClassAsync(int classId, int trainerId);
    }
}