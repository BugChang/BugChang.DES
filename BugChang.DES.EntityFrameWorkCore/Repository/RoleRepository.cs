using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Authorization.Roles;
using BugChang.DES.Core.Authorization.Users;
using BugChang.DES.Core.Commons;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.EntityFrameWorkCore.Repository
{
    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        private readonly DesDbContext _dbContext;
        public RoleRepository(DesDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IList<Role>> GetAllAsync(int userId)
        {
            var roles = await _dbContext.Set<UserRole>().Where(a => a.User.Id == userId).Select(a => a.Role).ToListAsync();
            return roles;
        }

        public async Task<Role> GetByName(string roleName)
        {
            var role = await _dbContext.Roles.AsNoTracking().SingleOrDefaultAsync(a => a.Name.Equals(roleName));
            return role;
        }

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
}
