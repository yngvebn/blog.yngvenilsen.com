using System.Web.Http.Controllers;

namespace RESTable.Infrastructure.RESTable
{
    public interface IRESTableConfiguration
    {
        IResourceConfiguration AddResource(string name);
        IResourceConfiguration AddResource(string name, string idProperty);
        void AuthorizeBefore(string resourceName, HttpActionContext actionContext);
        IRESTableConfiguration OnlyExplicitResourcesAllowed();
        IRESTableConfiguration RoutePrefix(string routePrefix);
        IRESTableConfiguration UseMembershipResource(string resourceName, string userNameProperty, string passwordProperty);
        IRESTableConfiguration AllowAnonymousUnlessSpecifiedOtherwise();
        RESTableAuthenticationOptions AuthenticationOptions { get; }
        bool AllowAnonymous { get; }
        bool AnonymousAllowedFor(string resource);
        IResourceConfiguration ResourceConfiguration(string resource);
       
    }
}