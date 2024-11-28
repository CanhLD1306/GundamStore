using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using GundamStore.Common;
using Microsoft.AspNetCore.Authorization;

namespace GundamStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Roles.Admin)]
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "Account", Action = "login" }));
            }
            base.OnActionExecuting(filterContext);
        }
    }
}