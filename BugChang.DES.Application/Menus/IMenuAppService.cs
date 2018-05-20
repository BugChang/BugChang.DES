using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Application.Menus.Dtos;

namespace BugChang.DES.Application.Menus
{
    public interface IMenuAppService
    {
        Task<IList<MenuDto>> GetUserMenusAsync(int userId);
    }
}
