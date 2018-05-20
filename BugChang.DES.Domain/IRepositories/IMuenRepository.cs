using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Domain.Entities;

namespace BugChang.DES.Domain.IRepositories
{
    public interface IMenuRepository : IBaseRepository<Menu>
    {
        Task<IList<Menu>> GetAllAsync(IList<Privilege> privileges);
    }
}
