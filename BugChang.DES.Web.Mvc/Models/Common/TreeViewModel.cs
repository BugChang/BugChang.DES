﻿using System.Collections.Generic;

namespace BugChang.DES.Web.Mvc.Models.Common
{
    public class TreeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public bool Open { get; set; }

        public bool IsParent { get; set; }

        public IList<TreeViewModel> Children { get; set; }
    }
}