using System.Collections.Generic;
using BugChang.DES.Application.Commons;

namespace BugChang.DES.Application.Departments.Dtos
{
    public class DepartmentListDto : BaseDto
    {
        public string Name { get; set; }

        public string FullName { get; set; }

        public string Code { get; set; }

        public string FullCode { get; set; }

        public int? ParentId { get; set; }

        public bool IsDeleted { get; set; }

        public string ReceiveChannel { get; set; }

        public IList<DepartmentListDto> Children { get; set; }

        public string ParentName { get; set; }

        public int Sort { get; set; }
    }
}
