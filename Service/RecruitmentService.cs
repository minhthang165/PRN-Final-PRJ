using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Repositories.Common;
using PRN_Final_Project.Repositories.Interface;
using PRN_Final_Project.Service.Interface;

namespace PRN_Final_Project.Service
{
    public class RecruitmentService : IRecruitmentService
    {
        private readonly IRecruitmentRepository _repository;

        public RecruitmentService(IRecruitmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Recruitment>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Page<Recruitment>> GetAllPagingAsync(string? searchKey = "", int page = 1, int pageSize = 10)
        {
            return await _repository.GetAllPagingAsync(searchKey, page, pageSize);
        }

        public async Task<Recruitment> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task AddAsync(Recruitment recruitment)
        {
            await _repository.AddAsync(recruitment);
        }

        public async Task UpdateAsync(Recruitment recruitment)
        {
            await _repository.UpdateAsync(recruitment);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}