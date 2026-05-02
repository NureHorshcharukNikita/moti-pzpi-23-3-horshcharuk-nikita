using Lab3.Constants;

namespace Lab3.Helpers
{
    public static class AuthHelper
    {
        public static bool IsLoggedIn(HttpContext context)
        {
            return context.Session.GetInt32("UserID") != null;
        }

        public static bool IsAdmin(HttpContext context)
        {
            return IsLoggedIn(context) && GetRole(context) == Roles.Admin;
        }

        public static string? GetUserName(HttpContext context)
        {
            return context.Session.GetString("Username");
        }

        public static int? GetUserId(HttpContext context)
        {
            return context.Session.GetInt32("UserID");
        }

        public static int? GetEvaluatorId(HttpContext context)
        {
            return context.Session.GetInt32("EvaluatorID");
        }

        public static string? GetRole(HttpContext context)
        {
            return context.Session.GetString("Role");
        }
    }
}
