using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Authorization.Menus;
using BugChang.DES.Core.Authorization.Powers;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.EntityFrameWorkCore.Repository
{
    public class MenuRepository : BaseRepository<Menu>, IMenuRepository
    {
        private readonly MainDbContext _dbContext;
        public MenuRepository(MainDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 根据用户标识获取菜单列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IList<Menu>> GetAllByUserIdAsync(int userId)
        {
            var query = from menu in _dbContext.Menus
                        join rolePower in _dbContext.RolePowers on menu.Id equals rolePower.Power.ResourceId
                        join userRole in _dbContext.UserRoles on rolePower.Role.Id equals userRole.Role.Id
                        where rolePower.Power.Type == PowerType.菜单 && userRole.User.Id == userId
                        select menu;
            return await query.ToListAsync();
        }

        public new async Task<IList<Menu>> GetAllAsync()
        {
            return await _dbContext.Menus.Include(a => a.Items).Where(a => a.Parent == null).ToListAsync();
        }
    }
}
