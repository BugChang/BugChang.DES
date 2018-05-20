using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Domain.Entities;

namespace BugChang.DES.Domain.IRepositories
{
    public interface IRoleRepository : IBaseRepository<Role>
    {
        Task<IList<Role>> GetAllAsync(int userId);
    }
}
