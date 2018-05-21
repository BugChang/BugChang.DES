using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Core.Authorization.Roles;

namespace BugChang.DES.Core.Authorization.Powers
{
    public interface IPowerRepository : IBaseRepository<Power>
    {
        Task<IList<Power>> GetAllAsync(IList<Role> roles);
    }
}
