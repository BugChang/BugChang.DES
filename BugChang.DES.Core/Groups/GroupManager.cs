using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.Core.Groups
{
    public class GroupManager
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupDetailRepository _groupDetailRepository;

        public GroupManager(IGroupRepository groupRepository, IGroupDetailRepository groupDetailRepository)
        {
            _groupRepository = groupRepository;
            _groupDetailRepository = groupDetailRepository;
        }

        public async Task<ResultEntity> AddOrUpdateAsync(Group group)
        {
            var result = new ResultEntity();
            var exist = await _groupRepository.GetQueryable().Where(a => a.Name == group.Name && a.Id != group.Id).CountAsync() > 0;
            if (exist)
            {
                result.Message = "分组名称已存在";
            }
            else
            {
                if (group.Id > 0)
                {
                    _groupRepository.Update(group);
                }
                else
                {
                    await _groupRepository.AddAsync(group);
                }

                result.Success = true;
            }

            return result;
        }

        public async Task<ResultEntity> DeleteAsync(int id)
        {
            var result = new ResultEntity();
            var group = await _groupRepository.GetByIdAsync(id);
            group.IsDeleted = true;
            result.Success = true;
            result.Data = group;
            return result;
        }

        public async Task<IList<GroupDetail>> GetGroupDetails(int groupId)
        {
            var groupDetails =
                await _groupDetailRepository.GetQueryable().Where(a => a.GroupId == groupId).ToListAsync();
            return groupDetails;
        }

        public async Task<ResultEntity> AssignDetail(int groupId, List<int> lstDepartmentIds)
        {
            var result = new ResultEntity();
            var queryable = _groupDetailRepository.GetQueryable().Where(a => a.GroupId == groupId);
            foreach (var groupDetail in queryable)
            {
                groupDetail.IsDeleted = true;
            }

            foreach (var departmentId in lstDepartmentIds)
            {
                var detail = new GroupDetail
                {
                    DepartmentId = departmentId,
                    GroupId = groupId
                };
                await _groupDetailRepository.AddAsync(detail);
                result.Success = true;
            }
            return result;
        }

        public async Task<IList<Group>> GetReceiveLetterGroups()
        {
            var groups = await _groupRepository.GetQueryable().Where(a => a.Type == EnumGroupType.全部 || a.Type == EnumGroupType.收信)
                .ToListAsync();
            return groups;
        }

        public async Task<IList<Group>> GetSendLetterGroups()
        {
            var groups = await _groupRepository.GetQueryable().Where(a => a.Type == EnumGroupType.全部 || a.Type == EnumGroupType.发信)
                .ToListAsync();
            return groups;
        }

    }
}
