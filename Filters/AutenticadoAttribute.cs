namespace Project_AG.Filters
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;

    public class AutenticadoAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var usuario = context.HttpContext.Session.GetString("UsuarioLogeado");
            if (string.IsNullOrEmpty(usuario))
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                        { "controller", "Home" },
                        { "action", "Index" }
                    });
            }
        }
    }
}
