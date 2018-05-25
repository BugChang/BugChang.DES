﻿using BugChang.DES.Core.Authorization.Roles;
using BugChang.DES.Core.Common;

namespace BugChang.DES.Core.Authorization.Users
{
    public class UserRole : BaseEntity
    {
        public User User { get; set; }

        public Role Role { get; set; }
    }
}
