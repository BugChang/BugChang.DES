using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Authorization.Menus;
using BugChang.DES.Core.Authorization.Powers;
using BugChang.DES.Core.Common;
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

        public async Task<IList<Menu>> GetAllAsync(int? parentId)
        {
            var query = from menu in _dbContext.Menus
                        where menu.ParentId == parentId
                        select menu;
            return await query.Include(a => a.Items).ToListAsync();
        }

        public async Task<IList<Menu>> GetAllRootAsync()
        {
            return await _dbContext.Menus.Include(a => a.Items).Where(a => a.ParentId == null).ToListAsync();
        }

        public async Task<PageResultEntity<Menu>> GetPagingAysnc(int? parentId, int take, int skip, string keywords)
        {
            var query = from menu in _dbContext.Menus
                        where menu.ParentId == parentId
                        select menu;
            if (!string.IsNullOrWhiteSpace(keywords))
            {
                query = query.Where(q =>
                    q.Name.Contains(keywords) || q.Url.Contains(keywords) || q.Icon.Contains(keywords) || q.Description.Contains(keywords));
            }
            var pageResultEntity = new PageResultEntity<Menu>
            {
                Total = await query.CountAsync(),
                Rows = await query.Take(take).Skip(skip).ToListAsync()
            };

            return pageResultEntity;
        }
    }
}
