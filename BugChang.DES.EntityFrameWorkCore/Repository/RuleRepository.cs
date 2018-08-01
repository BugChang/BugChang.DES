using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Exchanges.Rules;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.EntityFrameWorkCore.Repository
{
    public class RuleRepository : BaseRepository<Rule>, IRuleRepository
    {
        private readonly DesDbContext _dbContext;
        public RuleRepository(DesDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PageResultModel<Rule>> GetPagingAysnc(PageSearchCommonModel pageSearchModel)
        {

            var query = _dbContext.BarcodeRules.Include(a => a.CreateUser).Include(a => a.UpdateUser).Where(a => true);
            if (!string.IsNullOrWhiteSpace(pageSearchModel.Keywords))
            {
                query = query.Where(a => a.Name.Contains(pageSearchModel.Keywords));
            }
            return new PageResultModel<Rule>
            {
                Rows = await query.Skip(pageSearchModel.Skip).Take(pageSearchModel.Take).OrderByDescending(a => a.Id).ToListAsync(),
                Total = await query.CountAsync()
            };
        }
    }
}
