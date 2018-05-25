using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Core.Common;

namespace BugChang.DES.Core.Authorization.Menus
{
    public interface IMenuRepository : IBaseRepository<Menu>
    {
        Task<IList<Menu>> GetAllByUserIdAsync(int userId);
    }
}
