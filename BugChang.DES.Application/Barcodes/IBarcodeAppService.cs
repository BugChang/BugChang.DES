using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Application.Barcodes.Dtos;
using BugChang.DES.Core.SecretLevels;
using BugChang.DES.Core.UrgentLevels;

namespace BugChang.DES.Application.Barcodes
{
    public interface IBarcodeAppService
    {
        IList<BarcodeTypeListDto> GetBarcodeTypes();

        string MakeBarcodeLength33(string sendDepartmentCode, string receiveDepartmentCode, EnumSecretLevel secretLevel,
            EnumUrgentLevel urgentLevel, int serialNo);

        string MakeBarcodeLength26(string sendDepartmentCode, string receiveDepartmentCode,
           EnumSecretLevel secretLevel,
           EnumUrgentLevel urgentLevel, int serialNo);
    }
}
