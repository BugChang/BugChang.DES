using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BugChang.DES.Core.Logs;

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
        }
    }
}
