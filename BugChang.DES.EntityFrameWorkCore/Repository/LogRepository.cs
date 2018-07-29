using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Logs;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.EntityFrameWorkCore.Repository
{
    public class LogRepository : ILogRepository
    {
        private readonly DesDbContext _dbContext;

        public LogRepository(DesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Log log)
        {
            await _dbContext.Logs.AddAsync(log);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<PageResultModel<Log>> GetSystemLogs(PageSearchCommonModel pageSearchModel)
        {
            var query = _dbContext.Logs.Where(a => a.Type == EnumLogType.System);
            if (!string.IsNullOrWhiteSpace(pageSearchModel.Keywords))
            {
                query = query.Where(a =>
                    a.Title.Contains(pageSearchModel.Keywords) || a.Content.Contains(pageSearchModel.Keywords) ||
                    a.Data.Contains(pageSearchModel.Keywords));
            }
            return new PageResultModel<Log>
            {
                Total = await query.CountAsync(),
                Rows = await query.Skip(pageSearchModel.Skip).Take(pageSearchModel.Take).ToListAsync()
            };
        }

        public async Task<PageResultModel<Log>> GetAuditLogs(PageSearchCommonModel pageSearchModel)
        {
            var query = _dbContext.Logs.Include(a => a.Operator).Where(a => a.Type == EnumLogType.Audit);
            if (!string.IsNullOrWhiteSpace(pageSearchModel.Keywords))
            {
                query = query.Where(a =>
                    a.Title.Contains(pageSearchModel.Keywords) || a.Content.Contains(pageSearchModel.Keywords) ||
                    a.Data.Contains(pageSearchModel.Keywords));
            }
            return new PageResultModel<Log>
            {
                Total = await query.CountAsync(),
                Rows = await query.Skip(pageSearchModel.Skip).Take(pageSearchModel.Take).ToListAsync()
            };
        }
    }
}
