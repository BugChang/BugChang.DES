using System;

namespace BugChang.DES.Application.Departments.Dtos
{
    public class DepartmentViewDto
    {
        public string Name { get; set; }

        public string FullName { get; set; }

        public string Code { get; set; }

        public string ParentName { get; set; }

        public string CreateUserName { get; set; }

        public string UpdateUserName { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }    
    }
}
