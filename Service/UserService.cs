using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Repositories;
using PRN_Final_Project.Repositories.Common;
using PRN_Final_Project.Repositories.Interface;
using PRN_Final_Project.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN_Final_Project.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            this._repository = repository;
        }

        public async Task AddAsync(user entity)
        {
            await _repository.AddAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<List<user>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Page<user>> GetAllPagingAsync(string? searchKey = "", int page = 1, int pageSize = 10)
        {
            return await _repository.GetAllPagingAsync(searchKey, page, pageSize);
        }

        public Task<user?> GetByIdAsync(int id)
        {
            return _repository.GetByIdAsync(id);
        }

        public async Task UpdateAsync(user entity)
        {
            await _repository.UpdateAsync(entity);
        }

        public async Task<List<user>> GetByEmail(string email)
        {
            return await _repository.GetByEmail(email);
        }
        public async Task BanUser(int userId, int durationInDays, string reason)
        {
            await _repository.BanUser(userId, durationInDays, reason);
        }

        public async Task<Dictionary<string, object>> GetBanStatus(int userId)
        {
            return await _repository.GetBanStatus(userId);
        }

        public async Task UnbanUser(int userId)
        {
            await _repository.UnbanUser(userId);
        }
        public async Task<List<user>> GetTraineeByClassId(int classId)
        {
            return await _repository.GetTraineeByClassId(classId);
        }
        public async Task<user> GetOneByEmail(string email)
        {
            return await _repository.GetOneByEmail(email);
        }
    }
}