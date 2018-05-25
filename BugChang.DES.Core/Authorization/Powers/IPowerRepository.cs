using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Core.Authorization.Roles;
using BugChang.DES.Core.Common;

namespace BugChang.DES.Core.Authorization.Powers
{
    public interface IPowerRepository : IBaseRepository<Power>
    {
        Task<IList<Power>> GetAllAsync(IList<Role> roles);
    }
}
