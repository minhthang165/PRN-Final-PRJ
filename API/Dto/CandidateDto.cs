using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN_Final_Project.API.Dto
{
    public class CandidateDto
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public decimal? gpa { get; set; }
        public string education { get; set; }
        public string skill { get; set; }
        public string path { get; set; }
        public int fileId { get; set; }
        public bool? isActive { get; set; }
    }
}