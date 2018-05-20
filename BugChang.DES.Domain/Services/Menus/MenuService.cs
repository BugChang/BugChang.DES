using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Domain.Entities;
using BugChang.DES.Domain.IRepositories;

namespace BugChang.DES.Domain.Services.Menus
{
    public class MenuService : IMenuService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IPrivilegeRepository _privilegeRepository;
        private readonly IMenuRepository _menuRepository;

        public MenuService(IRoleRepository roleRepository, IPrivilegeRepository privilegeRepository, IMenuRepository menuRepository)
        {
            _roleRepository = roleRepository;
            _privilegeRepository = privilegeRepository;
            _menuRepository = menuRepository;
        }

        public async Task<IList<Menu>> GetUserMenusAsync(int userId)
        {
            //获取角色
            var roles = await _roleRepository.GetAllAsync(userId);

            //获取权限
            var privileges = await _privilegeRepository.GetAllAsync(roles);

            //获取菜单
            var menus = await _menuRepository.GetAllAsync(privileges);

            return menus;
        }
    }
}
