using System;
using BugChang.DES.Application.UserApp;
using BugChang.DES.Core.Authorization.Users;
using BugChang.DES.EntityFrameWorkCore.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BugChang.DES.Web.Mvc
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(env.ContentRootPath)
             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
             .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
             .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //cookie认证
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "BugChang.DES.Cookies";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);//默认20分钟过期
                });

            //mvc
            services.AddMvc(config =>
            {
                //全局不允许匿名访问
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });


            services.AddScoped<IUserAppService, UserAppService>();
            services.AddScoped<IUserRepository, UserRepository>();


        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                //报错指向路径
                app.UseExceptionHandler("/Errors/500");

                //其他错误代码指向路径
                app.UseStatusCodePagesWithReExecute("/errors/{0}");
            }

            //静态文件
            app.UseStaticFiles();

            //身份认证
            app.UseAuthentication();

            //默认路由的MVC
            app.UseMvcWithDefaultRoute();



        }
    }
}
