using System.ComponentModel.DataAnnotations;
using BugChang.DES.Application.Commons;

namespace BugChang.DES.Application.Boxs.Dtos
{
    public class BoxEditDto : EditDto
    {
        /// <summary>
        /// 设备码
        /// </summary>
        [Required]
        public string DeviceCode { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        [MaxLength(150, ErrorMessage = "提示信息最多150字")]
        public string Tips { get; set; }

        /// <summary>
        /// 场所ID
        /// </summary>
        [Required]
        public int PlaceId { get; set; }

        /// <summary>
        /// 流转对象ID
        /// </summary>
        [Required]
        public int ObjectId { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        public int Order { get; set; }
    }
}
