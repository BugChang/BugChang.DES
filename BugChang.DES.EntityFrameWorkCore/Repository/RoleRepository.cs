using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Authorization.Roles;
using BugChang.DES.Core.Authorization.Users;
using BugChang.DES.Core.Commons;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.EntityFrameWorkCore.Repository
{
    /// <summary>
    /// 角色仓储
    /// </summary>
    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        private readonly DesDbContext _dbContext;
        public RoleRepository(DesDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 获取用户拥有的角色
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public async Task<IList<Role>> GetAllAsync(int userId)
        {
            var roles = await _dbContext.Set<UserRole>().Where(a => a.User.Id == userId).Select(a => a.Role).ToListAsync();
            return roles;
        }

        /// <summary>
        /// 获取角色
        /// </summary>
        /// <param name="roleName">角色名称</param>
        /// <returns></returns>
        public async Task<Role> GetByName(string roleName)
        {
            var role = await _dbContext.Roles.AsNoTracking().SingleOrDefaultAsync(a => a.Name.Equals(roleName));
            return role;
        }

        /// <summary>
        /// 关键字分页获取角色
        /// </summary>
        /// <param name="pageSearchModel"></param>
        /// <returns></returns>
        public async Task<PageResultModel<Role>> GetPagingAysnc(PageSearchModel pageSearchModel)
        {
            var query = _dbContext.Roles.Include(a => a.CreateUser).Include(a => a.UpdateUser)
              .Where(a => true);
            if (!string.IsNullOrEmpty(pageSearchModel.Keywords))
            {
                query = query.Where(a => a.Name.Contains(pageSearchModel.Keywords) || a.Description.Contains(pageSearchModel.Keywords));
            }
            var pageResult = new PageResultModel<Role>()
            {
                Total = await query.CountAsync(),
                Rows = await query.ToListAsync()
            };
            return pageResult;
        }
    }

    /// <summary>
    /// 角色和菜单关联关系仓储
    /// </summary>
    public class RoleMenuRepository : BaseRepository<RoleMenu>, IRoleMenuRepository
    {
        private readonly DesDbContext _dbContext;
        public RoleMenuRepository(DesDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
