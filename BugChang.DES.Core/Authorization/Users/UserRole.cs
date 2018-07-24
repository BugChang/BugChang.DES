using System.ComponentModel.DataAnnotations.Schema;
using BugChang.DES.Core.Authorization.Roles;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Authorization.Users
{
    public class UserRole : BaseEntity<int>, ISoftDelete
    {
        public int UserId { get; set; }

        public int RoleId { get; set; }

        public bool IsDeleted { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }

    }
}
