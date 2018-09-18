using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace BugChang.DES.Web.Mvc.Filters
{
    public class RefererFilter : Attribute, IActionFilter
    {
        private readonly IOptions<CommonSettings> _commonSettings;

        public RefererFilter(IOptions<CommonSettings> commonSettings)
        {
            _commonSettings = commonSettings;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var referer = context.HttpContext.Request.Headers["Referer"].ToString().Trim();
            if (!string.IsNullOrEmpty(referer))
            {
                if (!referer.StartsWith(_commonSettings.Value.Referer))
                {
                    context.HttpContext.ForbidAsync();
                    context.Result = new ContentResult()
                    {
                        Content = "可疑的请求！"
                    };
                }
            }

        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
    }
}
