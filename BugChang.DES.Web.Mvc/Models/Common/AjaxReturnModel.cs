namespace BugChang.DES.Web.Mvc.Models.Common
{
    public class AjaxReturnModel
    {
        public int Code { get; set; }

        public string Message { get; set; }

        public dynamic Data { get; set; }
    }
}
