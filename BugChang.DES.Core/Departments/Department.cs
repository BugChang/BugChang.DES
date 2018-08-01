using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BugChang.DES.Core.Authorization.Users;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Exchanges.Channel;
using Newtonsoft.Json;

namespace BugChang.DES.Core.Departments
{

    [JsonObject(MemberSerialization.OptOut)]
    public class Department : BaseEntity, ISoftDelete
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(150)]
        public string FullName { get; set; }

        /// <summary>
        /// 单位代码
        /// </summary>
        [Required]
        [StringLength(3)]
        public string Code { get; set; }

        /// <summary>
        /// 默认收件渠道
        /// </summary>
        public EnumChannel ReceiveChannel { get; set; }

        public int? ParentId { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        public bool IsDeleted { get; set; }

        [JsonIgnore]
        public virtual IList<User> Users { get; set; }

        [JsonIgnore]
        public virtual IList<Department> Children { get; set; }

        [JsonIgnore]
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
        public string SetFullName(Department parentDepartment)
        {
            if (!string.IsNullOrWhiteSpace(FullName))
            {
                return parentDepartment.FullName + Name;
            }
            return Name;
        }
    }
}
