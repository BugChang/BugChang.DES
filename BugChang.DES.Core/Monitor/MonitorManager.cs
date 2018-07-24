using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Exchanges.Barcodes;
using BugChang.DES.Core.Exchanges.Boxs;
using BugChang.DES.Core.Exchanges.Places;
using BugChang.DES.Core.Exchanges.Routes;
using BugChang.DES.Core.Exchanges.Rules;
using BugChang.DES.Core.Logs;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.Core.Monitor
{
    public class MonitorManager
    {
        private readonly IPlaceRepository _placeRepository;
        private readonly IBarcodeRepository _barcodeRepository;
        private readonly IRouteRepository _routeRepository;
        private readonly IBoxObjectRepository _boxObjectRepository;
        private readonly LogManager _logManager;
        private readonly BarcodeManager _barcodeManager;
        private readonly IBoxRepository _boxRepository;

        public MonitorManager(IPlaceRepository placeRepository, IBarcodeRepository barcodeRepository, IRouteRepository routeRepository, IBoxObjectRepository boxObjectRepository, LogManager logManager, BarcodeManager barcodeManager, IBoxRepository boxRepository)
        {
            _placeRepository = placeRepository;
            _barcodeRepository = barcodeRepository;
            _routeRepository = routeRepository;
            _boxObjectRepository = boxObjectRepository;
            _logManager = logManager;
            _barcodeManager = barcodeManager;
            _boxRepository = boxRepository;
        }

        public async Task<CheckBarcodeModel> CheckBarcodeType(string barcodeNo, int placeId)
        {

            var checkBarcodeModel = new CheckBarcodeModel();
            var barcode = await _barcodeRepository.GetByNoAsync(barcodeNo);
            var place = await _placeRepository.GetByIdAsync(placeId);
            if (barcode != null)
            {
                //1数据库存在记录
                switch (barcode.Status)
                {
                    case EnumBarcodeStatus.已就绪:
                    case EnumBarcodeStatus.已签收:
                        var nextRoute = _routeRepository.GetQueryable().OrderBy(a => a.Order)
                            .FirstOrDefault(a => !a.Completed);
                        if (nextRoute != null)
                        {
                            //1.1存在下一路由节点
                            var boxs = await _boxObjectRepository.GetBoxsByObjectId(nextRoute.ObjectId);
                            if (boxs.Count > 0)
                            {
                                //1.1.1下一路由节点已分配箱格
                                var currentPlaceBoxs = boxs.Where(a => a.PlaceId == placeId).ToList();
                                if (currentPlaceBoxs.Count > 0)
                                {
                                    //1.1.1.1存在本场所下的箱格
                                    var enabledBoxs = currentPlaceBoxs.Where(a => a.Enabled).ToList();
                                    if (enabledBoxs.Count > 0)
                                    {
                                        //1.1.1.1.1存在已启用状态的箱格
                                        checkBarcodeModel.Type = EnumCheckBarcodeType.唯一指定;
                                        foreach (var box in enabledBoxs)
                                        {
                                            var record = new CheckedBarcodeRecord
                                            {
                                                No = box.Id,
                                                FileCount = 1
                                            };
                                            checkBarcodeModel.Record.Add(record);
                                        }
                                    }
                                    else
                                    {
                                        //1.1.1.1.2不存在已启用状态的箱格
                                        await _logManager.LogErrorAsync(EnumLogType.System, LogTitleConstString.BarcodeSendFail, $"条码号【{barcodeNo}】投箱失败，错误原因【不存在已启用状态的箱格】,交换场所【{place.Name}】");
                                    }
                                }
                                else
                                {
                                    //1.1.1.2不存在本场所下的箱格
                                    await _logManager.LogErrorAsync(EnumLogType.System, LogTitleConstString.BarcodeSendFail, $"条码号【{barcodeNo}】投箱失败，错误原因【不存在本场所下的箱格】,交换场所【{place.Name}】");
                                }
                            }
                            else
                            {
                                //1.1.2下一路由节点未分配箱格
                                await _logManager.LogErrorAsync(EnumLogType.System, LogTitleConstString.BarcodeSendFail, $"条码号【{barcodeNo}】投箱失败，错误原因【下一路由节点未分配箱格】,交换场所【{place.Name}】");
                            }
                        }
                        else
                        {
                            //1.2不存在下一路由节点
                            await _logManager.LogErrorAsync(EnumLogType.System, LogTitleConstString.BarcodeSendFail, $"条码号【{barcodeNo}】投箱失败，错误原因【不存在下一路由节点】,交换场所【{place.Name}】");
                        }
                        break;
                    case EnumBarcodeStatus.已投递:
                        break;
                    case EnumBarcodeStatus.已勘误:
                    case EnumBarcodeStatus.已退回:
                        await _logManager.LogErrorAsync(EnumLogType.System, LogTitleConstString.BarcodeSendFail, $"条码号【{barcodeNo}】投箱失败，错误原因【此条码流程已结束】,交换场所【{place.Name}】");
                        break;
                    default:
                        await _logManager.LogErrorAsync(EnumLogType.System, LogTitleConstString.BarcodeSendFail, $"条码号【{barcodeNo}】投箱失败，错误原因【条码状态有误】,交换场所【{place.Name}】");
                        break;
                }
            }
            else
            {
                //数据库不存在记录

                //登记条码
                var registerResult = await _barcodeManager.RegisterBarcode(barcodeNo);
                if (registerResult.Success)
                {
                    //分配路由
                    var result = await _barcodeManager.AssignBarcodeRoute(barcodeNo);
                    if (result.Success)
                    {
                        //分配路由成功
                        await CheckBarcodeType(barcodeNo, placeId);
                    }
                    else
                    {
                        //分配路由失败
                        await _logManager.LogErrorAsync(EnumLogType.System, LogTitleConstString.BarcodeSendFail, $"条码号【{barcodeNo}】投箱失败，错误原因【分配路由失败】,交换场所【{place.Name}】");
                    }
                }
                else
                {
                    //分配路由失败
                    await _logManager.LogErrorAsync(EnumLogType.System, LogTitleConstString.BarcodeSendFail, $"条码号【{barcodeNo}】投箱失败，错误原因【{registerResult.Message}】,交换场所【{place.Name}】");
                }
            }
            return checkBarcodeModel;
        }


        public async Task<IList<Box>> GetAllBoxs(int placeId)
        {
            var boxs = await _boxRepository.GetQueryable().Where(a => a.PlaceId == placeId).ToListAsync();
            return boxs;
        }

        public async Task<Box> GetBox(int boxId)
        {
            var box = await _boxRepository.GetByIdAsync(boxId);
            return box;
        }
    }
}
