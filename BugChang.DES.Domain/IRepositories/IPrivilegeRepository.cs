using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Domain.Entities;

namespace BugChang.DES.Domain.IRepositories
{
    public interface IPrivilegeRepository : IBaseRepository<Privilege>
    {
        Task<IList<Privilege>> GetAllAsync(IList<Role> roles);
    }
}
