using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace mvc_first_task.ActionFilters
{
    public class RoleValidationAttribute : ActionFilterAttribute
    {
        private readonly string _requiredRole;

        public RoleValidationAttribute(string requiredRole)
        {
            _requiredRole = requiredRole;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Get the role from the user's claims
            var userRole = context.HttpContext.User.FindFirstValue(ClaimTypes.Role);

            if (userRole != _requiredRole)
            {
                // Redirect to the NotAuth action if the role doesn't match
                context.Result = new RedirectToActionResult("NotAuth", "Account", null);
            }
            else if (string.IsNullOrEmpty(userRole))
            {
                // Redirect to the Login page if the role is missing
                context.Result = new RedirectToActionResult("Index", "Account", null);
            }

            base.OnActionExecuting(context);
        }
    }
}
