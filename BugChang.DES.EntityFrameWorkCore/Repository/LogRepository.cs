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

        public async Task<PageResultModel<Log>> GetSystemLogs(LogPageSerchModel pageSearchModel)
        {
            var query = _dbContext.Logs.Where(a => a.Type == EnumLogType.System);
            if (!string.IsNullOrWhiteSpace(pageSearchModel.Title))
            {
                query = query.Where(a =>
                    a.Title.Contains(pageSearchModel.Title));
            }
            if (!string.IsNullOrWhiteSpace(pageSearchModel.Content))
            {
                query = query.Where(a =>
                    a.Title.Contains(pageSearchModel.Content));
            }
            if (pageSearchModel.Level != -1)
            {
                query = query.Where(a =>
                    (int)a.Level == pageSearchModel.Level);
            }

            query = query.Where(a =>
                a.CreateTime >= pageSearchModel.BeginTime && a.CreateTime <= pageSearchModel.EndTime);
            return new PageResultModel<Log>
            {
                Total = await query.CountAsync(),
                Rows = await query.OrderByDescending(a => a.Id).Skip(pageSearchModel.Skip).Take(pageSearchModel.Take).ToListAsync()
            };
        }

        public async Task<PageResultModel<Log>> GetAuditLogs(LogPageSerchModel pageSearchModel)
        {
            var query = _dbContext.Logs.Include(a => a.Operator).Where(a => a.Type == EnumLogType.Audit);
            if (!string.IsNullOrWhiteSpace(pageSearchModel.Title))
            {
                query = query.Where(a =>
                    a.Title.Contains(pageSearchModel.Title));
            }
            if (!string.IsNullOrWhiteSpace(pageSearchModel.Content))
            {
                query = query.Where(a =>
                    a.Title.Contains(pageSearchModel.Content));
            }
            if (pageSearchModel.Level != -1)
            {
                query = query.Where(a =>
                    (int)a.Level == pageSearchModel.Level);
            }

            query = query.Where(a =>
                a.CreateTime >= pageSearchModel.BeginTime && a.CreateTime <= pageSearchModel.EndTime);
            return new PageResultModel<Log>
            {
                Total = await query.CountAsync(),
                Rows = await query.OrderByDescending(a => a.Id).Skip(pageSearchModel.Skip).Take(pageSearchModel.Take).ToListAsync()
            };
        }
    }
}
