using System;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Exchanges.Barcodes
{
    public class BarcodeManager
    {

        private readonly IBarcodeRepository _barcodeRepository;

        public BarcodeManager(IBarcodeRepository barcodeRepository)
        {
            _barcodeRepository = barcodeRepository;
        }

        /// <summary>
        /// 分配条码路由
        /// </summary>
        /// <param name="barcodeNo">条码号</param>
        /// <returns></returns>
        public Task<ResultEntity> AssignBarcodeRoute(string barcodeNo)
        {
            return new Task<ResultEntity>(() => new ResultEntity());
        }

        /// <summary>
        /// 登记条码
        /// </summary>
        /// <param name="barcodeNo"></param>
        /// <returns></returns>
        public async Task<ResultEntity> RegisterBarcode(string barcodeNo)
        {
            var result = new ResultEntity();
            var barcode = new Barcode
            {
                BarcodeNo = barcodeNo
            };
            barcode.BarcodeType = barcode.AnalysisBarcodeType(barcodeNo);
            if (barcode.BarcodeType != EnumBarcodeType.未知条码)
            {
                barcode.Entity = barcode.AnalysisBarcodeEntity(barcode.BarcodeType);
                barcode.Souce = EnumBarcodeSouce.外部;
                barcode.Status = EnumBarcodeStatus.已就绪;
                barcode.SubStatus = EnumBarcodeSubStatus.正常;
                barcode.CreateTime = DateTime.Now;

                await _barcodeRepository.AddAsync(barcode);
                result.Success = true;
            }
            else
            {
                result.Message = "未知的条码类型";
            }
            return result;
        }

        /// <summary>
        /// 生成内部条码，33位
        /// </summary>
        /// <param name="recDepartmentId">收件单位</param>
        /// <param name="sendDepartmentId">发件单位</param>
        /// <returns></returns>
        public Task<string> MakeInsideBarcode(int recDepartmentId, int sendDepartmentId)
        {
            return new Task<string>(() => "");
        }


    }
}
