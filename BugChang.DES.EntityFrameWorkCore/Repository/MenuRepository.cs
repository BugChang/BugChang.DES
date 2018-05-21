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

        public async Task<IList<Menu>> GetAllAsync(IList<Power> powers)
        {
            var menus = await _dbContext.Menus.Where(a => powers.Any(b => b.Type == PowerType.菜单 && b.ResourceId == a.Id)).ToListAsync();
            return menus;
        }

        public new async Task<IList<Menu>> GetAllAsync()
        {
            return await _dbContext.Menus.Include(a => a.Items).Where(a => a.Parent == null).ToListAsync();
        }
    }
}
