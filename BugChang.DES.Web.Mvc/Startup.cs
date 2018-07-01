using System;
using BugChang.DES.Application.Commons;
using BugChang.DES.Core.Authentication;
using BugChang.DES.EntityFrameWorkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            Configuration = builder.Build();

            //初始化AutoMapper
            DesMapper.Initialize();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //cookie认证
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "BugChang.DES.AuthenticationCookies";
                    options.AccessDeniedPath = "/Error/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);//默认20分钟过期
                });


            //依赖注入
            DependencyInjectionConfig.Initialize(services, Configuration);

            //防伪标识
            services.AddAntiforgery(options =>
            {
                options.Cookie.Name = "BugChang.DES.Antiforgery";
            });

            //mvc
            services.AddMvc(config =>
            {
                //全局不允许匿名访问
                //var policy = new AuthorizationPolicyBuilder()
                //    .RequireAuthenticatedUser()
                //   .Build();
                //config.Filters.Add(new AuthorizeFilter(policy));
            });

            services.AddOptions();
            services.Configure<LoginSettings>(Configuration.GetSection("LoginSettings"));
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
                app.UseExceptionHandler("/Error/500");
            }

            //其他错误代码指向路径
            app.UseStatusCodePagesWithReExecute("/Error/{0}");

            //静态文件
            app.UseStaticFiles();

            //身份认证
            app.UseAuthentication();

            //默认路由的MVC
            app.UseMvcWithDefaultRoute();

            //初始化数据库
            //SeedData.Initialize(app.ApplicationServices);

        }
    }
}
