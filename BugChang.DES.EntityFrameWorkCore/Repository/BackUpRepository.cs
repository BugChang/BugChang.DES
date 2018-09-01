using BugChang.DES.Core.BackUps;

namespace BugChang.DES.EntityFrameWorkCore.Repository
{
    public class BackUpRepository : BaseRepository<DataBaseBackUp>, IBackUpRepository
    {
        public BackUpRepository(DesDbContext dbContext) : base(dbContext)
        {
        }
    }
}
