using System;
using System.Collections.Generic;

namespace BugChang.DES.Application.Departments.Dtos
{
    public class DepartmentDto : BaseDto
    {
        public string Name { get; set; }

        public string FullName { get; set; }

        public string Code { get; set; }

        public int? ParentId { get; set; }

        public bool IsDeleted { get; set; }

        public IList<DepartmentDto> Children { get; set; }

        public string ParentName { get; set; }
    }
}
