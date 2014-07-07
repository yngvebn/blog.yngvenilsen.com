using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security.OAuth;
using RESTable.Infrastructure.Azure;
using RESTable.Infrastructure.RESTable;

namespace blogapi.yngvenilsen.com
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //if (context.OwinContext.Response.Headers.ContainsKey("Access-Control-Allow-Origin")) context.OwinContext.Response.Headers.Remove
            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            var user = TableStorage.FindUser(context.UserName, context.Password);

            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("sub", context.UserName));
            identity.AddClaim(new Claim("role", "user"));

            context.Validated(identity);

        }
    }

    public static class ClaimsIdentityHelpers
    {
        public static string Username(this ClaimsIdentity identity)
        {
            var firstOrDefault = identity.Claims.FirstOrDefault(c => c.Type.Equals("sub"));
            if (firstOrDefault != null) return firstOrDefault.Value;

            return "";
        }
    }
}