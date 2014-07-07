using System;
using System.Web.Http.Controllers;

namespace RESTable.Infrastructure.RESTable
{
    public interface IResourceConfiguration
    {
        string Name { get; }
        string IdProperty { get; }
        bool AllowAnonymous { get; }
        IRESTableConfiguration End();
        IResourceConfiguration Resource(string name);
        IResourceConfiguration Resource(string name, string idProperty);
        void AddInterceptor(Type type);
        void AddAuthorizer(Type type);
        void AuthorizeBefore(HttpActionContext actionContext);
        IResourceConfiguration RequiresAuthentication();
        IResourceConfiguration ExcludeProperties(string[] properties);
        string[] ExcludedProperties { get; }
    }
}