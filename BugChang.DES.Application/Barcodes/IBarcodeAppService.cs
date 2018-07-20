using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Application.Barcodes.Dtos;

namespace BugChang.DES.Application.Barcodes
{
    public interface IBarcodeAppService
    {
        IList<BarcodeTypeListDto> GetBarcodeTypes();

        Task<string> MakeBarcodeNo(int recDepartmentId, int sendDepartmentId);
    }
}
