using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using GundamStore.Common;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace GundamStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    // [Authorize(Roles = "Admin")]
    public class BaseController : Controller
    {
        // GET: Admin/Base
        // public override void OnActionExecuting(ActionExecutingContext filterContext)
        // {
        //     var session = HttpContext.Session.GetObjectFromJson<UserLogin>(Constant.ADMIN_SESSION);
        //     if (session == null)
        //     {
        //         filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "Login", Action = "Index", Areas = "Admin" }));
        //     }
        //     base.OnActionExecuting(filterContext);
        // }
    }
}