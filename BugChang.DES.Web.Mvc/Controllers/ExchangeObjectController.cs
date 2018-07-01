using System.Collections.Generic;
using System.Linq;
using BugChang.DES.Application.Channels;
using BugChang.DES.Application.Channels.Dtos;
using BugChang.DES.Application.ExchangeObjects;
using BugChang.DES.Web.Mvc.Models.Common;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BugChang.DES.Web.Mvc.Controllers
{
    public class ExchangeObjectController : BaseController
    {

        private readonly IChannelAppService _channelAppService;
        private readonly IExchangeObjectAppService _exchangeObjectAppService;

        public ExchangeObjectController(IChannelAppService channelAppService, IExchangeObjectAppService exchangeObjectAppService)
        {
            _channelAppService = channelAppService;
            _exchangeObjectAppService = exchangeObjectAppService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetChannels()
        {
            var channels = _channelAppService.GetChannels();
            var json = channels.Select(a => new SelectViewModel
            {
                Id = a.Id,
                Text = a.Name
            });
            return Json(json);
        }

        public IActionResult GetObjectTypes()
        {
            var objectTypes = _exchangeObjectAppService.GetObjectTypes();
            var json = objectTypes.Select(a => new SelectViewModel
            {
                Id = a.Id,
                Text = a.Name
            });
            return Json(json);
        }
    }
}
