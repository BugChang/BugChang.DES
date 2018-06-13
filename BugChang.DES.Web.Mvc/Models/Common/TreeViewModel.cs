using System.Collections.Generic;

namespace BugChang.DES.Web.Mvc.Models.Common
{
    public class TreeViewModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }

        public bool Open { get; set; }

        public bool IsParent { get; set; }

        public string CustomData { get; set; }

        public IList<TreeViewModel> Children { get; set; }
    }

    public class SimpleTreeViewModel
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public bool Checked { get; set; }

        public int? ParentId { get; set; }
    }
}
