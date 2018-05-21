using System;
using System.Collections.Generic;
using System.Text;
using BugChang.DES.Core.Authorization.Powers;

namespace BugChang.DES.Core.Authorization.Roles
{
    public class RolePower : BaseEntity
    {
        public Role Role { get; set; }

        public Power Power { get; set; }
    }
}
