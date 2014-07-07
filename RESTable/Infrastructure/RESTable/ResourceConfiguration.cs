using System;
using System.Collections.Generic;
using System.Web.Http.Controllers;

namespace RESTable.Infrastructure.RESTable
{
    public class ResourceConfiguration : IResourceConfiguration
    {
        private readonly IRESTableConfiguration _parent;

        private IList<Type> _interceptors;
        private IList<Type> _authorizers;

        public ResourceConfiguration(IRESTableConfiguration parent, string name, string idProperty = "id")
            : base()
        {
            _parent = parent;
            IdProperty = idProperty;
            Name = name;
            _interceptors = new List<Type>();
            _authorizers = new List<Type>();
        }

        public string Name { get; private set; }
        public string IdProperty { get; private set; }
        public bool AllowAnonymous { get; private set; }
        public string[] ExcludedProperties { get; private set; }

        public IRESTableConfiguration End()
        {
            return _parent;
        }

        public IResourceConfiguration Resource(string name)
        {
            return _parent.AddResource(name);
        }

        public IResourceConfiguration Resource(string name, string idProperty)
        {
            return _parent.AddResource(name, idProperty);
        }

        public IRESTableConfiguration Resource()
        {
            return _parent;
        }

        public void AddInterceptor(Type type)
        {
            _interceptors.Add(type);
        }

        public void AddAuthorizer(Type type)
        {
            _authorizers.Add(type);
        }

        public void AuthorizeBefore(HttpActionContext actionContext)
        {
            foreach (var authorizer in _authorizers)
            {
                var authorizerInstance = (IResourceAuthorizer) Activator.CreateInstance(authorizer);
                authorizerInstance.Before(actionContext);
            }
        }

        public IResourceConfiguration RequiresAuthentication()
        {
            AllowAnonymous = false;
            return this;
        }

        public IResourceConfiguration ExcludeProperties(string[] properties)
        {
            this.ExcludedProperties = properties;
            return this;
        }

        
    }
}