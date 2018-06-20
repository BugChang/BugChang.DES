using System;
using System.Threading.Tasks;

namespace BugChang.DES.Core.Logs
{
    public class LogManager
    {
        private readonly ILogRepository _logRepository;

        public LogManager(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task LogDebugAsync(EnumLogType logType, string title, string content, string data = "", int? operatorId = null)
        {
            var log = new Log
            {
                Title = title,
                Content = content,
                CreateTime = DateTime.Now,
                Data = data,
                LogLevel = EnumLogLevel.Debug,
                OperatorId = operatorId
            };
            await _logRepository.AddAsync(log);
        }

        public async Task LogInfomationAsync(EnumLogType logType, string title, string content, string data = "", int? operatorId = null)
        {
            var log = new Log
            {
                Title = title,
                Content = content,
                CreateTime = DateTime.Now,
                Data = data,
                LogLevel = EnumLogLevel.Information,
                OperatorId = operatorId
            };
            await _logRepository.AddAsync(log);
        }

        public async Task LogWarningAsync(EnumLogType logType, string title, string content, string data = "", int? operatorId = null)
        {
            var log = new Log
            {
                Title = title,
                Content = content,
                CreateTime = DateTime.Now,
                Data = data,
                LogLevel = EnumLogLevel.Warnning,
                OperatorId = operatorId
            };
            await _logRepository.AddAsync(log);
        }

        public async Task LogErrorAsync(EnumLogType logType, string title, string content, string data = "", int? operatorId = null)
        {
            var log = new Log
            {
                Title = title,
                Content = content,
                CreateTime = DateTime.Now,
                Data = data,
                LogLevel = EnumLogLevel.Error,
                OperatorId = operatorId
            };
            await _logRepository.AddAsync(log);
        }

        public async Task LogFatalAsync(EnumLogType logType, string title, string content, string data = "", int? operatorId = null)
        {
            var log = new Log
            {
                Title = title,
                Content = content,
                CreateTime = DateTime.Now,
                Data = data,
                LogLevel = EnumLogLevel.Fatal,
                OperatorId = operatorId
            };
            await _logRepository.AddAsync(log);
        }

    }
}
