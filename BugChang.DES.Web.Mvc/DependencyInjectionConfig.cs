using BugChang.DES.Application.Accounts;
using BugChang.DES.Application.UserApp;
using BugChang.DES.Domain.IRepositories;
using BugChang.DES.Domain.Services.Accounts;
using BugChang.DES.EntityFrameWorkCore;
using BugChang.DES.EntityFrameWorkCore.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BugChang.DES.Web.Mvc
{
    public static class DependencyInjectionConfig
    {
        public static void Initialize(IServiceCollection services, IConfigurationRoot configuration)
        {


            #region DB

            var mainConnectionString = configuration.GetConnectionString("MainConnectionString");
            var logConnectionString = configuration.GetConnectionString("LogConnectionString");

            //注册业务数据库上下文
            services.AddDbContext<MainDbContext>(option => option.UseMySql(mainConnectionString));
            //注入日志数据库上下文
            services.AddDbContext<LogDbContext>(option => option.UseMySql(logConnectionString));

            #endregion


            #region AppService

            services.AddScoped<IUserAppService, UserAppService>();
            services.AddScoped<IAccountAppService, AccountAppService>();

            #endregion


            #region DomainServices

            services.AddScoped<IAccountServcice, AccountService>();

            #endregion


            #region Repository

            services.AddScoped<IUserRepository, UserRepository>();

            #endregion

        }
    }
}
