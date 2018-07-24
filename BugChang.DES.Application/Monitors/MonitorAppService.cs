using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BugChang.DES.Application.Boxs.Dtos;
using BugChang.DES.Core.Monitor;

namespace BugChang.DES.Application.Monitors
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

        public async Task<IList<BoxListDto>> GetAllBoxs(int placeId)
        {
            var boxs = await _monitorManager.GetAllBoxs(placeId);
            return Mapper.Map<IList<BoxListDto>>(boxs);
        }

        public async Task<BoxListDto> GetBox(int boxId)
        {
            var box = await _monitorManager.GetBox(boxId);
            return Mapper.Map<BoxListDto>(box);
        }
    }
}
