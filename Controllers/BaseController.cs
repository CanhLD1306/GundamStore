using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GundamStore.Controllers
{
    public class BaseController : Controller
    {
        // public override void OnActionExecuting(ActionExecutingContext filterContext)
        // {
        //     if (User.Identity?.IsAuthenticated != true)
        //     {
        //         filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "Account", Action = "login" }));
        //     }
        //     base.OnActionExecuting(filterContext);
        // }
    }
}