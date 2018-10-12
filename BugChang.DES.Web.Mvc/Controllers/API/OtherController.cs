using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Application.Barcodes;
using BugChang.DES.Application.Departments;
using BugChang.DES.Application.Letters;
using BugChang.DES.Application.Letters.Dtos;
using BugChang.DES.Application.SerialNumber;
using BugChang.DES.Core.Departments;
using BugChang.DES.Core.Exchanges.Channel;
using BugChang.DES.Core.Letters;
using BugChang.DES.Core.SecretLevels;
using BugChang.DES.Core.SerialNumbers;
using BugChang.DES.Core.UrgentLevels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BugChang.DES.Web.Mvc.Controllers.API
{
    [Route("Api/[controller]/[action]")]
    public class OtherController : Controller
    {
        private readonly ILogger<OtherController> _logger;
        private readonly IDepartmentAppService _departmentAppService;
        private readonly IBarcodeAppService _barcodeAppService;
        private readonly ISerialNumberAppService _serialNumberAppService;
        private readonly ILetterAppService _letterAppService;
        private readonly IDepartmentRepository _departmentRepository;

        public OtherController(ILogger<OtherController> logger, IDepartmentAppService departmentAppService, IBarcodeAppService barcodeAppService, ISerialNumberAppService serialNumberAppService, ILetterAppService letterAppService, IDepartmentRepository departmentRepository)
        {
            _logger = logger;
            _departmentAppService = departmentAppService;
            _barcodeAppService = barcodeAppService;
            _serialNumberAppService = serialNumberAppService;
            _letterAppService = letterAppService;
            _departmentRepository = departmentRepository;
        }

        public async Task<string> GetBarcode(string recvCode, string sendCode, int sec, int urg, string wenhao)
        {
            _logger.LogWarning($"OA请求信息：recvCode:{recvCode},sendCode:{sendCode},sec:{sec},urg:{urg},wenhao:{wenhao}");
            var receiveDepartment = await _departmentAppService.GetForEditByIdAsync(await GetSendDepartmentId(recvCode));
            _logger.LogWarning($"receiveDepartment：{JsonConvert.SerializeObject(receiveDepartment)}");
            var sendDepartment = await _departmentAppService.GetForEditByIdAsync(await GetSendDepartmentId(sendCode));
            _logger.LogWarning($"sendDepartment：{JsonConvert.SerializeObject(sendDepartment)}");
            string barcodeNo;
            if ((EnumSecretLevel)sec != EnumSecretLevel.绝密 && (EnumChannel)receiveDepartment.ReceiveChannel == EnumChannel.同城交换)
            {
                var serialNo = await _serialNumberAppService.GetSerialNumber(sendDepartment.Id, EnumSerialNumberType.同城交换);
                _logger.LogWarning($"serialNo：{JsonConvert.SerializeObject(serialNo)}");

                barcodeNo = _barcodeAppService.MakeBarcodeLength26(sendCode, recvCode,
                      (EnumSecretLevel)sec, (EnumUrgentLevel)urg, serialNo);
                _logger.LogWarning($"barcodeNo：{JsonConvert.SerializeObject(barcodeNo)}");
            }
            else
            {
                //绝密全部走市机
                var type = receiveDepartment.ReceiveChannel == (int)EnumChannel.内部
                    ? EnumSerialNumberType.内部交换
                    : EnumSerialNumberType.机要通信;
                _logger.LogWarning($"type：{JsonConvert.SerializeObject(type)}");
                var serialNo = await _serialNumberAppService.GetSerialNumber(sendDepartment.Id, type);
                barcodeNo = _barcodeAppService.MakeBarcodeLength33(sendCode, recvCode,
                    (EnumSecretLevel)sec, (EnumUrgentLevel)urg, serialNo);
                _logger.LogWarning($"barcodeNo：{JsonConvert.SerializeObject(barcodeNo)}");
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
                _logger.LogWarning($"letter：{JsonConvert.SerializeObject(letter)}");
                var result = await _letterAppService.AddSendLetter(letter);
                if (result.Success)
                {
                    return barcodeNo;
                }
                var info = $"条码号{barcodeNo}已生成，但保存信件出现错误！";
                _logger.LogError(info);
                return barcodeNo;
            }
            _logger.LogError("条码号生成失败");
            return "条码号生成失败";
        }


        public async Task<int> GetSendDepartmentId(string barCodeNo)
        {
            var sendDepartmentId = 0;

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
            return sendDepartmentId;
        }
        
    }
}
