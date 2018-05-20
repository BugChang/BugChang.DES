using AutoMapper;
using BugChang.DES.Application.Menus.Dtos;
using BugChang.DES.Domain.Entities;

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
            });
        }
    }
}
