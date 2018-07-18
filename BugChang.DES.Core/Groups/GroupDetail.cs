using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Groups
{
    public class DepartmentGroupDetail : BaseEntity<int>, ISoftDelete
    {
        public int GroupId { get; set; }

        public int DepartmentId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
