using System.Threading.Tasks;
using BugChang.DES.Core.Monitor;

namespace BugChang.DES.Application.Monitor
{
    public class MonitorAppService : IMonitorAppService
    {
        private readonly MonitorManager _monitorManager;

        public MonitorAppService(MonitorManager monitorManager)
        {
            _monitorManager = monitorManager;
        }

        public async Task<CheckBarcodeModel> CheckBarcodeType(string barcodeNo, int placeId)
        {
            return await _monitorManager.CheckBarcodeType(barcodeNo, placeId);
        }
    }
}
