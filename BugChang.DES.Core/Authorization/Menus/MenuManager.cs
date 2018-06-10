using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Authorization.Menus
{
    public class MenuManager
    {
        private readonly IMenuRepository _menuRepository;

        public MenuManager(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public async Task<IList<Menu>> GetUserMenusAsync(IList<string> userRoles)
        {
            var menus = await _menuRepository.GetMenusByRolesAsync(userRoles);
            return menus;
        }

        public async Task<IList<Menu>> GetAllAsync(int? parentId)
        {
            var menus = await _menuRepository.GetAllAsync(parentId);
            return menus;
        }

        public async Task<IList<Menu>> GetAllAsync()
        {
            var menus = await _menuRepository.GetAllAsync();
            return menus;
        }

        public async Task<IList<Menu>> GetAllRootAsync()
        {
            var menus = await _menuRepository.GetAllRootAsync();
            return menus;
        }

        public async Task<PageResultModel<Menu>> GetPagingAysnc(int? parentId, int take, int skip, string keywords)
        {
            return await _menuRepository.GetPagingAysnc(parentId, take, skip, keywords);
        }

        public async Task<ResultEntity> AddOrUpdateAsync(Menu menu)
        {
            var result = new ResultEntity();
            if (menu.Id > 0)
            {
               _menuRepository.Update(menu);
            }
            else
            {
                await _menuRepository.AddAsync(menu);
            }

            result.Success = true;
            return result;
        }

        public async Task<bool> HasMenu(IList<string> userRoles, string url)
        {
            var menus = await _menuRepository.GetMenusByRolesAsync(userRoles);
            if (menus.FirstOrDefault(a => a.Url.Equals(url)) != null)
            {
                return true;
            }
            return false;
        }

        public Task<string> GetMenuBreadCrumb(string url)
        {
            var breadCrumb = "";
            var menu = _menuRepository.GetQueryable().FirstOrDefault(a => a.Url == url);
            if (menu != null)
            {
                breadCrumb = menu.Name;
                var tempMenu = menu;
                while (tempMenu?.ParentId != null)
                {
                    tempMenu = _menuRepository.GetQueryable().FirstOrDefault(a => a.Id == menu.ParentId);
                    if (tempMenu != null)
                    {
                        breadCrumb = tempMenu.Name + "#" + breadCrumb;
                    }

                }
            }
            return Task.FromResult(breadCrumb);
        }

        public async Task<ResultEntity> DeleteAsync(int id)
        {
            var resultEntity = new ResultEntity();
            var subMenus = await _menuRepository.GetAllAsync(id);
            if (subMenus.Count > 0)
            {
                resultEntity.Message = $"请先删除本菜单下的{subMenus.Count}个子项";
                return resultEntity;
            }
            await _menuRepository.DeleteByIdAsync(id);
            resultEntity.Success = true;
            return resultEntity;
        }
    }
}
