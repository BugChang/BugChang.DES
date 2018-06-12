using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BugChang.DES.Application.Commons;
using BugChang.DES.Application.Menus.Dtos;
using BugChang.DES.Core.Authorization.Menus;
using BugChang.DES.Core.Commons;
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

        public async Task<IList<MenuListDto>> GetUserMenusAsync(IList<string> userRoles)
        {
            var menus = await _menuManager.GetUserMenusAsync(userRoles);
            return Mapper.Map<IList<MenuListDto>>(menus);
        }

        public async Task<IList<MenuListDto>> GetAllAsync(int? parentId)
        {
            var menus = await _menuManager.GetAllAsync(parentId);
            return Mapper.Map<IList<MenuListDto>>(menus);
        }

        public async Task<IList<MenuListDto>> GetAllAsync()
        {
            var menus = await _menuRepository.GetAllAsync();
            return Mapper.Map<IList<MenuListDto>>(menus);
        }

        public async Task<IList<MenuListDto>> GetAllRootAsync()
        {
            var menus = await _menuManager.GetAllRootAsync();
            return Mapper.Map<IList<MenuListDto>>(menus);
        }

        public async Task<PageResultModel<MenuListDto>> GetPagingAysnc(PageSearchModel pageSearchDto)
        {
            var pageResult = await _menuRepository.GetPagingAysnc(pageSearchDto.ParentId, pageSearchDto.Take, pageSearchDto.Skip, pageSearchDto.Keywords);
            return Mapper.Map<PageResultModel<MenuListDto>>(pageResult);
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

        public async Task<ResultEntity> DeleteByIdAsync(int id)
        {
            var result = await _menuManager.DeleteAsync(id);
            if (result.Success)
            {
                await _unitOfWork.CommitAsync();
            }
            return result;
        }

        public async Task<MenuEditDto> GetForEditByIdAsync(int id)
        {
            var menu = await _menuRepository.GetByIdAsync(id);
            return Mapper.Map<MenuEditDto>(menu);
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
