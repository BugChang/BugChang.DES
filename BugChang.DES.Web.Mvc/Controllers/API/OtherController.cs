using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Application.Barcodes;
using BugChang.DES.Application.Departments;
using BugChang.DES.Application.Letters;
using BugChang.DES.Application.Letters.Dtos;
using BugChang.DES.Application.SerialNumber;
using BugChang.DES.Core.Exchanges.Channel;
using BugChang.DES.Core.Letters;
using BugChang.DES.Core.SecretLevels;
using BugChang.DES.Core.SerialNumbers;
using BugChang.DES.Core.UrgentLevels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BugChang.DES.Web.Mvc.Controllers.API
{
    public class OtherController : Controller
    {
        private readonly ILogger<OtherController> _logger;
        private readonly IDepartmentAppService _departmentAppService;
        private readonly IBarcodeAppService _barcodeAppService;
        private readonly ISerialNumberAppService _serialNumberAppService;
        private readonly ILetterAppService _letterAppService;

        public OtherController(ILogger<OtherController> logger, IDepartmentAppService departmentAppService, IBarcodeAppService barcodeAppService, ISerialNumberAppService serialNumberAppService, ILetterAppService letterAppService)
        {
            _logger = logger;
            _departmentAppService = departmentAppService;
            _barcodeAppService = barcodeAppService;
            _serialNumberAppService = serialNumberAppService;
            _letterAppService = letterAppService;
        }

        public async Task<string> GetBarcode(string recvCode, string sendCode, int sec, int urg, string wenhao)
        {
            _logger.LogInformation($"OA请求信息：recvCode:{recvCode},sendCode:{sendCode},sec:{sec},urg:{urg},wenhao:{wenhao}");
            var receiveDepartment = await _departmentAppService.GetDepartmentByCode(recvCode);
            var sendDepartment = await _departmentAppService.GetDepartmentByCode(sendCode);
            string barcodeNo;
            if ((EnumSecretLevel)sec != EnumSecretLevel.绝密 && (EnumChannel)receiveDepartment.ReceiveChannel == EnumChannel.同城交换)
            {
                var serialNo = await _serialNumberAppService.GetSerialNumber(sendDepartment.Id, EnumSerialNumberType.同城交换);
                barcodeNo = _barcodeAppService.MakeBarcodeLength26(sendDepartment.FullCode, receiveDepartment.FullCode,
                      (EnumSecretLevel)sec, (EnumUrgentLevel)urg, serialNo);
            }
            else
            {
                //绝密全部走市机
                var type = receiveDepartment.ReceiveChannel == (int)EnumChannel.内部
                    ? EnumSerialNumberType.内部交换
                    : EnumSerialNumberType.机要通信;
                var serialNo = await _serialNumberAppService.GetSerialNumber(sendDepartment.Id, type);
                barcodeNo = _barcodeAppService.MakeBarcodeLength33(sendDepartment.FullCode, receiveDepartment.FullCode,
                    (EnumSecretLevel)sec, (EnumUrgentLevel)urg, serialNo);
            }

            if (!string.IsNullOrWhiteSpace(barcodeNo))
            {
                var letter = new LetterSendEditDto
                {
                    BarcodeNo = barcodeNo,
                    CreateTime = DateTime.Now,
                    CustomData = wenhao,
                    LetterType = receiveDepartment.ReceiveChannel == (int)EnumChannel.内部
                        ? EnumLetterType.内交换
                        : EnumLetterType.发信,
                    ReceiveDepartmentId = receiveDepartment.Id,
                    SecretLevel = sec,
                    UrgencyLevel = urg
                };
                var result = await _letterAppService.AddSendLetter(letter);
                if (result.Success)
                {
                    return barcodeNo;
                }
                var info = $"条码号{barcodeNo}已生成，但保存信件出现错误！";
                _logger.LogError(info);
                return info;
            }
            _logger.LogError("条码号生成失败");
            return "条码号生成失败";
        }
    }
}
