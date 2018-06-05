using System;
using AutoMapper;
using BugChang.DES.Application.Departments.Dtos;
using BugChang.DES.Application.Menus.Dtos;
using BugChang.DES.Core.Authorization.Menus;
using BugChang.DES.Core.Departments;

namespace BugChang.DES.Application
{
    public static class DesMapper
    {
        public static void Initialize()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Menu, MenuDto>();
                cfg.CreateMap<MenuDto, Menu>();
                cfg.CreateMap<MenuEditDto, Menu>();

                cfg.CreateMap<Department, DepartmentDto>()
                    .ForMember(a => a.ParentName, b => b.MapFrom(c => c.Parent.Name))
                    .ForMember(a => a.CreateUserName, b => b.MapFrom(c => c.CreateUser.DisplayName))
                    .ForMember(a => a.UpdateUserName, b => b.MapFrom(c => c.UpdateUser.DisplayName))
                    .ForMember(a => a.CreateTime, b => b.MapFrom(c => c.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")))
                    .ForMember(a => a.UpdateTime, b => b.MapFrom(c => c.UpdateTime == null ? "" : c.UpdateTime.Value.ToString("yyyy-MM-dd HH:mm:ss")));
                cfg.CreateMap<DepartmentEditDto, Department>();
            });
        }
    }
}
