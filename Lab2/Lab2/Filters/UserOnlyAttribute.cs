using Lab2.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Lab2.Filters
{
    public class UserOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!AuthHelper.IsLoggedIn(context.HttpContext))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            if (AuthHelper.IsAdmin(context.HttpContext))
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}