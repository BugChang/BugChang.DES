using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BugChang.DES.Core.Authentication.Card;
using BugChang.DES.Core.Commons;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.EntityFrameWorkCore.Repository
{
    public class CardRepository : BaseRepository<Card>, ICardRepository
    {
        private readonly DesDbContext _dbContext;
        public CardRepository(DesDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PageResultModel<Card>> GetPagingAysnc(PageSearchModel pageSearchModel)
        {
            var query = _dbContext.Cards.Include(a => a.CreateUser).Include(a => a.UpdateUser).Include(a => a.User).Where(a => true);
            if (!string.IsNullOrWhiteSpace(pageSearchModel.Keywords))
            {
                query = query.Where(a =>
                    a.Number.Contains(pageSearchModel.Keywords) || a.Value.Contains(pageSearchModel.Keywords) ||
                    a.User.DisplayName.Contains(pageSearchModel.Keywords));
            }
            return new PageResultModel<Card>
            {
                Rows = await query.Skip(pageSearchModel.Skip).Take(pageSearchModel.Take).OrderByDescending(a => a.Id).ToListAsync(),
                Total = await query.CountAsync()
            };
        }
    }
}
