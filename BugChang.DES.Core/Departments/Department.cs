using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using BugChang.DES.Core.Authorization.Users;
using BugChang.DES.Core.Common;

namespace BugChang.DES.Core.Departments
{
    public class Department : BaseEntity, ISoftDelete
    {
        public string Name { get; set; }

        public string FullName { get; set; }

        public string Code { get; set; }

        public int? ParentId { get; set; }

        public bool IsDeleted { get; set; }

        public virtual IList<User> Users { get; set; }

        public virtual IList<Department> Children { get; set; }

        [ForeignKey("ParentId")]
        public virtual Department Parent { get; set; }
    }
}
