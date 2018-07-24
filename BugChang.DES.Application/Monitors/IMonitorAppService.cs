using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Application.Boxs.Dtos;
using BugChang.DES.Core.Monitor;

namespace BugChang.DES.Application.Monitors
{
    public interface IMonitorAppService
    {
        Task<CheckBarcodeModel> CheckBarcodeType(string barcodeNo, int placeId);

        Task<IList<BoxListDto>> GetAllBoxs(int placeId);

        Task<BoxListDto> GetBox(int boxId);
    }
}
