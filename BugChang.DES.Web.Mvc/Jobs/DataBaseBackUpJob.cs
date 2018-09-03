using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BugChang.DES.Application.BackUps;
using BugChang.DES.Application.BackUps.Dtos;
using BugChang.DES.Core.BackUps;
using Microsoft.Extensions.Options;
using Pomelo.AspNetCore.TimedJob;

namespace BugChang.DES.Web.Mvc.Jobs
{
    public class DataBaseBackUpJob : Job
    {
        private readonly IBackUpAppService _backUpAppService;
        public DataBaseBackUpJob(IOptions<BackUpSettings> backUpSettings, IBackUpAppService backUpAppService)
        {
            _backUpAppService = backUpAppService;
        }

        // Begin 起始时间；Interval执行时间间隔，单位是毫秒，建议使用以下格式，此处为3小时；
        //SkipWhileExecuting是否等待上一个执行完成，true为等待；
        [Invoke(Begin = "2018-09-01 02:00", Interval = 1000 * 3600 * 24, SkipWhileExecuting = true)]
        public async Task Run()
        {
            await _backUpAppService.BackUpNow(1, "系统", "系统定时执行备份");
        }
    }
}
