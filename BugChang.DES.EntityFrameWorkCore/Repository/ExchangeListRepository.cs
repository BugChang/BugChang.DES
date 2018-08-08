using BugChang.DES.Core.Exchanges.Bill;

namespace BugChang.DES.EntityFrameWorkCore.Repository
{
   public class ExchangeListRepository:BaseRepository<ExchangeList>,IExchangeListRepository
    {
        public ExchangeListRepository(DesDbContext dbContext) : base(dbContext)
        {
        }
    }

    public class ExchangeListDetailRepository:BaseRepository<ExchangeListDetail>,IExchangeListDetailRepository
    {
        public ExchangeListDetailRepository(DesDbContext dbContext) : base(dbContext)
        {
        }
    }
}


