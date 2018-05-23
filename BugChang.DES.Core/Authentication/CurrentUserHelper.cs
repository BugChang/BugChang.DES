using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace BugChang.DES.Core.Authentication
{
    public static class CurrentUserHelper
    {
        public static int Id { get; private set; }
        public static string UserName { get; private set; }
        public static string DisplayName { get; private set; }

        public static void Initialize(List<Claim> claims)
        {
            Id = Convert.ToInt32(claims.Find(a => a.Type == "Id").Value);
            UserName = claims.Find(a => a.Type == "UserName").Value;
            DisplayName = claims.Find(a => a.Type == "DisplayName").Value;
        }
    }
}
