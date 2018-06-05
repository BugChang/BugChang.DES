using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace BugChang.DES.Core.Authorization
{
    public class DesdRequirement : IAuthorizationRequirement
    {
        public string Name { get; set; }
    }

    public static class DesOperations
    {
        public static DesdRequirement Create = new DesdRequirement { Name = "Create" };
        public static DesdRequirement Read = new DesdRequirement { Name = "Read" };
        public static DesdRequirement Update = new DesdRequirement { Name = "Update" };
        public static DesdRequirement Delete = new DesdRequirement { Name = "Delete" };
    }
}
