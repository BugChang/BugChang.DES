using System.ComponentModel.DataAnnotations;
using BugChang.DES.Application.Commons;

namespace BugChang.DES.Application.Roles.Dtos
{
    public class RoleOperationEditDto : EditDto
    {
        [Required]
        public int RoleId { get; set; }

        [Required]
        public string OperationCode { get; set; }
    }
}
