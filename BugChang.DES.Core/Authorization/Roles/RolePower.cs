using BugChang.DES.Core.Authorization.Powers;
using BugChang.DES.Core.Common;

namespace BugChang.DES.Core.Authorization.Roles
{
    public class RolePower : BaseEntity
    {
        public Role Role { get; set; }

        public Power Power { get; set; }
    }
}
