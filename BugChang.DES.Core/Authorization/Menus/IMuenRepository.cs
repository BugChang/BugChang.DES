using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Core.Common;

namespace BugChang.DES.Core.Authorization.Menus
{
    public interface IMenuRepository : IBaseRepository<Menu>
    {
        Task<IList<Menu>> GetAllByUserIdAsync(int userId);
        Task<IList<Menu>> GetAllAsync(int? parentId);

        Task<IList<Menu>> GetAllRootAsync();

        Task<PageResultEntity<Menu>> GetPagingAysnc(int? parentId, int take, int skip, string keywords);
    }
}
