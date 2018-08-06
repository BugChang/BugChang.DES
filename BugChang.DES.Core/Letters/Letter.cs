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
            if (sendDepartmentCode.Substring(0, 3) == receiveDepartmentCode.Substring(0, 3))
            {
                return EnumLetterType.内交换;
            }

            return EnumLetterType.发信;
        }
    }
}
