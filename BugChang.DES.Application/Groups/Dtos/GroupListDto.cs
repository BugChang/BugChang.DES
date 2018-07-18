using BugChang.DES.Application.Commons;

namespace BugChang.DES.Application.Groups.Dtos
{
    public class GroupListDto : BaseDto
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public string Description { get; set; }
    }
}
