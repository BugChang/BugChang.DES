using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Core.Common;

namespace BugChang.DES.Core.Authorization.Menus
{
    public class MenuManager
    {
        private readonly IMenuRepository _menuRepository;

        public MenuManager(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public async Task<IList<Menu>> GetUserMenusAsync(int userId)
        {
            var menus = await _menuRepository.GetAllByUserIdAsync(userId);
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

        public async Task<PageResultEntity<Menu>> GetPagingAysnc(int? parentId, int take, int skip, string keywords)
        {
            return await _menuRepository.GetPagingAysnc(parentId, take, skip, keywords);
        }
    }
}
