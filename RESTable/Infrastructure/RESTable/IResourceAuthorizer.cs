using System.Web.Http.Controllers;

namespace RESTable.Infrastructure.RESTable
{
    public interface IResourceAuthorizer {
        void Before(HttpActionContext actionContext);
    }
}