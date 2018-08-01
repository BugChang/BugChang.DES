using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Exchanges.ExchangeObjects;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.EntityFrameWorkCore.Repository
{
    public class ExchangeObjectRepository : BaseRepository<ExchangeObject>, IExchangeObjectRepository
    {
        private readonly DesDbContext _dbContext;
        public ExchangeObjectRepository(DesDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PageResultModel<ExchangeObject>> GetPagingAysnc(PageSearchModel pageSearchModel)
        {

            var queryable = _dbContext.ExchangeObjects.Include(a => a.CreateUser).Include(a => a.UpdateUser).Include(a => a.Parent).Where(a => true);
            if (!string.IsNullOrWhiteSpace(pageSearchModel.Keywords))
            {
                queryable = queryable.Where(a => a.Name.Contains(pageSearchModel.Keywords) || a.ValueText.Contains(pageSearchModel.Keywords) || a.Parent.Name.Contains(pageSearchModel.Keywords));
            }
            return new PageResultModel<ExchangeObject>
            {
                Rows = await queryable.Skip(pageSearchModel.Skip).Take(pageSearchModel.Take).OrderByDescending(a => a.Id).ToListAsync(),
                Total = await queryable.CountAsync()
            };
        }
    }

    public class ExchangeObjectSignerRepository : BaseRepository<ExchangeObjectSigner>, IExchangeObjectSignerRepository
    {
        public ExchangeObjectSignerRepository(DesDbContext dbContext) : base(dbContext)
        {
        }
    }
}
