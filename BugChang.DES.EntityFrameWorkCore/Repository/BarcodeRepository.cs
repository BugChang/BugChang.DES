using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Exchanges.Barcodes;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.EntityFrameWorkCore.Repository
{
    public class BarcodeRepository : BaseRepository<Barcode>, IBarcodeRepository
    {
        private readonly DesDbContext _dbContext;
        public BarcodeRepository(DesDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<PageResultModel<Barcode>> GetPagingAysnc(PageSearchCommonModel pageSearchModel)
        {
            throw new NotImplementedException();
        }

        public async Task<Barcode> GetByNoAsync(string barcodeNo)
        {
            var barcode = await _dbContext.Barcodes.FirstOrDefaultAsync(a => a.BarcodeNo == barcodeNo);
            return barcode;
        }
    }

    public class BarcodeLogRepository : BaseRepository<BarcodeLog>, IBarcodeLogRepository
    {
        public BarcodeLogRepository(DesDbContext dbContext) : base(dbContext)
        {
        }

        public Task<PageResultModel<BarcodeLog>> GetPagingAysnc(PageSearchCommonModel pageSearchModel)
        {
            throw new NotImplementedException();
        }
    }
}
