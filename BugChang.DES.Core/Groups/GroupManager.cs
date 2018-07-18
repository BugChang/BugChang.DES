using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.Core.Groups
{
    public class GroupManager
    {
        private readonly IGroupRepository _groupRepository;

        public GroupManager(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
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
    }
}
