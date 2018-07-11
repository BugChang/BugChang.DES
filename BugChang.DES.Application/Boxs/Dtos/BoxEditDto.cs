using System.ComponentModel.DataAnnotations;
using BugChang.DES.Application.Commons;

namespace BugChang.DES.Application.Boxs.Dtos
{
    public class BoxEditDto : EditDto
    {
        /// <summary>
        /// 设备码
        /// </summary>
        [Required(ErrorMessage = "设备编码不能为空")]
        public string DeviceCode { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required(ErrorMessage = "箱格名称不能为空")]
        public string Name { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        [MaxLength(150, ErrorMessage = "常驻提示信息最多150字")]
        public string PermanentMessage { get; set; }

        /// <summary>
        /// 场所ID
        /// </summary>
        public int PlaceId { get; set; }
    }
}
