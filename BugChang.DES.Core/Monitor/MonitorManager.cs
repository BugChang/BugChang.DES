using System;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Exchanges.Barcodes;
using BugChang.DES.Core.Exchanges.Boxs;
using BugChang.DES.Core.Exchanges.Places;

namespace BugChang.DES.Core.Monitor
{
    public class MonitorManager
    {
        private readonly IPlaceRepository _placeRepository;
        private readonly IBarcodeRepository _barcodeRepository;
        private readonly IBarcodeRouteRepository _barcodeRouteRepository;
        private readonly IBoxObjectRepository _boxObjectRepository;

        public MonitorManager(IPlaceRepository placeRepository, IBarcodeRepository barcodeRepository, IBarcodeRouteRepository barcodeRouteRepository, IBoxObjectRepository boxObjectRepository)
        {
            _placeRepository = placeRepository;
            _barcodeRepository = barcodeRepository;
            _barcodeRouteRepository = barcodeRouteRepository;
            _boxObjectRepository = boxObjectRepository;
        }

        public async Task<string> CheckBarcodeType(string barcodeNo, int placeId)
        {
            var barcode = await _barcodeRepository.GetByNoAsync(barcodeNo);
            if (barcode != null)
            {
                switch (barcode.Status)
                {
                    case EnumBarcodeStatus.已就绪:
                        var nextRoute = _barcodeRouteRepository.GetQueryable().OrderBy(a => a.Order)
                            .FirstOrDefault(a => !a.Completed);
                        if (nextRoute != null)
                        {
                            var boxs = await _boxObjectRepository.GetBoxsByObjectId(nextRoute.ObjectId);
                        }
                        else
                        {
                            return "尚未分配路由";
                        }
                        break;
                    case EnumBarcodeStatus.已投递:
                        break;
                    case EnumBarcodeStatus.已签收:
                        break;
                    case EnumBarcodeStatus.已勘误:
                        break;
                    case EnumBarcodeStatus.已退回:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {

            }
            return "";
        }
    }
}
