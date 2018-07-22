using System.ComponentModel.DataAnnotations;
using BugChang.DES.Application.Commons;

namespace BugChang.DES.Application.Cards.Dtos
{
    public class CardEditDto : EditDto
    {
        [Required(ErrorMessage = "请选择一个有效用户")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "证卡编号不能为空")]
        [MaxLength(50,ErrorMessage ="证卡编号最大长度50" )]
        public string Number { get; set; }

        [Required(ErrorMessage = "证卡卡号不能为空")]
        [MaxLength(500, ErrorMessage = "证卡卡号最大长度500")]
        public string Value { get; set; }

        public bool Enabled { get; set; }
    }
}
