using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Application.Commons;
using BugChang.DES.Application.Menus.Dtos;

namespace BugChang.DES.Application.Menus
{
    public interface IMenuAppService : ICurdAppService<MenuEditDto, MenuListDto>
    {
        Task<IList<MenuListDto>> GetUserMenusAsync(IList<string> userRoles);
        Task<IList<MenuListDto>> GetAllAsync(int? parentId);
        Task<IList<MenuListDto>> GetAllAsync();
        Task<IList<MenuListDto>> GetAllRootAsync();
        Task<bool> HasMenu(IList<string> userRoles, string url);
        Task<string> GetMenuBreadCrumbAsync(string url);
    }
}
