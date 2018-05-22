using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace BugChang.DES.Web.Mvc.Models
{
    public static class CurrentUserModel
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
