using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Repositories.Common;
using PRN_Final_Project.Models;

namespace PRN_Final_Project.Repositories.Interface
{
    public interface IUserRepository : ICommonRepository<user>
    {
        Task<user> GetByEmail(string email);
<<<<<<< HEAD
        Task BanUser(int userId, int durationInDays, string reason);
        Task<Dictionary<string, object>> GetBanStatus(int userId);
        Task UnbanUser(int userId);
=======
        Task<List<user>> GetTraineeByClassId(int classId);
>>>>>>> e3373cdf77c8268bce3aa1b118a1fd64c856ce1f
    }
}