using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TenderTracker.Filter
{
    public class LoginFilter : IActionFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public LoginFilter(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string user_type = _session.GetString("user_type");
            if (string.IsNullOrEmpty(user_type))
            {

                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    {"controller","Login" },
                    {"action","LoginForm" }
                });
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

    }
}