using System.Threading.Tasks;
using BugChang.DES.Application.Letters;
using BugChang.DES.Application.Letters.Dtos;
using Microsoft.AspNetCore.Mvc;
namespace BugChang.DES.Web.Mvc.Controllers
{
    public class LetterController : BaseController
    {
        private readonly ILetterAppService _letterAppService;

        public LetterController(ILetterAppService letterAppService)
        {
            _letterAppService = letterAppService;
        }

        public async Task<IActionResult> Receive(int id)
        {
            var receiveLetter = new ReceiveLetterEditDto();
            if (id > 0)
            {
                receiveLetter = await _letterAppService.GetReceiveLetter(id);
            }
            return View(receiveLetter);
        }

        public IActionResult Send()
        {
            return View();
        }
    }
}
