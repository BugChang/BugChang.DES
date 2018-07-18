using BugChang.DES.Application.Commons;

namespace BugChang.DES.Application.Groups.Dtos
{
    public class GroupEditDto : EditDto
    {
        public string Name { get; set; }

        public int Type { get; set; }

        public string Description { get; set; }
    }
}
