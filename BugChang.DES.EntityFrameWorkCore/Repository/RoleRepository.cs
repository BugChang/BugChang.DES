using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Authorization.Roles;
using BugChang.DES.Core.Authorization.Users;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.EntityFrameWorkCore.Repository
{
    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        private readonly MainDbContext _dbContext;
        public RoleRepository(MainDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IList<Role>> GetAllAsync(int userId)
        {
            var roles = await _dbContext.Set<UserRole>().Where(a => a.User.Id == userId).Select(a => a.Role).ToListAsync();
            return roles;
        }
    }
}
