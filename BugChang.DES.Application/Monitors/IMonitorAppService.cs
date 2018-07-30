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

        Task<CheckCardTypeModel> CheckCardType(int placeId, int boxId, string cardValue);

        Task<int> SaveLetter(int placeId, string barCode, int boxId, int fileCount, bool isJiaJi);

        Task<int> UserGetLetter(int boxId, string cardValue, int placeId);
    }
}
