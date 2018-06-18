using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BugChang.DES.Application.Accounts;
using BugChang.DES.Application.Departments;
using BugChang.DES.Application.Menus;
using BugChang.DES.Application.Operations;
using BugChang.DES.Application.Roles;
using BugChang.DES.Application.Users;
using BugChang.DES.Core.Authentication;
using BugChang.DES.Core.Authorization.Menus;
using BugChang.DES.Core.Authorization.Operations;
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
            services.AddScoped<IRoleAppService, RoleAppService>();
            services.AddScoped<IOperationAppService, OperationAppService>();

            #endregion


            #region Logic Manager

            services.AddScoped<LoginManager>();
            services.AddScoped<MenuManager>();
            services.AddScoped<DepartmentManager>();
            services.AddScoped<UserManager>();
            services.AddScoped<RoleManager>();
            services.AddScoped<OperationManager>();

            #endregion


            #region Repository

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoleMenuRepository, RoleMenuRepository>();
            services.AddScoped<IRoleOperationRepository, RoleOperationRepository>();

            #endregion

            ////集中注册服务
            //foreach (var item in GetClassName("AppService"))
            //{
            //    foreach (var typeArray in item.Value)
            //    {
            //        services.AddScoped(typeArray, item.Key);
            //    }
            //}

            ////集中注册服务
            //foreach (var item in GetClassName("Manager"))
            //{
            //    foreach (var typeArray in item.Value)
            //    {
            //        services.AddScoped(typeArray, item.Key);
            //    }
            //}

            ////集中注册服务
            //foreach (var item in GetClassName("Repository"))
            //{
            //    foreach (var typeArray in item.Value)
            //    {
            //        services.AddScoped(typeArray, item.Key);
            //    }
            //}
        }


        /// <summary>  
        /// 获取程序集中的实现类对应的多个接口
        /// </summary>  
        /// <param name="assemblyName">程序集</param>
        public static Dictionary<Type, Type[]> GetClassName(string assemblyName)
        {
            if (!String.IsNullOrEmpty(assemblyName))
            {
                Assembly assembly = Assembly.Load(assemblyName);
                List<Type> ts = assembly.GetTypes().ToList();

                var result = new Dictionary<Type, Type[]>();
                foreach (var item in ts.Where(s => !s.IsInterface))
                {
                    var interfaceType = item.GetInterfaces();
                    result.Add(item, interfaceType);
                }
                return result;
            }
            return new Dictionary<Type, Type[]>();
        }

    }
}
