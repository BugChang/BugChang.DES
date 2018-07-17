using BugChang.DES.Application.Accounts;
using BugChang.DES.Application.BarcodeRules;
using BugChang.DES.Application.Barcodes;
using BugChang.DES.Application.Boxs;
using BugChang.DES.Application.Channels;
using BugChang.DES.Application.Departments;
using BugChang.DES.Application.ExchangeObjects;
using BugChang.DES.Application.Logs;
using BugChang.DES.Application.Menus;
using BugChang.DES.Application.Monitor;
using BugChang.DES.Application.Operations;
using BugChang.DES.Application.Places;
using BugChang.DES.Application.Roles;
using BugChang.DES.Application.Users;
using BugChang.DES.Core.Authentication;
using BugChang.DES.Core.Authorization.Menus;
using BugChang.DES.Core.Authorization.Operations;
using BugChang.DES.Core.Authorization.Roles;
using BugChang.DES.Core.Authorization.Users;
using BugChang.DES.Core.Departments;
using BugChang.DES.Core.Exchanges.Barcodes;
using BugChang.DES.Core.Exchanges.Boxs;
using BugChang.DES.Core.Exchanges.ExchangeObjects;
using BugChang.DES.Core.Exchanges.Places;
using BugChang.DES.Core.Logs;
using BugChang.DES.Core.Monitor;
using BugChang.DES.EntityFrameWorkCore;
using BugChang.DES.EntityFrameWorkCore.Repository;
using BugChang.DES.Web.Mvc.Filters;
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

            var mainConnectionString = configuration.GetConnectionString("DefaultConnectionString");

            //注册业务数据库上下文
            services.AddDbContext<DesDbContext>(option => option.UseMySql(mainConnectionString));
            //注入日志数据库上下文

            services.AddScoped<UnitOfWork>();

            #endregion

            services.AddScoped<MenuFilter>();

            #region AppService

            services.AddScoped<IUserAppService, UserAppService>();
            services.AddScoped<IAccountAppService, AccountAppService>();
            services.AddScoped<IMenuAppService, MenuAppService>();
            services.AddScoped<IDepartmentAppService, DepartmentAppService>();
            services.AddScoped<IRoleAppService, RoleAppService>();
            services.AddScoped<IOperationAppService, OperationAppService>();
            services.AddScoped<ILogAppService, LogAppService>();
            services.AddScoped<IPlaceAppService, PlaceAppService>();
            services.AddScoped<IBoxAppService, BoxAppService>();
            services.AddScoped<IChannelAppService, ChannelAppService>();
            services.AddScoped<IExchangeObjectAppService, ExchangeObjectAppService>();
            services.AddScoped<IMonitorAppService, MonitorAppService>();
            services.AddScoped<IBarcodeRuleAppService, BarcodeRuleAppSercvice>();
            services.AddScoped<IBarcodeAppService, BarcodeAppService>();

            #endregion


            #region Logic Manager

            services.AddScoped<LoginManager>();
            services.AddScoped<MenuManager>();
            services.AddScoped<DepartmentManager>();
            services.AddScoped<UserManager>();
            services.AddScoped<RoleManager>();
            services.AddScoped<OperationManager>();
            services.AddScoped<LogManager>();
            services.AddScoped<PlaceManager>();
            services.AddScoped<ExchangeObjectManager>();
            services.AddScoped<BoxManager>();
            services.AddScoped<MonitorManager>();
            services.AddScoped<BarcodeManager>();
            services.AddScoped<BarcodeRuleManager>();

            #endregion


            #region Repository

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoleMenuRepository, RoleMenuRepository>();
            services.AddScoped<IRoleOperationRepository, RoleOperationRepository>();
            services.AddScoped<ILogRepository, LogRepository>();
            services.AddScoped<IPlaceRepository, PlaceRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IBoxRepository, BoxRepository>();
            services.AddScoped<IExchangeObjectRepository, ExchangeObjectRepository>();
            services.AddScoped<IBoxObjectRepository, BoxObjectRepository>();
            services.AddScoped<IPlaceWardenRepository, PlaceWardenRepository>();
            services.AddScoped<IExchangeObjectSignerRepository, ExchangeObjectSignerRepository>();
            services.AddScoped<IBarcodeRuleRepository, BarcodeRuleRepository>();

            #endregion


        }
    }
}
