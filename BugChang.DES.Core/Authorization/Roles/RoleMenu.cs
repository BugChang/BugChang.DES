using System.ComponentModel.DataAnnotations.Schema;
using BugChang.DES.Core.Authorization.Menus;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Authorization.Roles
{
    public class RoleMenu : BaseEntity,ISoftDelete
    {
        public int RoleId { get; set; }

        public int MenuId { get; set; }

        public int BatchNo { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role  Role{ get; set; }

        [ForeignKey("MenuId")]
        public virtual Menu  Menu{ get; set; }

        public bool IsDeleted { get; set; }
    }
}
