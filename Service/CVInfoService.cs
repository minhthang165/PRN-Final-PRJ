using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PRN_Final_Project.API.Dto;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Repositories.Common;
using PRN_Final_Project.Repositories.Interface;
using PRN_Final_Project.Service.Interface;

namespace PRN_Final_Project.Service
{
    public class CVInfoService : ICVInfoService
    {
        private readonly ICVInfoRepository _repository;
        private readonly IAIExtractor _extractor;
        private readonly IFileService _file;
        private readonly IRecruitmentRepository _recruitmentRepo;

        public CVInfoService(ICVInfoRepository repository, IAIExtractor extractor, IFileService file, IRecruitmentRepository recruitmentRepo)
        {
            _repository = repository;
            _extractor = extractor;
            _file = file;
            _recruitmentRepo = recruitmentRepo;
        }

        public async Task<List<CV_Info>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Page<CV_Info>> GetAllPagingAsync(string? searchKey = "", int page = 1, int pageSize = 10)
        {
            return await _repository.GetAllPagingAsync(searchKey, page, pageSize);
        }

        public async Task<CV_Info> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task AddAsync(CV_Info cv)
        {
            await _repository.AddAsync(cv);
        }

        public async Task UpdateAsync(CV_Info cv)
        {
            await _repository.UpdateAsync(cv);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<CV_Info> ApplyCv(int fileId, int recruitmentId)
        {
            var file = await _file.GetByIdAsync(fileId);
            if (file == null || string.IsNullOrEmpty(file.path))
                throw new Exception("File not found or missing URL");
            var extractedInfo = await _extractor.ExtractData(file.path);

            if (extractedInfo.GPA == 0)
            {
                extractedInfo.GPA = null;
            }

            var educationDbString = string.Join("; ", extractedInfo.Education);

            var skillString = string.Join(", ", extractedInfo.Skill);

            Recruitment existedRecruitment = await _recruitmentRepo.GetByIdAsync(recruitmentId);
            if (existedRecruitment == null)
                throw new Exception("Recruitment is required!");

            var cvInfo = new CV_Info
            {
                recruitment_id = recruitmentId,
                file_id = file.id,
                gpa = extractedInfo.GPA ?? 0.0m,
                education = educationDbString,
                skill = skillString,
                created_at = DateTime.Now,
                is_active = true
            };

            await _repository.AddAsync(cvInfo);
            return cvInfo;
        }

        public async Task<List<CandidateDto>> FindCvInfoByRecruitmentId(int recruitmentId)
        {
            return await _repository.FindCvInfoByRecruitmentId(recruitmentId);
        }

        public async Task ApproveCv(int cvId)
        {
            await _repository.ApproveCv(cvId);
        }

        public async Task RejectCv(int cvId)
        {
            await _repository.RejectCv(cvId);
        }
        public async Task<int> CountActiveCVByRecruitmentId(int recruitmentId)
        {
            return await _repository.CountActiveCVByRecruitmentId(recruitmentId);
        }
        public async Task<bool> ExistsByFileIdAndRecruitmentId(int fileId, int recruitmentId)
        {
            return await _repository.ExistsByFileIdAndRecruitmentId(fileId, recruitmentId);
        }
    }
}