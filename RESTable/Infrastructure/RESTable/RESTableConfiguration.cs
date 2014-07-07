using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace RESTable.Infrastructure.RESTable
{
    //NOTE: Here I am creating base attributes which you would need to inherit from.

    public class RESTableConfiguration : IRESTableConfiguration
    {
        private readonly HttpConfiguration _configuration;
        public RESTableAuthenticationOptions AuthenticationOptions { get; private set; }
        public bool AllowAnonymous { get; private set; }

        public bool AnonymousAllowedFor(string resource)
        {
            if (!_resourceConfigurations.ContainsKey(resource)) return AllowAnonymous;
            return _resourceConfigurations[resource].AllowAnonymous;
        }

        public IResourceConfiguration ResourceConfiguration(string resource)
        {
            return _resourceConfigurations[resource];
        }

        private bool _explicitResourcesOnly;
        private IDictionary<string, IResourceConfiguration> _resourceConfigurations;

        public RESTableConfiguration(HttpConfiguration configuration)
        {
            _configuration = configuration;
            _resourceConfigurations = new Dictionary<string, IResourceConfiguration>();
            ReplaceFilterHandling(configuration);
        }

        private void ReplaceFilterHandling(HttpConfiguration config)
        {
            // Start clean by replacing with filter provider for global configuration.
            // For these globally added filters we need not do any ordering as filters are 
            // executed in the order they are added to the filter collection
            config.Services.Replace(typeof(IFilterProvider), new System.Web.Http.Filters.ConfigurationFilterProvider());

            // Custom action filter provider which does ordering
            config.Services.Add(typeof(IFilterProvider), new OrderedFilterProvider());
        }

        public IResourceConfiguration AddResource(string name)
        {
            var resource = new ResourceConfiguration(this, name);
            _resourceConfigurations.Add(name, resource);
            return resource;
        }

        public IResourceConfiguration AddResource(string name, string idProperty)
        {
            var resource = new ResourceConfiguration(this, name, idProperty);
            _resourceConfigurations.Add(name, resource);
            return resource;
        }

 
        public void AuthorizeBefore(string finalResource, HttpActionContext actionContext)
        {
            if (!_resourceConfigurations.ContainsKey(finalResource) && !_explicitResourcesOnly) return;

            if (!_resourceConfigurations.ContainsKey(finalResource) && _explicitResourcesOnly)
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.NotFound);
                return;
            }

            var resourceConfiguration = _resourceConfigurations[finalResource];
            resourceConfiguration.AuthorizeBefore(actionContext);
        }

        public IRESTableConfiguration OnlyExplicitResourcesAllowed()
        {
            _explicitResourcesOnly = true;
            return this;
        }

        public IRESTableConfiguration RoutePrefix(string routePrefix)
        {
            _configuration.Routes.MapHttpRoute(name: "RESTable", routeTemplate:string.Format("{0}{1}", AddTrailingSlash(routePrefix), "{*path}"), defaults:new
            {
                controller = "RESTable"
            });
            return this;
        }

        public IRESTableConfiguration UseMembershipResource(string resourceName, string userNameProperty, string passwordProperty)
        {
            AuthenticationOptions = new RESTableAuthenticationOptions()
            {
                ResourceName = resourceName, PasswordProperty = passwordProperty, UsernameProperty = userNameProperty
            };

            return this;
        }

        public IRESTableConfiguration AllowAnonymousUnlessSpecifiedOtherwise()
        {
            AllowAnonymous = true;
            return this;
        }

        private string AddTrailingSlash(string routePrefix)
        {
            if (!routePrefix.EndsWith("/")) routePrefix += "/";
            return routePrefix;
        }
    }
};