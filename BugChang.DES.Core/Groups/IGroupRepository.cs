using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Groups
{
    public interface IGroupRepository : IBasePageSearchRepository<Group>
    {
    }
    public interface IGroupDetailRepository : IBaseRepository<GroupDetail>
    {
    }
}
