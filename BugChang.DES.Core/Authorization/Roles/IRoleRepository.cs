using System.Collections.Generic;
using System.Threading.Tasks;

namespace BugChang.DES.Core.Authorization.Roles
{
    public interface IRoleRepository : IBaseRepository<Role>
    {
        Task<IList<Role>> GetAllAsync(int userId);
    }
}
