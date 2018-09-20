using System.Threading.Tasks;
using AutoMapper;
using BugChang.DES.Application.Logs.Dtos;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Logs;

namespace BugChang.DES.Application.Logs
{
    public class LogAppService : ILogAppService
    {
        private readonly ILogRepository _logRepository;

        public LogAppService(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task<PageResultModel<AuditLogListDto>> GetAuditLogs(LogPageSerchModel pageSearchModel)
        {
            var logs = await _logRepository.GetAuditLogs(pageSearchModel);
            return Mapper.Map<PageResultModel<AuditLogListDto>>(logs);
        }

        public async Task<PageResultModel<SystemLogListDto>> GetSystemLogs(LogPageSerchModel pageSearchModel)
        {
            var logs = await _logRepository.GetSystemLogs(pageSearchModel);
            return Mapper.Map<PageResultModel<SystemLogListDto>>(logs);
        }
    }
}
