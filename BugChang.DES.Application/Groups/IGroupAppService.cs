using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Application.Commons;
using BugChang.DES.Application.Groups.Dtos;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Application.Groups
{
    public interface IGroupAppService : ICurdAppService<GroupEditDto, GroupListDto>
    {
        IList<GroupTypeListDto> GetGroupTypes();

        Task<IList<GroupDetailListDto>> GetGroupDetails(int groupId);

        Task<IList<GroupListDto>> GetAllGroups();

        Task<IList<GroupListDto>> GetReceiveLetterGroups();

        Task<IList<GroupListDto>> GetSendLetterGroups();

        Task<ResultEntity> AssignDetail(int groupId,List<int> lstDepartmentId,int operatorId);
    }
}
