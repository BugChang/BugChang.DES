using BugChang.DES.Application.Accounts;
using BugChang.DES.Application.Departments;
using BugChang.DES.Application.Menus;
using BugChang.DES.Application.Users;
using BugChang.DES.Core.Authentication;
using BugChang.DES.Core.Authorization.Menus;
using BugChang.DES.Core.Authorization.Powers;
using BugChang.DES.Core.Authorization.Roles;
using BugChang.DES.Core.Authorization.Users;
using BugChang.DES.Core.Departments;
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

            #endregion


            #region Logic Manager

            services.AddScoped<LoginManager>();
            services.AddScoped<MenuManager>();
            services.AddScoped<DepartmentManager>();
            services.AddScoped<UserManager>();

            #endregion


            #region Repository

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();

            #endregion

        }
    }
}
