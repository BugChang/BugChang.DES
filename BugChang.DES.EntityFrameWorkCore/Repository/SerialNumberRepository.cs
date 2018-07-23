using BugChang.DES.Core.SerialNumbers;

namespace BugChang.DES.EntityFrameWorkCore.Repository
{
    public class SerialNumberRepository : BaseRepository<SerialNumber>, ISerialNumberRepository
    {
        private readonly DesDbContext _dbContext;
        public SerialNumberRepository(DesDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
