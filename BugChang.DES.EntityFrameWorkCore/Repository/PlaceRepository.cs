using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Exchanges.Places;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.EntityFrameWorkCore.Repository
{
    public class PlaceRepository : BaseRepository<Place>, IPlaceRepository
    {
        private readonly DesDbContext _dbContext;
        public PlaceRepository(DesDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PageResultModel<Place>> GetPagingAysnc(PageSearchModel pageSearchModel)
        {
            var query = _dbContext.Places.Include(a => a.Department).Include(a => a.Parent).Include(a => a.CreateUser).Include(a => a.UpdateUser).Where(a => true);
            if (!string.IsNullOrWhiteSpace(pageSearchModel.Keywords))
            {
                query = query.Where(a => a.Name.Contains(pageSearchModel.Keywords));
            }
            var pageResultEntity = new PageResultModel<Place>
            {
                Total = await query.CountAsync(),
                Rows = await query.Take(pageSearchModel.Take).Skip(pageSearchModel.Skip).ToListAsync()
            };
            return pageResultEntity;
        }
    }

    public class PlaceWardenRepository : BaseRepository<PlaceWarden>, IPlaceWardenRepository
    {
        private readonly DesDbContext _dbContext;
        public PlaceWardenRepository(DesDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
