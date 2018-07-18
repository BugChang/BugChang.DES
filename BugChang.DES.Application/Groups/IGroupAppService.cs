using System.Collections.Generic;
using BugChang.DES.Application.Commons;
using BugChang.DES.Application.Groups.Dtos;

namespace BugChang.DES.Application.Groups
{
    public interface IGroupAppService : ICurdAppService<GroupEditDto, GroupListDto>
    {
        IList<GroupTypeListDto> GetGroupTypes();
    }
}
