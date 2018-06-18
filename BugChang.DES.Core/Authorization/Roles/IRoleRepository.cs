using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Authorization.Roles
{
    public interface IRoleRepository : IBasePageSearchRepository<Role>
    {
        Task<IList<Role>> GetAllAsync(int userId);

        Task<Role> GetByName(string roleName);
    }

    public interface IRoleMenuRepository : IBaseRepository<RoleMenu>
    {
    }

    public interface IRoleOperationRepository : IBaseRepository<RoleOperation>
    {
    }
}
