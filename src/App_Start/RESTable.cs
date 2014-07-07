using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Results;
using Microsoft.AspNet.Identity;
using RESTable.Infrastructure.RESTable;

namespace blogapi.yngvenilsen.com
{
    public static class RESTable
    {
        public static void Configure(HttpConfiguration config)
        {
            config.ConfigureRESTable()
                .RoutePrefix("api/v4")
                .UseMembershipResource("users", "username", "password").AllowAnonymousUnlessSpecifiedOtherwise()
                .Resource("tags")
                .Resource("users").ExcludeProperties(new string[] { "password" }).AuthorizeWith<CanOnlyChangeOwnUser>();

        }
    }

    public class CanOnlyChangeOwnUser:  IResourceAuthorizer
    {
        public void Before(HttpActionContext actionContext)
        {
            if (actionContext.Request.Method != HttpMethod.Get && actionContext.Request.Method != HttpMethod.Post)
            {
                var item = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(actionContext.Request.Content.ReadAsStringAsync().Result, new {username = ""});

                if (!item.username.Equals(((ClaimsIdentity)Thread.CurrentPrincipal.Identity).Username(), StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new UnauthorizedAccessException();
                }
            }
        }
    }
}