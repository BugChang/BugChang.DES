using System.Threading.Tasks;
using BugChang.DES.Core.Monitor;

namespace BugChang.DES.Application.Monitor
{
    public interface IMonitorAppService
    {
        Task<CheckBarcodeModel> CheckBarcodeType(string barcodeNo, int placeId);
    }
}
