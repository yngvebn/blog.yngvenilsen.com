using System;
using Newtonsoft.Json;

namespace blogapi.yngvenilsen.com.Controllers
{
    public class Resource
    {
        public string Name { get; set; }
        public string RowKey { get; set; }

        [JsonIgnore]
        public bool IsSingle
        {
            get { return !String.IsNullOrEmpty(RowKey); }
        }

        private Resource _child;

        public Resource Child { get; private set; }

        public string ChildRelation
        {
            get { return Child != null ? string.Format("{0}00{1}", Name, Child.Name) : null; }
        }

        public Resource CreateChild()
        {
            Child = new Resource();
            return Child;
        }

        [JsonIgnore]
        public bool IsDone
        {
            get { return !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(RowKey); }
        }

        public void Set(string segment)
        {
            if (string.IsNullOrEmpty(Name))
            {
                Name = segment;
            }
            else
            {
                RowKey = segment;
            }
        }
    }
}