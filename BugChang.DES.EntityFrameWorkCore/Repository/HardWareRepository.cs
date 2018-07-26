using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using BugChang.DES.Core.HardWares;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.EntityFrameWorkCore.Repository
{
    public class HardWareRepository : BaseRepository<HardWare>, IHardWareRepository
    {
        private readonly DesDbContext _dbContext;
        public HardWareRepository(DesDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IList<HardWare>> GetSettings(string deviceCode)
        {
            var hardWares = await _dbContext.HardWares.Where(a => a.DeviceCode == deviceCode).ToListAsync();
            return hardWares;
        }
    }
}
