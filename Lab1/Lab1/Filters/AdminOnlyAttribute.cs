using Lab1.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Lab1.Filters
{
    public class AdminOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!AuthHelper.IsLoggedIn(context.HttpContext))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            if (!AuthHelper.IsAdmin(context.HttpContext))
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}