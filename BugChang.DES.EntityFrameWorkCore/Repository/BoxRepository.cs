using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Exchanges.Boxs;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.EntityFrameWorkCore.Repository
{
    public class BoxRepository : BaseRepository<Box>, IBoxRepository
    {
        private readonly DesDbContext _dbContext;
        public BoxRepository(DesDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PageResultModel<Box>> GetPagingAysnc(PageSearchModel pageSearchModel)
        {
            var query = _dbContext.Boxs.Include(a => a.Place).Include(a => a.Object).Where(a => true);
            if (!string.IsNullOrWhiteSpace(pageSearchModel.Keywords))
            {
                query = query.Where(a =>
                    a.Name.Contains(pageSearchModel.Keywords) || a.DeviceCode.Contains(pageSearchModel.Keywords));
            }
            return new PageResultModel<Box>
            {
                Total = await query.CountAsync(),
                Rows = await query.Skip(pageSearchModel.Skip).Take(pageSearchModel.Take).ToListAsync()
            };
        }
    }
}
