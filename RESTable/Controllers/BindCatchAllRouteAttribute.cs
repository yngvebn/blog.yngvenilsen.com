using System.Web.Http;
using System.Web.Http.Controllers;

namespace RESTable.Controllers
{
    public class BindCatchAllRouteAttribute : ParameterBindingAttribute
    {

        private readonly char _delimiter;

        public BindCatchAllRouteAttribute(char delimiter)
        {

            _delimiter = delimiter;
        }

        public override HttpParameterBinding GetBinding(HttpParameterDescriptor parameter)
        {

            return new CatchAllRouteParameterBinding(parameter, _delimiter);
        }
    }
}