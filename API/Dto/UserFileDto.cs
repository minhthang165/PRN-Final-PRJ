using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN_Final_Project.API.Dto
{
    public class UserFileDto
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Path { get; set; }
    }
}