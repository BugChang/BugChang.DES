using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BugChang.DES.Application.Menus.Dtos;
using BugChang.DES.Core.Authorization.Menus;
using BugChang.DES.Core.Common;
using BugChang.DES.EntityFrameWorkCore;

namespace BugChang.DES.Application.Menus
{
    public class MenuAppService : IMenuAppService
    {
        private readonly MenuManager _menuManager;
        private readonly UnitOfWork _unitOfWork;
        public MenuAppService(MenuManager menuManager, UnitOfWork unitOfWork)
        {
            _menuManager = menuManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<IList<MenuDto>> GetUserMenusAsync(IList<string> userRoles)
        {
            var menus = await _menuManager.GetUserMenusAsync(userRoles);
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

        public async Task<ResultEntity> AddOrUpdateAsync(MenuEditDto menu)
        {
            var model = Mapper.Map<Menu>(menu);
            var result = await _menuManager.AddOrUpdateAsync(model);
            if (result.Success)
            {
                await _unitOfWork.CommitAsync();
            }
            return result;
        }

        public async Task<bool> HasMenu(IList<string> userRoles, string url)
        {
            return await _menuManager.HasMenu(userRoles, url);
        }

        public Task<string> GetMenuBreadCrumbAsync(string url)
        {
          return _menuManager.GetMenuBreadCrumb(url);
        }
    }
}
