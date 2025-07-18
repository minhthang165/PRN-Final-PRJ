using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Service.Common;

namespace PRN_Final_Project.Service.Interface
{
    public interface ICVInfoService : ICommonService<CV_Info>
    {
        Task<CV_Info> ApplyCv(int fileId, int recruitmentId);
        Task<List<CV_Info>> FindCvInfoByRecruitmentId(int recruitmentId);
        Task ApproveCv(int cvId);
        Task RejectCv(int cvId);
        Task<int> CountActiveCVByRecruitmentId(int recruitmentId);
        Task<bool> ExistsByFileIdAndRecruitmentId(int fileId, int recruitmentId);
    }
}