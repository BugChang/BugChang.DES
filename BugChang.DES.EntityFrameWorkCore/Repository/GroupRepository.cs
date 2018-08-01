using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Groups;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.EntityFrameWorkCore.Repository
{
    public class GroupRepository : BaseRepository<Group>, IGroupRepository
    {
        private readonly DesDbContext _dbContext;
        public GroupRepository(DesDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PageResultModel<Group>> GetPagingAysnc(PageSearchCommonModel pageSearchModel)
        {
            var query = _dbContext.Groups.Include(a => a.CreateUser).Include(a => a.UpdateUser).Where(a => true);
            if (!string.IsNullOrWhiteSpace(pageSearchModel.Keywords))
            {
                query = query.Where(a => a.Name.Contains(pageSearchModel.Keywords));
            }
            return new PageResultModel<Group>
            {
                Rows = await query.Skip(pageSearchModel.Skip).Take(pageSearchModel.Take).OrderByDescending(a => a.Id).ToListAsync(),
                Total = await query.CountAsync()
            };
        }
    }

    public class GroupDetailRepository : BaseRepository<GroupDetail>, IGroupDetailRepository
    {
        public GroupDetailRepository(DesDbContext dbContext) : base(dbContext)
        {
        }
    }
}
