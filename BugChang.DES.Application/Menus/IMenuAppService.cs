using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Application.Commons;
using BugChang.DES.Application.Menus.Dtos;
using BugChang.DES.Core.Common;

namespace BugChang.DES.Application.Menus
{
    public interface IMenuAppService:ICurdAppService<MenuEditDto>
    {
        Task<IList<MenuDto>> GetUserMenusAsync(IList<string> userRoles);
        Task<IList<MenuDto>> GetAllAsync(int? parentId);
        Task<IList<MenuDto>> GetAllAsync();
        Task<IList<MenuDto>> GetAllRootAsync();
        Task<PageResultEntity<MenuDto>> GetPagingAysnc(int? parentId, int take, int skip, string keywords);
        Task<bool> HasMenu(IList<string> userRoles, string url);
        Task<string> GetMenuBreadCrumbAsync(string url);
    }
}
