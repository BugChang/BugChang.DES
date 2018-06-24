using System.ComponentModel.DataAnnotations;
using BugChang.DES.Application.Commons;

namespace BugChang.DES.Application.Places.Dtos
{
    public class PlaceEditDto : EditDto
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Required(ErrorMessage = "名称不允许为空")]
        [MaxLength(20, ErrorMessage = "名称长度不能超过20")]
        public string Name { get; set; }

        /// <summary>
        /// 所属机构
        /// </summary>
        [Required(ErrorMessage = "所属机构不能为空")]
        public int DepartmentId { get; set; }

        /// <summary>
        /// 上级交换场所
        /// </summary>
        public int? ParentId { get; set; }
    }
}
