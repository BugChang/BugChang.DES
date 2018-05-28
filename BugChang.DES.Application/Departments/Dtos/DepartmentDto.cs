using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BugChang.DES.Application.Departments.Dtos
{
    public class DepartmentDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(150)]
        public string FullName { get; set; }

        [Required]
        [StringLength(3)]
        public string Code { get; set; }

        public int? ParentId { get; set; }

        public bool IsDeleted { get; set; }

        public IList<DepartmentDto> Children { get; set; }

    }
}
