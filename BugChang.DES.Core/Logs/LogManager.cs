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

        public async Task LogDebugAsync(string title, string content, string data = "")
        {
            var log = new Log
            {
                Title = title,
                Content = content,
                CreateTime = DateTime.Now,
                Data = data,
                LogLevel = EnumLogLevel.Debug
            };
            await _logRepository.AddAsync(log);
        }

        public async Task LogInfomationAsync(string title, string content, string data = "")
        {
            var log = new Log
            {
                Title = title,
                Content = content,
                CreateTime = DateTime.Now,
                Data = data,
                LogLevel = EnumLogLevel.Information,
            };
            await _logRepository.AddAsync(log);
        }

        public async Task LogWarningAsync(string title, string content, string data = "")
        {
            var log = new Log
            {
                Title = title,
                Content = content,
                CreateTime = DateTime.Now,
                Data = data,
                LogLevel = EnumLogLevel.Warnning,
            };
            await _logRepository.AddAsync(log);
        }

        public async Task LogErrorAsync(string title, string content, string data = "")
        {
            var log = new Log
            {
                Title = title,
                Content = content,
                CreateTime = DateTime.Now,
                Data = data,
                LogLevel = EnumLogLevel.Error
            };
            await _logRepository.AddAsync(log);
        }

        public async Task LogFatalAsync(string title, string content, string data = "")
        {
            var log = new Log
            {
                Title = title,
                Content = content,
                CreateTime = DateTime.Now,
                Data = data,
                LogLevel = EnumLogLevel.Fatal
            };
            await _logRepository.AddAsync(log);
        }

    }
}
