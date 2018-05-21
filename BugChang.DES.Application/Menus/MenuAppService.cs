using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BugChang.DES.Application.Menus.Dtos;
using BugChang.DES.Core.Authorization.Menus;

namespace BugChang.DES.Application.Menus
{
    public class MenuAppService : IMenuAppService
    {
        private readonly MenuManager _menuManager;

        public MenuAppService(MenuManager menuManager)
        {
            _menuManager = menuManager;
        }

        public async Task<IList<MenuDto>> GetUserMenusAsync(int userId)
        {
            var menus = await _menuManager.GetUserMenusAsync(userId);
            return Mapper.Map<IList<MenuDto>>(menus);
        }
    }
}
