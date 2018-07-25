using System;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Letters;
using Microsoft.EntityFrameworkCore;

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

        public async Task<PageResultModel<Letter>> GetTodayReceiveLetters(PageSearchModel pageSearchModel)
        {
            var query = _dbContext.Letters.Include(a => a.SendDepartment).Include(a => a.ReceiveDepartment).Include(a => a.CreateUser).Where(a =>
                      a.LetterType == EnumLetterType.收信 && a.CreateTime.Date == DateTime.Now.Date);

            return new PageResultModel<Letter>
            {
                Rows =await query.Skip(pageSearchModel.Skip).Take(pageSearchModel.Take).OrderByDescending(a=>a.Id).ToListAsync(),
                Total = await query.CountAsync()
            };
        }
    }
}
