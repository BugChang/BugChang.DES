using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Application.Menus.Dtos;
using BugChang.DES.Core.Common;

namespace BugChang.DES.Application.Menus
{
    public interface IMenuAppService
    {
        Task<IList<MenuDto>> GetUserMenusAsync(int userId);

        Task<IList<MenuDto>> GetAllAsync(int? parentId);

        Task<IList<MenuDto>> GetAllAsync();
        Task<IList<MenuDto>> GetAllRootAsync();

        Task<PageResultEntity<MenuDto>> GetPagingAysnc(int? parentId, int take, int skip, string keywords);
    }
}
