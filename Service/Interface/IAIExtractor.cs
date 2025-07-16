using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PRN_Final_Project.Service.Dto;

namespace PRN_Final_Project.Service.Interface
{
    public interface IAIExtractor
    {
        Task<CVExtractedInfo> ExtractData(string base64OrText);
    }
}