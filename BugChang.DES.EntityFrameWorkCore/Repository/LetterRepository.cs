using System;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Letters;

namespace BugChang.DES.EntityFrameWorkCore.Repository
{
    public class LetterRepository : BaseRepository<Letter>, ILetterRepository
    {
        private readonly DesDbContext _dbContext;
        public LetterRepository(DesDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<PageResultModel<Letter>> GetPagingAysnc(PageSearchModel pageSearchModel)
        {
            throw new NotImplementedException();
        }
    }
}
