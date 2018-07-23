using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Exchanges.Barcodes;

namespace BugChang.DES.EntityFrameWorkCore.Repository
{
    public class BarcodeRepository : BaseRepository<Barcode>, IBarcodeRepository
    {
        public BarcodeRepository(DesDbContext dbContext) : base(dbContext)
        {
        }

        public Task<PageResultModel<Barcode>> GetPagingAysnc(PageSearchModel pageSearchModel)
        {
            throw new NotImplementedException();
        }

        public Task<Barcode> GetByNoAsync(string barcode)
        {
            throw new NotImplementedException();
        }
    }
}
