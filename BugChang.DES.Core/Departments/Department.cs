using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BugChang.DES.Core.Authorization.Users;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Departments
{
    public class Department : BaseEntity, ISoftDelete
    {
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

        public virtual IList<User> Users { get; set; }

        public virtual IList<Department> Children { get; set; }

        [ForeignKey("ParentId")]
        public virtual Department Parent { get; set; }

        /// <summary>
        /// 检查Code是否合法
        /// </summary>
        /// <returns></returns>
        public bool CheckCode()
        {
            return int.TryParse(Code, out _);
        }

        /// <summary>
        /// 设置全称
        /// </summary>
        public void SetFullName(Department parentDepartment)
        {
            if (string.IsNullOrWhiteSpace(FullName))
            {
                FullName = parentDepartment.FullName + Name;
            }
        }
    }
}
