using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BugChang.DES.Application.Boxs.Dtos;
using BugChang.DES.Core.Monitor;
using BugChang.DES.EntityFrameWorkCore;

namespace BugChang.DES.Application.Monitors
{
    public class MonitorAppService : IMonitorAppService
    {
        private readonly MonitorManager _monitorManager;
        private readonly UnitOfWork _unitOfWork;

        public MonitorAppService(MonitorManager monitorManager, UnitOfWork unitOfWork)
        {
            _monitorManager = monitorManager;
            _unitOfWork = unitOfWork;
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

        public async Task<CheckCardTypeModel> CheckCardType(int placeId, int boxId, string cardValue)
        {
            return await _monitorManager.CheckCardType(placeId, boxId, cardValue);
        }

        public async Task<int> SaveLetter(int placeId, string barCode, int boxId, int fileCount, bool isJiaJi)
        {
            var result = await _monitorManager.SaveLetter(placeId, barCode, boxId, fileCount, isJiaJi);
            if (result > 0)
            {
                await _unitOfWork.CommitAsync();
            }
            return result;
        }

        public async Task<int> UserGetLetter(int boxId, string cardValue, int placeId)
        {
            var result = await _monitorManager.UserGetLetter(boxId, cardValue, placeId);
            if (result > 0)
            {
                await _unitOfWork.CommitAsync();
            }
            return result;
        }
    }
}
