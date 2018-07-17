using System.Collections.Generic;
using BugChang.DES.Application.Barcodes.Dtos;

namespace BugChang.DES.Application.Barcodes
{
    public interface IBarcodeAppService
    {
        IList<BarcodeTypeListDto> GetBarcodeTypes();
    }
}
