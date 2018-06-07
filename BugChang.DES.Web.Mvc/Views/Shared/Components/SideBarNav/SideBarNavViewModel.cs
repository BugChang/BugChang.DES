using System.Collections.Generic;
using BugChang.DES.Application.Menus.Dtos;

namespace BugChang.DES.Web.Mvc.Views.Shared.Components.SideBarNav
{
    public class SideBarNavViewModel
    {
        public IList<MenuListDto> Menus { get; set; }

        public string ActiveMenuName { get; set; }
    }
}
