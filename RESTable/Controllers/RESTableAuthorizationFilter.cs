using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using RESTable.Infrastructure.RESTable;

namespace RESTable.Controllers
{
    public class RESTableOwinTokenAuthorizationFilter: AuthorizationFilterAttribute, IOrderedFilter
    {
        public override Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            AddActionArgumentFromRoute(actionContext, "path");
            var config = RESTableConfigurationHelpers.RESTableConfiguration();
             ResourceTree tree = ResourceTree.Parse(actionContext.ActionArguments["path"].ToString());
            var finalResource = tree.FinalResource;
            if (config.AllowAnonymous && config.AnonymousAllowedFor(finalResource.Name))
            {
                return base.OnAuthorizationAsync(actionContext, cancellationToken);
            }
            if (Thread.CurrentPrincipal != null && (Thread.CurrentPrincipal.Identity.IsAuthenticated))
            {
                return base.OnAuthorizationAsync(actionContext, cancellationToken);
            }
                
                throw new UnauthorizedAccessException();
        }

        private void AddActionArgumentFromRoute(HttpActionContext actionContext, string path)
        {
            if (!actionContext.ActionArguments.ContainsKey("path"))
                actionContext.ActionArguments.Add("path", actionContext.ControllerContext.RouteData.Values["path"]);
            
        }

        public int Order { get; set; }
    }


    public class RESTableExceptionHandler : ExceptionFilterAttribute, IOrderedFilter
    {
        public int Order { get; set; }
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception is UnauthorizedAccessException)
            {
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                return;
            }
            base.OnException(actionExecutedContext);
        }
    }

    public class RESTableAuthorizationFilter : ActionFilterAttribute, IOrderedFilter
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            
            base.OnActionExecuted(actionExecutedContext);
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var configuration = RESTableConfigurationHelpers.RESTableConfiguration();
            if (!actionContext.ActionArguments.ContainsKey("path")) return;
            ResourceTree tree = ResourceTree.Parse(actionContext.ActionArguments["path"].ToString());
            var finalResource = tree.FinalResource;

            configuration.AuthorizeBefore(finalResource.Name, actionContext);
        }

        public int Order { get; set; }
    }
}