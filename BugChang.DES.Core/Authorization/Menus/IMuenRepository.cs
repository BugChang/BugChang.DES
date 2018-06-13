using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Authorization.Menus
{
    public interface IMenuRepository : IBaseRepository<Menu>
    {
        Task<IList<Menu>> GetMenusByRolesAsync(IList<string> roles);
        Task<IList<Menu>> GetAllAsync(int? parentId);

        Task<IList<Menu>> GetAllRootAsync();

        Task<IList<Menu>> GetAllByRoleIdAsync(int roleId);

        Task<PageResultModel<Menu>> GetPagingAysnc(int? parentId, int take, int skip, string keywords);

    }
}
