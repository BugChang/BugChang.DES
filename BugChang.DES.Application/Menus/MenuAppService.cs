using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BugChang.DES.Application.Menus.Dtos;
using BugChang.DES.Core.Authorization.Menus;
using BugChang.DES.Core.Common;

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

        public async Task<IList<MenuDto>> GetAllAsync(int? parentId)
        {
            var menus = await _menuManager.GetAllAsync(parentId);
            return Mapper.Map<IList<MenuDto>>(menus);
        }

        public async Task<IList<MenuDto>> GetAllAsync()
        {
            var menus = await _menuManager.GetAllAsync();
            return Mapper.Map<IList<MenuDto>>(menus);
        }

        public async Task<IList<MenuDto>> GetAllRootAsync()
        {
            var menus = await _menuManager.GetAllRootAsync();
            return Mapper.Map<IList<MenuDto>>(menus);
        }

        public async Task<PageResultEntity<MenuDto>> GetPagingAysnc(int? parentId, int take, int skip, string keywords)
        {
            var pageResult = await _menuManager.GetPagingAysnc(parentId, take, skip, keywords);
            return Mapper.Map<PageResultEntity<MenuDto>>(pageResult);
        }
    }
}
