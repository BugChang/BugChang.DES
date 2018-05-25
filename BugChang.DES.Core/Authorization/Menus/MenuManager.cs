using System.Collections.Generic;
using System.Threading.Tasks;

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
    }
}
