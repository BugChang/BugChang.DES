using System;
using System.ComponentModel.DataAnnotations.Schema;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Departments;
using BugChang.DES.Core.SecretLevels;
using BugChang.DES.Core.UrgentLevels;

namespace BugChang.DES.Core.Letters
{
    public class Letter : BaseEntity, ISoftDelete
    {
        /// <summary>
        /// 条码号
        /// </summary>
        public string BarcodeNo { get; set; }

        /// <summary>
        /// 信封号
        /// </summary>
        public string LetterNo { get; set; }

        /// <summary>
        /// 信件类型
        /// </summary>
        public EnumLetterType LetterType { get; set; }

        /// <summary>
        /// 原条码号
        /// </summary>
        public string OldBarcodeNo { get; set; }

        /// <summary>
        /// 密级
        /// </summary>
        public EnumSecretLevel SecretLevel { get; set; }

        /// <summary>
        /// 紧急程度
        /// </summary>
        public EnumUrgentLevel UrgencyLevel { get; set; }

        /// <summary>
        /// 限时时间
        /// </summary>
        public DateTime? UrgencyTime { get; set; }

        /// <summary>
        /// 收件单位
        /// </summary>
        public int ReceiveDepartmentId { get; set; }

        /// <summary>
        /// 收件人
        /// </summary>
        public string Receiver { get; set; }

        /// <summary>
        /// 发件单位
        /// </summary>
        public int SendDepartmentId { get; set; }

        /// <summary>
        /// 市机码
        /// </summary>
        public string ShiJiCode { get; set; }

        /// <summary>
        /// 附加数据
        /// </summary>
        public string CustomData { get; set; }

        /// <summary>
        /// 原发单位名称
        /// </summary>
        public string OldSendDepartmentName { get; set; }

        public bool IsDeleted { get; set; }

        [ForeignKey("SendDepartmentId")]
        public virtual Department SendDepartment { get; set; }

        [ForeignKey("ReceiveDepartmentId")]
        public virtual Department ReceiveDepartment { get; set; }

        public string GetLetterNo(string barCodeNo)
        {
            if (barCodeNo.Contains("(01)000001500011"))
            {
                //二维条码信件（AI）
                var letterNo = barCodeNo.Substring(barCodeNo.IndexOf("(637)", StringComparison.Ordinal));
                letterNo = letterNo.Replace("(637)", "");
                letterNo = letterNo.Substring(0, letterNo.IndexOf('('));
                return letterNo;
            }
            if (barCodeNo.Length == 33)
            {
                return barCodeNo.Substring(15, 7);
            }
            if (barCodeNo.Length == 26)
            {
                return barCodeNo.Substring(8, 8);
            }
            return "";
        }

        public EnumSecretLevel GetSecretLevel(string barCode)
        {
            if (barCode.Contains("(01)000001500011"))
            {
                //二维条码信件（AI）
                var sec = barCode.Substring(barCode.IndexOf("(623)", StringComparison.Ordinal));
                sec = sec.Replace("(623)", "");
                sec = sec.Substring(0, sec.IndexOf('('));
                return (EnumSecretLevel)Convert.ToInt32(sec);
            }
            if (barCode.Length == 33)
            {
                var sec = Convert.ToInt32(barCode.Substring(11, 1));
                return (EnumSecretLevel)(sec - 1);
            }

            if (barCode.Length == 26)
            {
                var sec = Convert.ToInt32(barCode.Substring(17, 1));
                return (EnumSecretLevel)sec;
            }

            return EnumSecretLevel.无;
        }

        public EnumUrgentLevel GetUrgencyLevel(string barCode)
        {
            if (barCode.Contains("(01)000001500011"))
            {
                //二维条码信件（AI）
                var urg = barCode.Substring(barCode.IndexOf("(624)", StringComparison.Ordinal));
                urg = urg.Replace("(624)", "");
                urg = urg.Substring(0, urg.IndexOf('('));
                return (EnumUrgentLevel)Convert.ToInt32(urg);
            }
            if (barCode.Length == 33)
            {
                var urg = Convert.ToInt32(barCode.Substring(12, 1));
                return (EnumUrgentLevel)(urg - 1);
            }

            if (barCode.Length == 26)
            {
                var urg = Convert.ToInt32(barCode.Substring(18, 1));
                return (EnumUrgentLevel)urg;
            }

            return EnumUrgentLevel.无;
        }

        public string GetSendCode(string barCodeNo)
        {
            if (barCodeNo.Contains("(01)000001500011"))
            {
                //二维条码信件（AI）
                var sendCode = barCodeNo.Substring(barCodeNo.IndexOf("(251)", StringComparison.Ordinal));
                sendCode = sendCode.Replace("(251)", "");
                sendCode = sendCode.Substring(0, sendCode.IndexOf('('));
                return sendCode;
            }
            if (barCodeNo.Length == 33)
            {
                return barCodeNo.Substring(0, 11);
            }
            if (barCodeNo.Length == 26)
            {
                return barCodeNo.Substring(1, 6);
            }

            return "";
        }

        public string GetReceiveCode(string barCodeNo)
        {
            if (barCodeNo.Contains("(01)000001500011"))
            {
                //二维条码信件（AI）
                var receiveCode = barCodeNo.Substring(barCodeNo.IndexOf("(628)", StringComparison.Ordinal));
                receiveCode = receiveCode.Replace("(628)", "");
                receiveCode = receiveCode.Substring(0, receiveCode.IndexOf('('));
                return receiveCode;
            }

            if (barCodeNo.Length == 33)
            {
                return barCodeNo.Substring(22, 11);
            }
            if (barCodeNo.Length == 26)
            {
                return barCodeNo.Substring(19, 6);
            }

            return "";
        }

        /// <summary>
        /// 获取发信类型
        /// </summary>
        /// <param name="sendDepartmentCode">发件单位代码</param>
        /// <param name="receiveDepartmentCode">收件单位代码</param>
        /// <returns></returns>
        public EnumLetterType GetSendLetterType(string sendDepartmentCode, string receiveDepartmentCode)
        {
            if (sendDepartmentCode.Length == 15)
            {
                //二维条码信件（AI）
                if (sendDepartmentCode.Substring(3, 3) == receiveDepartmentCode.Substring(3, 3))
                {
                    return EnumLetterType.内交换;
                }
            }
            if (sendDepartmentCode.Substring(0, 3) == receiveDepartmentCode.Substring(0, 3))
            {
                return EnumLetterType.内交换;
            }

            return EnumLetterType.发信;
        }
    }
}
