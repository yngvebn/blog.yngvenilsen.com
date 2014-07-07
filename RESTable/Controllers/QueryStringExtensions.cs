using System.Collections.Specialized;
using System.Web;

namespace RESTable.Controllers
{
    public static class QueryStringExtensions
    {
        public static NameValueCollection ToNameValue(this string queryString)
        {
            return HttpUtility.ParseQueryString(queryString);
        }
    }
}