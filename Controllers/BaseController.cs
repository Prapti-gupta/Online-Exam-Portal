using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using onlineexamproject.Extensions;
using onlineexamproject.Models;

namespace onlineexamproject.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var sessionUser = HttpContext.Session.GetObjectFromJson<User>("sessionUser");
            if (sessionUser != null)
            {
                ViewBag.Username = sessionUser.Username;
                ViewBag.Id = sessionUser.Id;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}


