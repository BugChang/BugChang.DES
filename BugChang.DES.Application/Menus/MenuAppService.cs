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
        private readonly IMenuRepository _menuRepository;
        private readonly UnitOfWork _unitOfWork;
        public MenuAppService(MenuManager menuManager, UnitOfWork unitOfWork, IMenuRepository menuRepository)
        {
            _menuManager = menuManager;
            _unitOfWork = unitOfWork;
            _menuRepository = menuRepository;
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

        public async Task<MenuEditDto> GetAsync(int id)
        {
            var menu = await _menuRepository.GetAsync(id);
            return Mapper.Map<MenuEditDto>(menu);
        }

        public async Task<ResultEntity> DeleteAsync(int id)
        {
            var result = await _menuManager.DeleteAsync(id);
            if (result.Success)
            {
                if (await _unitOfWork.CommitAsync() == 0)
                {
                    result.Success = false;
                    result.Message = "执行Commit失败！";
                }
            }

            return result;
        }
    }
}
