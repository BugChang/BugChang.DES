using System;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Departments;
using BugChang.DES.Core.SecretLevels;
using BugChang.DES.Core.UrgentLevels;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.Core.Exchanges.Barcodes
{
    public class BarcodeManager
    {

        private readonly IBarcodeRepository _barcodeRepository;
        private readonly IDepartmentRepository _departmentRepository;

        public BarcodeManager(IBarcodeRepository barcodeRepository, IDepartmentRepository departmentRepository)
        {
            _barcodeRepository = barcodeRepository;
            _departmentRepository = departmentRepository;
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

        public async Task<int> GetSendDepartmentId(string barCodeNo)
        {
            var sendDepartmentId = 0;
            switch (barCodeNo.Length)
            {
                case 26:
                    {
                        var sendlv1Code = barCodeNo.Substring(1, 3);
                        var sendlv1Department = await _departmentRepository.GetQueryable().Where(a => a.ParentId == null && a.Code == sendlv1Code).FirstOrDefaultAsync();
                        if (sendlv1Department != null)
                        {
                            sendDepartmentId = sendlv1Department.Id;
                            var sendlv2Code = barCodeNo.Substring(4, 3);
                            var sendlv2Department = await _departmentRepository.GetQueryable().Where(a => a.ParentId == sendlv1Department.Id && a.Code == sendlv2Code).FirstOrDefaultAsync();
                            if (sendlv2Department != null)
                            {
                                sendDepartmentId = sendlv2Department.Id;
                            }
                        }
                    }
                    break;
                case 33:
                    {
                        var sendlv1Code = barCodeNo.Substring(0, 3);
                        var sendlv1Department = await _departmentRepository.GetQueryable().Where(a => a.ParentId == null && a.Code == sendlv1Code).FirstOrDefaultAsync();
                        if (sendlv1Department != null)
                        {
                            sendDepartmentId = sendlv1Department.Id;
                            var sendlv2Code = barCodeNo.Substring(3, 3);
                            var sendlv2Department = await _departmentRepository.GetQueryable().Where(a => a.ParentId == sendlv1Department.Id && a.Code == sendlv2Code).FirstOrDefaultAsync();
                            if (sendlv2Department != null)
                            {
                                sendDepartmentId = sendlv2Department.Id;
                                var sendlv3Code = barCodeNo.Substring(6, 3);
                                var sendlv3Department = await _departmentRepository.GetQueryable().Where(a => a.ParentId == sendlv2Department.Id && a.Code == sendlv3Code).FirstOrDefaultAsync();
                                if (sendlv3Department != null)
                                {
                                    sendDepartmentId = sendlv3Department.Id;
                                    var sendlv4Code = barCodeNo.Substring(9, 2);
                                    var sendlv4Department = await _departmentRepository.GetQueryable().Where(a => a.ParentId == sendlv3Department.Id && a.Code == sendlv4Code).FirstOrDefaultAsync();
                                    if (sendlv4Department != null)
                                    {
                                        sendDepartmentId = sendlv4Department.Id;
                                    }
                                }
                            }
                        }
                    }
                    break;
            }
            return sendDepartmentId;
        }

        public async Task<int> GetReceiveDepartmentId(string barCodeNo)
        {
            var receiveDepartmentId = 0;
            switch (barCodeNo.Length)
            {
                case 26:
                    {
                        var receivelv1Code = barCodeNo.Substring(19, 3);
                        var receivelv1Department = await _departmentRepository.GetQueryable().Where(a => a.ParentId == null && a.Code == receivelv1Code).FirstOrDefaultAsync();
                        if (receivelv1Department != null)
                        {
                            receiveDepartmentId = receivelv1Department.Id;
                            var receivelv2Code = barCodeNo.Substring(22, 3);
                            var receivelv2Department = await _departmentRepository.GetQueryable().Where(a => a.ParentId == receivelv1Department.Id && a.Code == receivelv2Code).FirstOrDefaultAsync();
                            if (receivelv2Department != null)
                            {
                                receiveDepartmentId = receivelv2Department.Id;
                            }
                        }
                    }
                    break;
                case 33:
                    {
                        var receivelv1Code = barCodeNo.Substring(22, 3);
                        var receivelv1Department = await _departmentRepository.GetQueryable().Where(a => a.ParentId == null && a.Code == receivelv1Code).FirstOrDefaultAsync();
                        if (receivelv1Department != null)
                        {
                            receiveDepartmentId = receivelv1Department.Id;
                            var receivelv2Code = barCodeNo.Substring(25, 3);
                            var receivelv2Department = await _departmentRepository.GetQueryable().Where(a => a.ParentId == receivelv1Department.Id && a.Code == receivelv2Code).FirstOrDefaultAsync();
                            if (receivelv2Department != null)
                            {
                                receiveDepartmentId = receivelv2Department.Id;
                                var receivelv3Code = barCodeNo.Substring(28, 3);
                                var receivelv3Department = await _departmentRepository.GetQueryable().Where(a => a.ParentId == receivelv2Department.Id && a.Code == receivelv3Code).FirstOrDefaultAsync();
                                if (receivelv3Department != null)
                                {
                                    receiveDepartmentId = receivelv3Department.Id;
                                    var receivelv4Code = barCodeNo.Substring(31, 2);
                                    var receivelv4Department = await _departmentRepository.GetQueryable().Where(a => a.ParentId == receivelv3Department.Id && a.Code == receivelv4Code).FirstOrDefaultAsync();
                                    if (receivelv4Department != null)
                                    {
                                        receiveDepartmentId = receivelv4Department.Id;
                                    }
                                }
                            }
                        }
                    }
                    break;
            }
            return receiveDepartmentId;
        }
    }
}
