using System.Collections.Generic;
using System.Linq;

namespace blogapi.yngvenilsen.com.Controllers
{
    public class ResourceTree
    {
        public Resource Resource { get; set; }

        private ResourceTree(IEnumerable<string> segments)
        {
            Resource = new Resource();
            var currentResource = Resource;
            foreach (var segment in segments)
            {
                if (currentResource.IsDone) currentResource = currentResource.CreateChild();

                currentResource.Set(segment);
            }
        }
        public static ResourceTree Parse(string path)
        {
            var segments = path.Split('/');
            return new ResourceTree(segments.Where(c => !string.IsNullOrEmpty(c)));
        }
    }
}