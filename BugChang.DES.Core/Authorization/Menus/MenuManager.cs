using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Authorization.Powers;
using BugChang.DES.Core.Authorization.Roles;

namespace BugChang.DES.Core.Authorization.Menus
{
    public class MenuManager
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPowerRepository _powerRepository;
        public MenuManager(IMenuRepository menuRepository, IRoleRepository roleRepository, IPowerRepository powerRepository)
        {
            _menuRepository = menuRepository;
            _roleRepository = roleRepository;
            _powerRepository = powerRepository;
        }

        public async Task<IList<Menu>> GetUserMenusAsync(int userId)
        {
            //获取用户角色
            var roles = await _roleRepository.GetAllAsync(userId);

            if (roles.SingleOrDefault(a => a.Name == Role.SysAdmin) != null)
            {
                //管理员直接返回全部菜单
                return await _menuRepository.GetAllAsync();
            }

            //获取权限列表
            var powers = await _powerRepository.GetAllAsync(roles);

            //获取菜单列表
            var muens = await _menuRepository.GetAllAsync(powers);

            return muens;
        }
    }
}
