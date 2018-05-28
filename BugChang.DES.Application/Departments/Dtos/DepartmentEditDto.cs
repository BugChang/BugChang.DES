using System.ComponentModel.DataAnnotations;

namespace BugChang.DES.Application.Departments.Dtos
{
    public class DepartmentEditDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "机构名称不允许为空")]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(150, ErrorMessage = "机构全称长度不能高于150")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "机构代码不能为空")]
        [StringLength(3, ErrorMessage = "机构代码长度必须是3位")]
        public string Code { get; set; }

        public int? ParentId { get; set; }
    }
}
