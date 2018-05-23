﻿using System.Collections.Generic;
using BugChang.DES.Core.Authorization.Users;

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
    }
}