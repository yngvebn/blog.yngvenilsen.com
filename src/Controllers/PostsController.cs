using System.Web.Http;

namespace blogapi.yngvenilsen.com.Controllers
{
    [RoutePrefix("api/v1")]
    public class PostsController: ApiController
    {
        [Route("Posts"), HttpGet]
        public string Test()
        {
            return "Hello";
        }
    }
}