using System.Collections.Generic;
using System.Linq;

namespace RESTable.Infrastructure.RESTable
{
    public class ResourceTree
    {
        public Resource Resource { get; set; }

        public Resource FinalResource
        {
            get
            {
                if (this.Resource.Child == null) return this.Resource;
                var _child = Resource.Child;
                while (_child.Child != null)
                {
                    _child = _child.Child;
                }
                return _child;
            }
        }

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
                return Parse(segments);
        }
        public static ResourceTree Parse(string[] segments)
        {
            return new ResourceTree(segments.Where(c => !string.IsNullOrEmpty(c)));
        }
    }
}