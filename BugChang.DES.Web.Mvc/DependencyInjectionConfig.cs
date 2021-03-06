﻿using BugChang.DES.Application.Accounts;
using BugChang.DES.Application.BackUps;
using BugChang.DES.Application.Barcodes;
using BugChang.DES.Application.Bills;
using BugChang.DES.Application.Boxs;
using BugChang.DES.Application.Cards;
using BugChang.DES.Application.Channels;
using BugChang.DES.Application.Clients;
using BugChang.DES.Application.DashBoards;
using BugChang.DES.Application.Departments;
using BugChang.DES.Application.ExchangeObjects;
using BugChang.DES.Application.Groups;
using BugChang.DES.Application.HardWares;
using BugChang.DES.Application.Letters;
using BugChang.DES.Application.Logs;
using BugChang.DES.Application.Menus;
using BugChang.DES.Application.Monitors;
using BugChang.DES.Application.Operations;
using BugChang.DES.Application.Places;
using BugChang.DES.Application.Roles;
using BugChang.DES.Application.Rules;
using BugChang.DES.Application.SerialNumber;
using BugChang.DES.Application.Users;
using BugChang.DES.Core.Authentication;
using BugChang.DES.Core.Authentication.Card;
using BugChang.DES.Core.Authorization.Menus;
using BugChang.DES.Core.Authorization.Operations;
using BugChang.DES.Core.Authorization.Roles;
using BugChang.DES.Core.Authorization.Users;
using BugChang.DES.Core.BackUps;
using BugChang.DES.Core.Clients;
using BugChang.DES.Core.Departments;
using BugChang.DES.Core.Exchanges.Barcodes;
using BugChang.DES.Core.Exchanges.Bill;
using BugChang.DES.Core.Exchanges.Boxs;
using BugChang.DES.Core.Exchanges.ExchangeObjects;
using BugChang.DES.Core.Exchanges.Places;
using BugChang.DES.Core.Exchanges.Rules;
using BugChang.DES.Core.Groups;
using BugChang.DES.Core.HardWares;
using BugChang.DES.Core.Letters;
using BugChang.DES.Core.Logs;
using BugChang.DES.Core.Monitor;
using BugChang.DES.Core.SerialNumbers;
using BugChang.DES.Core.Sortings;
using BugChang.DES.EntityFrameWorkCore;
using BugChang.DES.EntityFrameWorkCore.Repository;
using BugChang.DES.Web.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace BugChang.DES.Web.Mvc
{
    public static class DependencyInjectionConfig
    {
        public static readonly LoggerFactory MyLoggerFactory
            = new LoggerFactory(new[] { new ConsoleLoggerProvider((_, __) => true, true) });
        public static void Initialize(IServiceCollection services, IConfigurationRoot configuration)
        {


            #region DB

            var mainConnectionString = configuration.GetConnectionString("DefaultConnectionString");


            services.AddDbContext<DesDbContext>(option => option.UseMySql(mainConnectionString));

            services.AddScoped<UnitOfWork>();

            #endregion

            services.AddScoped<MenuFilter>();
            services.AddScoped<RefererFilter>();

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
            services.AddScoped<IRuleAppService, RuleAppSercvice>();
            services.AddScoped<IBarcodeAppService, BarcodeAppService>();
            services.AddScoped<IGroupAppService, GroupAppService>();
            services.AddScoped<ILetterAppService, LetterAppService>();
            services.AddScoped<IDashBoardAppService, DashBoardAppService>();
            services.AddScoped<ICardAppService, CardAppService>();
            services.AddScoped<IHardWareAppService, HardWareAppService>();
            services.AddScoped<IClientAppService, ClientAppService>();
            services.AddScoped<IBillAppService, BillAppService>();
            services.AddScoped<IBackUpAppService, BackUpAppService>();
            services.AddScoped<ISerialNumberAppService, SerialNumberAppService>();


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
            services.AddScoped<RuleManager>();
            services.AddScoped<GroupManager>();
            services.AddScoped<CardManager>();
            services.AddScoped<LetterManager>();
            services.AddScoped<SerialNumberManager>();
            services.AddScoped<ClientManager>();

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
            services.AddScoped<IRuleRepository, RuleRepository>();
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<IGroupDetailRepository, GroupDetailRepository>();
            services.AddScoped<ICardRepository, CardRepository>();
            services.AddScoped<ILetterRepository, LetterRepository>();
            services.AddScoped<ISerialNumberRepository, SerialNumberRepository>();
            services.AddScoped<IBarcodeRepository, BarcodeRepository>();
            services.AddScoped<IHardWareRepository, HardWareRepository>();
            services.AddScoped<IBarcodeLogRepository, BarcodeLogRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IBackLetterRepository, BackLetterRepository>();
            services.AddScoped<ICancelLetterRepository, CancelLetterRepository>();
            services.AddScoped<IExchangeListRepository, ExchangeListRepository>();
            services.AddScoped<IExchangeListDetailRepository, ExchangeListDetailRepository>();
            services.AddScoped<ISortingRepository, SortingRepository>();
            services.AddScoped<ISortingListRepository, SortingListRepository>();
            services.AddScoped<ISortingListDetailRepository, SortingListDetailRepository>();
            services.AddScoped<IBackUpRepository, BackUpRepository>();

            #endregion


        }
    }
}
