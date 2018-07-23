using System;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.SecretLevels;
using BugChang.DES.Core.UrgentLevels;

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


        public string MakeBarcodeLength33(string sendDepartmentCode, string receiveDepartmentCode, EnumSecretLevel secretLevel, EnumUrgentLevel urgentLevel, int serialNo)
        {
            var barCode = "";
            if (sendDepartmentCode.Length != 11 || receiveDepartmentCode.Length != 11)
            {
                return barCode;
            }

            var letterNo = serialNo.ToString("0000000");
            barCode = sendDepartmentCode + ((int)secretLevel + 1) + ((int)urgentLevel + 1) + "0" +
                          DateTime.Now.Year + letterNo + receiveDepartmentCode;
            var sum = barCode.ToCharArray().Aggregate(0, (current, value) => current + value);
            var checkCode = sum % 10;
            barCode = sendDepartmentCode
                      + ((int)secretLevel + 1) + ((int)urgentLevel + 1) + checkCode + DateTime.Now.Year.ToString().Substring(3, 1) + letterNo
                      + receiveDepartmentCode;
            return barCode;
        }

        public string MakeBarcodeLength26(string sendDepartmentCode, string receiveDepartmentCode,
            EnumSecretLevel secretLevel, EnumUrgentLevel urgentLevel, int serialNo)
        {
            var barCode = "0" + sendDepartmentCode + serialNo.ToString("00000") + DateTime.Now.ToString("yyyyMMdd").Substring(3, 1)
                          + (int)secretLevel + (int)urgentLevel
                          + receiveDepartmentCode + "0";
            return barCode;
        }

    }
}
