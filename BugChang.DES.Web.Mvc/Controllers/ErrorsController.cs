using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BugChang.DES.Web.Mvc.Controllers
{
    public class ErrorsController : BaseController
    {

        [Route("Errors/{statusCode}")]
        public IActionResult CustomError(string statusCode)
        {
            switch (statusCode)
            {
                case "404":
                    return View("404");
            }

            return View("500");
        }
    }
}
