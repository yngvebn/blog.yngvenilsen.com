using System;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;

[assembly: OwinStartup(typeof(blogapi.yngvenilsen.com.Startup))]
namespace blogapi.yngvenilsen.com
{
    public class Startup
    {
        public void Configuration(IAppBuilder builder)
        {
            ConfigureOAuth(builder);
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            RESTable.Configure(config);
            builder.UseWebApi(config);
        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            OAuthAuthorizationServerOptions options = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/api/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new SimpleAuthorizationServerProvider()
            };
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseOAuthAuthorizationServer(options);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }

}
