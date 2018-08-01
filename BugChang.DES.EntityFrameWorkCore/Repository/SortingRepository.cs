using BugChang.DES.Core.Sortings;

namespace BugChang.DES.EntityFrameWorkCore.Repository
{
    public class SortingRepository : BaseRepository<Sorting>, ISortingRepository
    {
        public SortingRepository(DesDbContext dbContext) : base(dbContext)
        {
        }
    }

    public class SortingListRepository : BaseRepository<SortingList>, ISortingListRepository
    {
        public SortingListRepository(DesDbContext dbContext) : base(dbContext)
        {
        }
    }

    public class SortingListDetailRepository : BaseRepository<SortingListDetail>, ISortingListDetailRepository
    {
        public SortingListDetailRepository(DesDbContext dbContext) : base(dbContext)
        {
        }
    }
}
