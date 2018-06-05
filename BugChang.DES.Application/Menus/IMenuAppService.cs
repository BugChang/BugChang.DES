using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Application.Menus.Dtos;
using BugChang.DES.Core.Common;

namespace BugChang.DES.Application.Menus
{
    public interface IMenuAppService
    {
        Task<IList<MenuDto>> GetUserMenusAsync(IList<string> userRoles);
        Task<IList<MenuDto>> GetAllAsync(int? parentId);
        Task<IList<MenuDto>> GetAllAsync();
        Task<IList<MenuDto>> GetAllRootAsync();
        Task<PageResultEntity<MenuDto>> GetPagingAysnc(int? parentId, int take, int skip, string keywords);
        Task<ResultEntity> AddOrUpdateAsync(MenuEditDto menu);
        Task<bool> HasMenu(IList<string> userRoles, string url);
    }
}
