using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BugChang.DES.Application.Menus.Dtos;
using BugChang.DES.Domain.Services.Menus;

namespace BugChang.DES.Application.Menus
{
    public class MenuAppService : IMenuAppService
    {
        private readonly IMenuService _menuService;

        public MenuAppService(IMenuService menuService)
        {
            _menuService = menuService;
        }

        public async Task<IList<MenuDto>> GetUserMenusAsync(int userId)
        {
            var menus = await _menuService.GetUserMenusAsync(userId);
            return Mapper.Map<IList<MenuDto>>(menus);
        }
    }
}
