using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Exchanges.Barcodes;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.EntityFrameWorkCore.Repository
{
    public class BarcodeRuleRepository : BaseRepository<BarcodeRule>, IBarcodeRuleRepository
    {
        private readonly DesDbContext _dbContext;
        public BarcodeRuleRepository(DesDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PageResultModel<BarcodeRule>> GetPagingAysnc(PageSearchModel pageSearchModel)
        {

            var query = _dbContext.BarcodeRules.Where(a => true);
            if (!string.IsNullOrWhiteSpace(pageSearchModel.Keywords))
            {
                query = query.Where(a => a.Name == pageSearchModel.Keywords);
            }
            return new PageResultModel<BarcodeRule>
            {
                Rows = await query.Skip(pageSearchModel.Skip).Take(pageSearchModel.Take).ToListAsync(),
                Total = await query.CountAsync()
            };
        }
    }
}
