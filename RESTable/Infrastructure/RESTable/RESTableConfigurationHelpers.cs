using System.Web.Http;

namespace RESTable.Infrastructure.RESTable
{
    public static class RESTableConfigurationHelpers
    {
        private static IRESTableConfiguration _config;

        public static IRESTableConfiguration RESTableConfiguration()
        {
            return _config;
        }

        public static IRESTableConfiguration ConfigureRESTable(this HttpConfiguration config)
        {
            RESTableConfiguration resTableConfiguration = new RESTableConfiguration(config);

            _config = resTableConfiguration;
            return resTableConfiguration;
        }


        public static IResourceConfiguration InterceptWith<T>(this IResourceConfiguration configuration) where T : IResourceInterceptor
        {
            configuration.AddInterceptor(typeof (T));
            return configuration;
        }

        public static IResourceConfiguration AuthorizeWith<T>(this IResourceConfiguration configuration) where T : IResourceAuthorizer
        {
            configuration.AddAuthorizer(typeof (T));
            return configuration;
        }

        public static IResourceConfiguration Resource(this IRESTableConfiguration configuration, string name)
        {
            return configuration.AddResource(name);
        }

        public static IResourceConfiguration Resource(this IRESTableConfiguration configuration, string name, string idProperty)
        {
            return configuration.AddResource(name, idProperty);
        }

    }
}