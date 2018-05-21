using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Core.Authorization.Powers;

namespace BugChang.DES.Core.Authorization.Menus
{
    public interface IMenuRepository : IBaseRepository<Menu>
    {
        Task<IList<Menu>> GetAllAsync(IList<Power> powers);
    }
}
