using System.Threading.Tasks;

namespace BugChang.DES.EntityFrameWorkCore
{
    /// <summary>
    /// 工作单元，负责统一提交
    /// </summary>
    public class UnitOfWork
    {
        private readonly DesDbContext _dbContext;

        public UnitOfWork(DesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <returns></returns>
        public async Task<int> CommitAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
