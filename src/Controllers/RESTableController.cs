using System;
using System.Linq;
using System.Web.Http;
using blogapi.yngvenilsen.com.Infrastructure.Azure;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace blogapi.yngvenilsen.com.Controllers
{
    [RoutePrefix("api/v2")]
    public class RESTableController : ApiController
    {
        [Route("{*path}"), HttpGet]
        public IHttpActionResult Get(string path)
        {
            var tree = ResourceTree.Parse(path);
            if (!tree.Resource.IsSingle)
            {
                return Ok(TableStorage.Get(tree.Resource.Name));
            }
            if (tree.Resource.ChildRelation == null)
            {
                var elasticTableEntity = TableStorage.Get(tree.Resource.Name, new RowKey(tree.Resource.RowKey));
                if (elasticTableEntity == null) return NotFound();

                return Ok(elasticTableEntity);
            }

            // looking for childresource(s)
            var currentResource = tree.Resource;
            while (currentResource.ChildRelation != null)
            {
                var relationTable = currentResource.ChildRelation;
                var childResource = currentResource.Child;

                // does the current resource exist?
                if (TableStorage.Get(currentResource.Name, new RowKey(currentResource.RowKey)) == null) return NotFound();

                // are we looking for a specific child-entity?
                if (childResource.IsSingle)
                {
                    // this is the end of the road
                    if (childResource.ChildRelation == null)
                    {
                        var entity = TableStorage.Get(childResource.Name, new RowKey(childResource.RowKey));
                        if (entity == null) return NotFound();

                        return Ok(entity);
                    }
                }
                else
                {
                    // nope - multiple, so just return the result
                    var related = TableStorage.Get(relationTable, new PartitionKey(currentResource.RowKey)).Select(t => new RowKey(t.RowKey));
                    return Ok(TableStorage.Get(childResource.Name, related));
                }
                currentResource = childResource;
            }


            return NotFound();
        }

        [Route("{*path}"), HttpDelete]
        public IHttpActionResult Delete(string path)
        {
            var tree = ResourceTree.Parse(path);
            if (!tree.Resource.IsSingle)
            {
                return BadRequest("Can't delete multiple items");
            }
            if (tree.Resource.ChildRelation == null)
            {
               TableStorage.Delete(tree.Resource.Name, tree.Resource.RowKey);

                return Ok();
            }

            // looking for childresource(s)
            var currentResource = tree.Resource;
            while (currentResource.ChildRelation != null)
            {
                var relationTable = currentResource.ChildRelation;
                var childResource = currentResource.Child;

                // does the current resource exist?
                if (TableStorage.Get(currentResource.Name, new RowKey(currentResource.RowKey)) == null) return NotFound();

                // are we looking for a specific child-entity?
                if (childResource.IsSingle)
                {
                    // this is the end of the road
                    if (childResource.ChildRelation == null)
                    {
                        TableStorage.Delete(relationTable, childResource.RowKey);

                        return Ok();
                    }
                }
                else
                {
                    return BadRequest("Can't delete multiple items");
                }
                currentResource = childResource;
            }


            return NotFound();
        }

        


        [Route("{*path}"), HttpPut]
        public IHttpActionResult Update(string path)
        {
            string result = Request.Content.ReadAsStringAsync().Result;

            var obj = JsonConvert.DeserializeObject<ElasticTableEntity>(result);
            
            var tree = ResourceTree.Parse(path);
            if (!tree.Resource.IsSingle)
            {
                return BadRequest("Can't update multiple items");
            }
            if (tree.Resource.ChildRelation == null)
            {
                var res = TableStorage.Update(tree.Resource.Name, tree.Resource.RowKey, obj);

                return Ok(res);
            }

            // looking for childresource(s)
            var currentResource = tree.Resource;
            while (currentResource.ChildRelation != null)
            {
                var relationTable = currentResource.ChildRelation;
                var childResource = currentResource.Child;

                // does the current resource exist?
                if (TableStorage.Get(currentResource.Name, new RowKey(currentResource.RowKey)) == null) return NotFound();

                // are we looking for a specific child-entity?
                if (childResource.IsSingle)
                {
                    // this is the end of the road
                    if (childResource.ChildRelation == null)
                    {
                        var res = TableStorage.Update(childResource.Name, childResource.RowKey, obj);

                        return Ok(res);
                    }
                }
                else
                {
                    return BadRequest("Can't update multiple items");
                }
                currentResource = childResource;
            }


            return NotFound();
        }

        [Route("{*path}"), HttpPost]
        public IHttpActionResult Create(string path)
        {
            string result = Request.Content.ReadAsStringAsync().Result;

            var obj = JsonConvert.DeserializeObject<ElasticTableEntity>(result);
            obj.PartitionKey = "anonymous";
            var tree = ResourceTree.Parse(path);
            if (tree.Resource.IsSingle && tree.Resource.ChildRelation == null)
            {
                return BadRequest("Missing child resource");
            }
            if (tree.Resource.ChildRelation == null)
            {
                
                TableStorage.Create(tree.Resource.Name, obj);
                return Created(new Uri(string.Format("/api/v2/{0}/{1}", tree.Resource.Name, obj.RowKey), UriKind.Relative), obj);
            }

            // looking for childresource(s)
            var currentResource = tree.Resource;
            while (currentResource.ChildRelation != null)
            {
                var relationTable = currentResource.ChildRelation;
                var childResource = currentResource.Child;

                // does the current resource exist?
                if (TableStorage.Get(currentResource.Name, new RowKey(currentResource.RowKey)) == null) return NotFound();

                // are we looking for a specific child-entity?
                if (childResource.IsSingle)
                {
                    // this is the end of the road
                    if (childResource.ChildRelation == null)
                    {
                        return BadRequest("Missing child resource");
                    }
                }
                else
                {
                    // nope - multiple, so create a child here.
                    TableStorage.Create(relationTable, currentResource.RowKey, childResource.Name, obj);
                    return Created(new Uri(string.Format("/api/v2/{0}/{1}", childResource.Name, obj.RowKey), UriKind.Relative), obj);
                }
                currentResource = childResource;
            }


            return NotFound();
        }
    }
}