using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Repositories.Common;
using PRN_Final_Project.Repositories.Interface;
using PRN_Final_Project.Service.Interface;

namespace PRN_Final_Project.Service
{
    public class ClassService : IClassService
    {
        private readonly IClassRepository _repository;

        public ClassService(IClassRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Class>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Page<Class>> GetAllPagingAsync(string? searchKey = "", int page = 1, int pageSize = 10)
        {
            return await _repository.GetAllPagingAsync(searchKey, page, pageSize);
        }

        public async Task<Class> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task AddAsync(Class classes)
        {
            await _repository.AddAsync(classes);
        }

        public async Task UpdateAsync(Class classes)
        {
            await _repository.UpdateAsync(classes);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task AssignTrainerToClassAsync(int classId, int trainerId)
        {
            await _repository.AssignTrainerToClassAsync(classId, trainerId);
        }
    }
}