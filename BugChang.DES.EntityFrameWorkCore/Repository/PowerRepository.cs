using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BugChang.DES.Core.Authorization.Powers;
using BugChang.DES.Core.Authorization.Roles;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.EntityFrameWorkCore.Repository
{
    public class PowerRepository : BaseRepository<Power>, IPowerRepository
    {
        private readonly DesDbContext _dbContext;
        public PowerRepository(DesDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IList<Power>> GetAllAsync(IList<Role> roles)
        {
            var powers = await _dbContext.RolePowers.Where(a => roles.Any(b => b == a.Role)).Select(a => a.Power).ToListAsync();
            return powers;
        }
    }
}
