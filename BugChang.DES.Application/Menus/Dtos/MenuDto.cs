using System.Collections.Generic;

namespace BugChang.DES.Application.Menus.Dtos
{
    /// <summary>
    /// 菜单
    /// </summary>

    public class MenuDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public string Icon { get; set; }

        public int? ParentId { get; set; }

        public IList<MenuDto> Items { get; set; }

    }
}
