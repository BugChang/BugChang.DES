using System;
using AutoMapper;
using BugChang.DES.Application.Boxs.Dtos;
using BugChang.DES.Application.Cards.Dtos;
using BugChang.DES.Application.Clients.Dtos;
using BugChang.DES.Application.Departments.Dtos;
using BugChang.DES.Application.ExchangeObjects.Dtos;
using BugChang.DES.Application.Groups.Dtos;
using BugChang.DES.Application.HardWares.Dtos;
using BugChang.DES.Application.Letters.Dtos;
using BugChang.DES.Application.Logs.Dtos;
using BugChang.DES.Application.Menus.Dtos;
using BugChang.DES.Application.Places.Dtos;
using BugChang.DES.Application.Roles.Dtos;
using BugChang.DES.Application.Rules.Dtos;
using BugChang.DES.Application.Users.Dtos;
using BugChang.DES.Core.Authentication.Card;
using BugChang.DES.Core.Authorization.Menus;
using BugChang.DES.Core.Authorization.Roles;
using BugChang.DES.Core.Authorization.Users;
using BugChang.DES.Core.Clients;
using BugChang.DES.Core.Departments;
using BugChang.DES.Core.Exchanges.Barcodes;
using BugChang.DES.Core.Exchanges.Boxs;
using BugChang.DES.Core.Exchanges.ExchangeObjects;
using BugChang.DES.Core.Exchanges.Places;
using BugChang.DES.Core.Exchanges.Rules;
using BugChang.DES.Core.Groups;
using BugChang.DES.Core.HardWares;
using BugChang.DES.Core.Letters;
using BugChang.DES.Core.Logs;
using BugChang.DES.Core.Tools;

namespace BugChang.DES.Application.Commons
{
    public static class DesMapper
    {
        public static void Initialize()
        {
            Mapper.Initialize(cfg =>
            {
                #region Menu

                cfg.CreateMap<Menu, MenuListDto>();
                cfg.CreateMap<MenuEditDto, Menu>();

                #endregion

                #region Department

                cfg.CreateMap<DepartmentEditDto, Department>();
                cfg.CreateMap<Department, DepartmentListDto>()
                    .ForMember(a => a.ParentName, b => b.MapFrom(c => c.Parent.Name))
                    .ForMember(a => a.CreateUserName, b => b.MapFrom(c => c.CreateUser.DisplayName))
                    .ForMember(a => a.UpdateUserName, b => b.MapFrom(c => c.UpdateUser.DisplayName))
                    .ForMember(a => a.CreateTime, b => b.MapFrom(c => c.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")))
                    .ForMember(a => a.UpdateTime, b => b.MapFrom(c => c.UpdateTime == null ? "" : c.UpdateTime.Value.ToString("yyyy-MM-dd HH:mm:ss")));

                #endregion

                #region User

                cfg.CreateMap<UserEditDto, User>();
                cfg.CreateMap<User, UserListDto>()
                    .ForMember(a => a.DepartmentName, b => b.MapFrom(c => c.Department.Name))
                    .ForMember(a => a.CreateUserName, b => b.MapFrom(c => c.CreateUser.DisplayName))
                    .ForMember(a => a.UpdateUserName, b => b.MapFrom(c => c.UpdateUser.DisplayName))
                    .ForMember(a => a.CreateTime, b => b.MapFrom(c => c.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")))
                    .ForMember(a => a.UpdateTime, b => b.MapFrom(c => c.UpdateTime == null ? "" : c.UpdateTime.Value.ToString("yyyy-MM-dd HH:mm:ss")));

                #endregion

                #region Role

                cfg.CreateMap<RoleEditDto, Role>();
                cfg.CreateMap<Role, RoleListDto>()
                    .ForMember(a => a.CreateUserName, b => b.MapFrom(c => c.CreateUser.DisplayName))
                    .ForMember(a => a.UpdateUserName, b => b.MapFrom(c => c.UpdateUser.DisplayName))
                    .ForMember(a => a.CreateTime, b => b.MapFrom(c => c.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")))
                    .ForMember(a => a.UpdateTime, b => b.MapFrom(c => c.UpdateTime == null ? "" : c.UpdateTime.Value.ToString("yyyy-MM-dd HH:mm:ss")));

                cfg.CreateMap<RoleOperationEditDto, RoleOperation>();

                #endregion

                #region Log

                cfg.CreateMap<Log, SystemLogListDto>()
                    .ForMember(a => a.CreateTime, b => b.MapFrom(c => c.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")))
                    .ForMember(a => a.Level, b => b.MapFrom(c => EnumHelper.GetEnumDescription(c.Level)));


                cfg.CreateMap<Log, AuditLogListDto>()
                    .ForMember(a => a.CreateTime, b => b.MapFrom(c => c.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")))
                    .ForMember(a => a.OperatorName, b => b.MapFrom(c => c.Operator.DisplayName))
                    .ForMember(a => a.Level, b => b.MapFrom(c => EnumHelper.GetEnumDescription(c.Level)));

                #endregion

                #region Place

                cfg.CreateMap<PlaceEditDto, Place>()
                    .ForMember(a => a.ParentId, b => b.MapFrom(c => c.ParentId == 0 ? null : c.ParentId));
                cfg.CreateMap<Place, PlaceEditDto>()
                    .ForMember(a => a.ParentId, b => b.MapFrom(c => c.ParentId ?? 0));
                cfg.CreateMap<Place, PlaceListDto>()
                    .ForMember(a => a.DepartmentName, b => b.MapFrom(c => c.Department.FullName))
                    .ForMember(a => a.ParentName, b => b.MapFrom(c => c.Parent.Name))
                    .ForMember(a => a.CreateUserName, b => b.MapFrom(c => c.CreateUser.DisplayName))
                    .ForMember(a => a.CreateUserName, b => b.MapFrom(c => c.CreateUser.DisplayName))
                    .ForMember(a => a.UpdateUserName, b => b.MapFrom(c => c.UpdateUser.DisplayName))
                    .ForMember(a => a.CreateTime, b => b.MapFrom(c => c.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")))
                    .ForMember(a => a.UpdateTime, b => b.MapFrom(c => c.UpdateTime == null ? "" : c.UpdateTime.Value.ToString("yyyy-MM-dd HH:mm:ss")));

                #endregion

                #region Box

                cfg.CreateMap<Box, BoxListDto>()
                    .ForMember(a => a.PlaceName, b => b.MapFrom(c => c.Place.Name))
                    .ForMember(a => a.CreateUserName, b => b.MapFrom(c => c.CreateUser.DisplayName))
                    .ForMember(a => a.CreateUserName, b => b.MapFrom(c => c.CreateUser.DisplayName))
                    .ForMember(a => a.UpdateUserName, b => b.MapFrom(c => c.UpdateUser.DisplayName))
                    .ForMember(a => a.CreateTime, b => b.MapFrom(c => c.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")))
                    .ForMember(a => a.UpdateTime, b => b.MapFrom(c => c.UpdateTime == null ? "" : c.UpdateTime.Value.ToString("yyyy-MM-dd HH:mm:ss")));

                cfg.CreateMap<BoxEditDto, Box>();

                #endregion

                #region ExchangeObject

                cfg.CreateMap<ExchangeObject, ExchangeObjectListDto>()
                    .ForMember(a => a.ObjectType, b => b.MapFrom(c => c.ObjectType.ToString()))
                    .ForMember(a => a.ParentName, b => b.MapFrom(c => c.Parent.Name))
                    .ForMember(a => a.CreateUserName, b => b.MapFrom(c => c.CreateUser.DisplayName))
                    .ForMember(a => a.CreateUserName, b => b.MapFrom(c => c.CreateUser.DisplayName))
                    .ForMember(a => a.UpdateUserName, b => b.MapFrom(c => c.UpdateUser.DisplayName))
                    .ForMember(a => a.CreateTime, b => b.MapFrom(c => c.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")))
                    .ForMember(a => a.UpdateTime, b => b.MapFrom(c => c.UpdateTime == null ? "" : c.UpdateTime.Value.ToString("yyyy-MM-dd HH:mm:ss")));

                cfg.CreateMap<ExchangeObjectEditDto, ExchangeObject>()
                    .ForMember(a => a.ParentId, b => b.MapFrom(c => c.ParentId == 0 ? null : c.ParentId));
                cfg.CreateMap<ExchangeObject, ExchangeObjectEditDto>()
                    .ForMember(a => a.ParentId, b => b.MapFrom(c => c.ParentId ?? 0));

                #endregion

                #region Rule

                cfg.CreateMap<Rule, RuleListDto>()
                    .ForMember(a => a.BarcodeType, b => b.MapFrom(c => c.BarcodeType.ToString()))
                    .ForMember(a => a.CreateUserName, b => b.MapFrom(c => c.CreateUser.DisplayName))
                    .ForMember(a => a.CreateUserName, b => b.MapFrom(c => c.CreateUser.DisplayName))
                    .ForMember(a => a.UpdateUserName, b => b.MapFrom(c => c.UpdateUser.DisplayName))
                    .ForMember(a => a.CreateTime, b => b.MapFrom(c => c.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")))
                    .ForMember(a => a.UpdateTime, b => b.MapFrom(c => c.UpdateTime == null ? "" : c.UpdateTime.Value.ToString("yyyy-MM-dd HH:mm:ss")));

                cfg.CreateMap<RuleEditDto, Rule>()
                    .ForMember(a => a.BarcodeType, b => b.MapFrom(c => (EnumBarcodeType)c.BarcodeType));

                cfg.CreateMap<Rule, RuleEditDto>()
                    .ForMember(a => a.BarcodeType, b => b.MapFrom(c => (int)c.BarcodeType));

                #endregion

                #region Group

                cfg.CreateMap<Group, GroupListDto>()
                    .ForMember(a => a.CreateUserName, b => b.MapFrom(c => c.CreateUser.DisplayName))
                    .ForMember(a => a.CreateUserName, b => b.MapFrom(c => c.CreateUser.DisplayName))
                    .ForMember(a => a.UpdateUserName, b => b.MapFrom(c => c.UpdateUser.DisplayName))
                    .ForMember(a => a.CreateTime, b => b.MapFrom(c => c.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")))
                    .ForMember(a => a.UpdateTime, b => b.MapFrom(c => c.UpdateTime == null ? "" : c.UpdateTime.Value.ToString("yyyy-MM-dd HH:mm:ss")));

                cfg.CreateMap<GroupEditDto, Group>();

                cfg.CreateMap<GroupDetail, GroupDetailListDto>();

                #endregion

                #region Card

                cfg.CreateMap<Card, CardListDto>()
                    .ForMember(a => a.UserName, b => b.MapFrom(c => c.User.DisplayName))
                    .ForMember(a => a.CreateUserName, b => b.MapFrom(c => c.CreateUser.DisplayName))
                    .ForMember(a => a.UpdateUserName, b => b.MapFrom(c => c.UpdateUser.DisplayName))
                    .ForMember(a => a.CreateTime, b => b.MapFrom(c => c.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")))
                    .ForMember(a => a.UpdateTime, b => b.MapFrom(c => c.UpdateTime == null ? "" : c.UpdateTime.Value.ToString("yyyy-MM-dd HH:mm:ss")));

                cfg.CreateMap<CardEditDto, Card>();

                #endregion

                #region Letter

                cfg.CreateMap<ReceiveLetterEditDto, Letter>();
                cfg.CreateMap<Letter, LetterReceiveListDto>()
                    .ForMember(a => a.SendDepartmentName, b => b.MapFrom(c => c.SendDepartment.Name))
                    .ForMember(a => a.ReceiveDepartmentName, b => b.MapFrom(c => c.ReceiveDepartment.Name))
                    .ForMember(a => a.CreateUserName, b => b.MapFrom(c => c.CreateUser.DisplayName))
                    .ForMember(a => a.CreateUserName, b => b.MapFrom(c => c.CreateUser.DisplayName))
                    .ForMember(a => a.CreateTime, b => b.MapFrom(c => c.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")))
                    .ForMember(a => a.UrgencyTime, b => b.MapFrom(c => c.UrgencyTime == null ? "-" : c.UrgencyTime.Value.ToString("yyyy-MM-dd HH:mm:ss")));

                cfg.CreateMap<Letter, LetterReceiveBarcodeDto>()
                    .ForMember(a => a.SendDepartmentName, b => b.MapFrom(c => c.SendDepartment.FullName))
                    .ForMember(a => a.ReceiveDepartmentName, b => b.MapFrom(c => c.ReceiveDepartment.Name))
                    .ForMember(a => a.SecretLevel, b => b.MapFrom(c => c.SecretLevel.ToString()))
                    .ForMember(a => a.UrgencyLevel, b => b.MapFrom(c => c.UrgencyLevel.ToString()))
                    .ForMember(a => a.PrintDate, b => b.MapFrom(c => DateTime.Now.ToString("yyyy-MM-dd")))
                    .ForMember(a => a.UrgencyTime, b => b.MapFrom(c => c.UrgencyTime == null ? "-" : c.UrgencyTime.Value.ToString("yyyy-MM-dd HH:mm:ss")));

                cfg.CreateMap<LetterSendEditDto, Letter>();

                cfg.CreateMap<Letter, LetterSendListDto>()
                    .ForMember(a => a.SendDepartmentName, b => b.MapFrom(c => c.SendDepartment.Name))
                    .ForMember(a => a.ReceiveDepartmentName, b => b.MapFrom(c => c.ReceiveDepartment.Name))
                    .ForMember(a => a.CreateUserName, b => b.MapFrom(c => c.CreateUser.DisplayName))
                    .ForMember(a => a.CreateTime, b => b.MapFrom(c => c.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")))
                    .ForMember(a => a.UrgencyTime, b => b.MapFrom(c => c.UrgencyTime == null ? "-" : c.UrgencyTime.Value.ToString("yyyy-MM-dd HH:mm:ss")));

                cfg.CreateMap<Letter, LetterBackListDto>()
                    .ForMember(a => a.SendDepartmentName, b => b.MapFrom(c => c.SendDepartment.Name))
                    .ForMember(a => a.ReceiveDepartmentName, b => b.MapFrom(c => c.ReceiveDepartment.Name))
                    .ForMember(a => a.CreateUserName, b => b.MapFrom(c => c.CreateUser.DisplayName))
                    .ForMember(a => a.CreateTime, b => b.MapFrom(c => c.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")))
                    .ForMember(a => a.UrgencyTime, b => b.MapFrom(c => c.UrgencyTime == null ? "-" : c.UrgencyTime.Value.ToString("yyyy-MM-dd HH:mm:ss")));

                #endregion

                #region HardWare

                cfg.CreateMap<HardWareSaveDto, HardWare>();

                #endregion

                #region Client  

                cfg.CreateMap<ClientEditDto, Client>();
                cfg.CreateMap<Client, ClientListDto>()
                    .ForMember(a => a.PlaceName, b => b.MapFrom(c => c.Place.Name))
                    .ForMember(a => a.CreateUserName, b => b.MapFrom(c => c.CreateUser.DisplayName))
                    .ForMember(a => a.CreateUserName, b => b.MapFrom(c => c.CreateUser.DisplayName))
                    .ForMember(a => a.CreateTime, b => b.MapFrom(c => c.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")))
                    .ForMember(a => a.UpdateTime, b => b.MapFrom(c => c.UpdateTime == null ? "" : c.UpdateTime.Value.ToString("yyyy-MM-dd HH:mm:ss")));

                #endregion
            });
        }

    }
}
