using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BugChang.DES.Application.Groups.Dtos;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Groups;
using BugChang.DES.Core.Logs;
using BugChang.DES.EntityFrameWorkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BugChang.DES.Application.Groups
{
    public class GroupAppService : IGroupAppService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly GroupManager _groupManager;
        private readonly UnitOfWork _unitOfWork;
        private readonly LogManager _logManager;

        public GroupAppService(IGroupRepository groupRepository, GroupManager groupManager, UnitOfWork unitOfWork, LogManager logManager)
        {
            _groupRepository = groupRepository;
            _groupManager = groupManager;
            _unitOfWork = unitOfWork;
            _logManager = logManager;
        }

        public async Task<ResultEntity> AddOrUpdateAsync(GroupEditDto editDto)
        {
            var group = Mapper.Map<Group>(editDto);
            var result = await _groupManager.AddOrUpdateAsync(group);
            if (result.Success)
            {
                await _unitOfWork.CommitAsync();
                if (editDto.Id > 0)
                {
                    await _logManager.LogInfomationAsync(EnumLogType.Audit, LogTitleConstString.GroupEdit,
                        $"【{editDto.Name}】", JsonConvert.SerializeObject(group), editDto.CreateBy);
                }
                else
                {
                    await _logManager.LogInfomationAsync(EnumLogType.Audit, LogTitleConstString.GroupAdd,
                        $"【{editDto.Name}】", JsonConvert.SerializeObject(group), editDto.CreateBy);
                }

            }
            return result;
        }

        public async Task<ResultEntity> DeleteByIdAsync(int id, int userId)
        {
            var result = await _groupManager.DeleteAsync(id);
            if (result.Success)
            {
                await _unitOfWork.CommitAsync();
                await _logManager.LogInfomationAsync(EnumLogType.Audit, LogTitleConstString.GroupDelete,
                    $"【{result.Data.Name}】已删除", JsonConvert.SerializeObject(result.Data), userId);
            }

            return result;
        }

        public async Task<GroupEditDto> GetForEditByIdAsync(int id)
        {
            var group = await _groupRepository.GetByIdAsync(id);
            return Mapper.Map<GroupEditDto>(group);
        }

        public async Task<PageResultModel<GroupListDto>> GetPagingAysnc(PageSearchModel pageSearchDto)
        {
            var groups = await _groupRepository.GetPagingAysnc(pageSearchDto);
            return Mapper.Map<PageResultModel<GroupListDto>>(groups);
        }

        public IList<GroupTypeListDto> GetGroupTypes()
        {
            var groupTypeList = new List<GroupTypeListDto>();
            foreach (var item in Enum.GetValues(typeof(EnumGroupType)))
            {
                var groupType = new GroupTypeListDto
                {
                    Id = (int)item,
                    Name = item.ToString()
                };
                groupTypeList.Add(groupType);
            }

            return groupTypeList;
        }

        public async Task<IList<GroupDetailListDto>> GetGroupDetails(int groupId)
        {
            var groupDetails = await _groupManager.GetGroupDetails(groupId);
            return Mapper.Map<IList<GroupDetailListDto>>(groupDetails);
        }

        public async Task<IList<GroupListDto>> GetAllGroups()
        {
            var groups = await _groupRepository.GetAllAsync();
            return Mapper.Map<IList<GroupListDto>>(groups);
        }

        public async Task<IList<GroupListDto>> GetReceiveLetterGroups()
        {
            var groups = await _groupManager.GetReceiveLetterGroups();
            return Mapper.Map<IList<GroupListDto>>(groups);
        }

        public async Task<IList<GroupListDto>> GetSendLetterGroups()
        {
            var groups = await _groupManager.GetSendLetterGroups();
            return Mapper.Map<IList<GroupListDto>>(groups);
        }

        public async Task<ResultEntity> AssignDetail(int groupId, List<int> lstDepartmentId, int operatorId)
        {
            var result = await _groupManager.AssignDetail(groupId, lstDepartmentId);
            if (result.Success)
            {
                await _unitOfWork.CommitAsync();
            }

            return result;
        }
    }
}
