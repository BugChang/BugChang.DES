using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Authorization.Menus;
using BugChang.DES.Core.Commons;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.EntityFrameWorkCore.Repository
{
    public class MenuRepository : BaseRepository<Menu>, IMenuRepository
    {
        private readonly DesDbContext _dbContext;
        public MenuRepository(DesDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 获取对应角色的菜单
        /// </summary>
        /// <param name="roles">角色</param>
        /// <returns></returns>
        public async Task<IList<Menu>> GetMenusByRolesAsync(IList<string> roles)
        {
            var query = from roleMenu in _dbContext.RoleMenus
                        where roles.Contains(roleMenu.Role.Name)
                        select  roleMenu.Menu;
            return await query.ToListAsync();
        }

        /// <summary>
        /// 通过父级Id获取全部菜单
        /// </summary>
        /// <param name="parentId">父Id</param>
        /// <returns></returns>
        public async Task<IList<Menu>> GetAllAsync(int? parentId)
        {
            var query = from menu in _dbContext.Menus
                        where menu.ParentId == parentId
                        select menu;
            return await query.Include(a => a.Items).ToListAsync();
        }

        /// <summary>
        /// 获取所有根级菜单
        /// </summary>
        /// <returns></returns>
        public async Task<IList<Menu>> GetAllRootAsync()
        {
            return await _dbContext.Menus.Include(a => a.Items).ThenInclude(a => a.Items).Where(a => a.ParentId == null).ToListAsync();
        }

        /// <summary>
        /// 获取对应角色的菜单
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <returns></returns>
        public async Task<IList<Menu>> GetAllByRoleIdAsync(int roleId)
        {
            return await _dbContext.RoleMenus.Where(a => a.RoleId == roleId).Select(a => a.Menu).ToListAsync();
        }

        /// <summary>
        /// 分页获取菜单
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public async Task<PageResultModel<Menu>> GetPagingAysnc(int? parentId, int take, int skip, string keywords)
        {
            var query = from menu in _dbContext.Menus
                        where menu.ParentId == parentId
                        select menu;
            if (!string.IsNullOrWhiteSpace(keywords))
            {
                query = query.Where(q =>
                    q.Name.Contains(keywords) || q.Url.Contains(keywords) || q.Icon.Contains(keywords) || q.Description.Contains(keywords));
            }
            var pageResultEntity = new PageResultModel<Menu>
            {
                Total = await query.CountAsync(),
                Rows = await query.OrderByDescending(a => a.Id).Take(take).Skip(skip).ToListAsync()
            };

            return pageResultEntity;
        }
    }
}
