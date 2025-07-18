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
    public class FileService : IFileService
    {
        private readonly IFileRepository _repository;
        public FileService(IFileRepository repository)
        {
            _repository = repository;
        }

        public async Task<UserFile> UploadPdf(IFormFile file)
        {
            return await _repository.UploadPdf(file);
        }

        public async Task<List<UserFile>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Page<UserFile>> GetAllPagingAsync(string? searchKey = "", int page = 1, int pageSize = 10)
        {
            return await _repository.GetAllPagingAsync(searchKey, page, pageSize);
        }

        public async Task<UserFile> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task AddAsync(UserFile file)
        {
            await _repository.AddAsync(file);
        }

        public async Task UpdateAsync(UserFile file)
        {
            await _repository.UpdateAsync(file);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
        public async Task<List<UserFile>> GetByUserIdAsync(int id)
        {
            return await _repository.GetByUserIdAsync(id);
        }
    }
}