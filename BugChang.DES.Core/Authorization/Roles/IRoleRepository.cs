using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Core.Common;

namespace BugChang.DES.Core.Authorization.Roles
{
    public interface IRoleRepository : IBaseRepository<Role>
    {
        Task<IList<Role>> GetAllAsync(int userId);
    }
}
