using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Clients;
using BugChang.DES.Core.Commons;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.EntityFrameWorkCore.Repository
{
    public class ClientRepository : BaseRepository<Client>, IClientRepository
    {
        private readonly DesDbContext _dbContext;
        public ClientRepository(DesDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Client> GetClient(string deviceCode)
        {
            var client = await _dbContext.Clients.FirstOrDefaultAsync(a => a.DeviceCode == deviceCode);
            return client;
        }

        public async Task<PageResultModel<Client>> GetPagingAysnc(PageSearchCommonModel pageSearchModel)
        {
            var query = _dbContext.Clients.Include(a => a.CreateUser).Include(a => a.UpdateUser).Include(a => a.Place).Where(a => true);
            if (!string.IsNullOrWhiteSpace(pageSearchModel.Keywords))
            {
                query = query.Where(a =>
                    a.DeviceCode.Contains(pageSearchModel.Keywords) || a.Name.Contains(pageSearchModel.Keywords));
            }
            return new PageResultModel<Client>
            {
                Rows = await query.Skip(pageSearchModel.Skip).Take(pageSearchModel.Take).OrderByDescending(a => a.Id).ToListAsync(),
                Total = await query.CountAsync()
            };
        }
    }
}
