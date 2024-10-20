using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace project_ca_nhan.Areas.Admin.Attributes
{
    public class CheckLogin : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (String.IsNullOrEmpty(context.HttpContext.Session.GetString("Admin_user_email")))
            {
                context.HttpContext.Response.Redirect("/Admin/Account/login");
            }
            base.OnActionExecuting(context);
        }
    }
}
