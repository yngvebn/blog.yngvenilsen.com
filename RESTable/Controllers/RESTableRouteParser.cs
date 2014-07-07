using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using RESTable.Infrastructure.RESTable;

namespace RESTable.Controllers
{
    public class RESTableRouteParser : ActionFilterAttribute, IOrderedFilter
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ActionArguments.ContainsKey("path"))
                actionContext.ActionArguments.Add("path", actionContext.ControllerContext.RouteData.Values["path"]);
            
            
            base.OnActionExecuting(actionContext);
        }

        public int Order { get; set; }
    }
}