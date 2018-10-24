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
            _logger.LogInformation($"GetBarcode接口收到请求,参数:recvCode:{recvCode},sendCode:{sendCode},sec:{sec},urg:{urg},wenhao:{wenhao}");
            Department sendDepartment;
            Department receiveDepartment;

            //四级收件单位代码
            var recvlv1Code = recvCode.Substring(0, 3);
            var recvlv2Code = recvCode.Substring(3, 3);
            var recvlv3Code = recvCode.Substring(6, 3);
            var recvlv4Code = recvCode.Substring(9, 2);
            if (recvlv1Code == "000")
            {
                return "Error：一级单位代码不允许为000";
            }

            var recvlv1Department =
                await _departmentRepository.GetQueryable().FirstOrDefaultAsync(a => a.Code == recvlv1Code && a.ParentId == null);
            if (recvlv1Department != null)
            {
                receiveDepartment = recvlv1Department;
                if (recvlv2Code != "000")
                {
                    var recvlv2Department =
                        await _departmentRepository.GetQueryable().FirstOrDefaultAsync(a => a.Code == recvlv2Code && a.ParentId == recvlv1Department.Id);
                    if (recvlv2Department != null)
                    {
                        receiveDepartment = recvlv2Department;
                        if (recvlv3Code != "000")
                        {
                            var recvlv3Department =
                                await _departmentRepository.GetQueryable().FirstOrDefaultAsync(a => a.Code == recvlv3Code && a.ParentId == recvlv2Department.Id);
                            if (recvlv3Department != null)
                            {
                                receiveDepartment = recvlv3Department;
                                if (recvlv4Code != "00")
                                {
                                    var recvlv4Department =
                                        await _departmentRepository.GetQueryable().FirstOrDefaultAsync(a => a.Code == recvlv4Code && a.ParentId == recvlv3Department.Id);
                                    if (recvlv4Department != null)
                                    {
                                        receiveDepartment = recvlv4Department;
                                    }
                                    else
                                        return "Error:未查询到四级收件单位记录";
                                }
                            }
                            else
                                return "Error:未查询到三级收件单位记录";
                        }
                    }
                    else
                        return "Error:未查询到二级收件单位记录";
                }
            }
            else
                return "Error:未查询到一级收件单位记录";
            //四级发件单位代码
            var sendlv1Code = sendCode.Substring(0, 3);
            var sendlv2Code = sendCode.Substring(3, 3);
            var sendlv3Code = sendCode.Substring(6, 3);
            var sendlv4Code = sendCode.Substring(9, 2);

            if (sendlv1Code != "000")
            {
                var sendlv1Department =
                   await _departmentRepository.GetQueryable().FirstOrDefaultAsync(a => a.Code == sendlv1Code && a.ParentId == null);
                if (sendlv1Department != null)
                {
                    sendDepartment = sendlv1Department;
                    if (sendlv2Code != "000")
                    {
                        var sendlv2Department =
                            await _departmentRepository.GetQueryable().FirstOrDefaultAsync(a => a.Code == sendlv2Code && a.ParentId == sendlv1Department.Id);
                        if (sendlv2Department != null)
                        {
                            sendDepartment = sendlv2Department;
                            if (sendlv3Code != "000")
                            {
                                var sendlv3Department =
                                    await _departmentRepository.GetQueryable().FirstOrDefaultAsync(a => a.Code == sendlv3Code && a.ParentId == sendlv2Department.Id);
                                if (sendlv3Department != null)
                                {
                                    sendDepartment = sendlv3Department;
                                    if (sendlv4Code != "00")
                                    {
                                        var sendlv4Department =
                                            await _departmentRepository.GetQueryable().FirstOrDefaultAsync(a => a.Code == recvlv4Code && a.ParentId == sendlv3Department.Id);
                                        if (sendlv4Code != null)
                                        {
                                            sendDepartment = sendlv4Department;
                                        }
                                        else
                                            return "Error:未查询到四级发件单位记录";
                                    }
                                }
                                else
                                    return "Error:未查询到三级发件单位记录";
                            }
                        }
                        else
                            return "Error:未查询到二级发件单位记录";
                    }
                }
                else
                    return "Error:未查询到一级发件单位记录";
            }
            else
                return "Error:一级发件单位代码不允许为000";

            int serialNum;
            string barcode;
            if ((EnumSecretLevel)sec != EnumSecretLevel.绝密 && (EnumChannel)receiveDepartment.ReceiveChannel == EnumChannel.同城交换)
            {
                serialNum = await _serialNumberAppService.GetSerialNumber(sendDepartment.Id, EnumSerialNumberType.同城交换);
                barcode = _barcodeAppService.MakeBarcodeLength26(sendCode, recvCode,
                    (EnumSecretLevel)sec, (EnumUrgentLevel)urg, serialNum);
            }
            else
            {
                var type = receiveDepartment.ReceiveChannel == EnumChannel.内部
                    ? EnumSerialNumberType.内部交换
                    : EnumSerialNumberType.机要通信;
                serialNum = await _serialNumberAppService.GetSerialNumber(sendDepartment.Id, type);
                barcode = _barcodeAppService.MakeBarcodeLength33(sendCode, recvCode,
                    (EnumSecretLevel)sec, (EnumUrgentLevel)urg, serialNum);
            }

            if (!string.IsNullOrWhiteSpace(barcode))
            {
                var letter = new LetterSendEditDto
                {
                    BarcodeNo = barcode,
                    CreateTime = DateTime.Now,
                    CustomData = wenhao,
                    LetterType = receiveDepartment.ReceiveChannel == EnumChannel.内部
                        ? EnumLetterType.内交换
                        : EnumLetterType.发信,
                    SendDepartmentId = sendDepartment.Id,
                    ReceiveDepartmentId = receiveDepartment.Id,
                    SecretLevel = sec,
                    UrgencyLevel = urg
                };

                if (sendDepartment.ReceiveChannel != EnumChannel.内部)
                {
                    letter.LetterType = EnumLetterType.收信;
                }

                var result = await _letterAppService.AddSendLetter(letter);
                if (result.Success)
                {
                    _logger.LogInformation($"条码号{barcode}生成成功");
                    return barcode;
                }
                _logger.LogWarning($"条码号{barcode}已生成成功，但保存信件失败");
                return barcode;
            }
            _logger.LogError($"条码号{barcode}生成失败");
            return "条码号生成失败";
        }
    }
}
