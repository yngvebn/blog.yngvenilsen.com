using System;
using System.Collections.Specialized;
using System.Web.Http;
using blogapi.yngvenilsen.com.Infrastructure.Azure;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace blogapi.yngvenilsen.com.Controllers
{
    [RoutePrefix("api/v1")]
    public class RestAPIController: ApiController
    {
        [Route("{resource}"), HttpGet]
        public IHttpActionResult Get(string resource)
        {
            return Ok(new
            {
                resource
            });
        }

        [Route("{resource}/{*id}"), HttpGet]
        public IHttpActionResult Get(string resource, string id)
        {
            return Ok(new
            {
                resource,
                id
            });
        }

        [Route("{resource}/{id*}"), HttpDelete]
        public IHttpActionResult Remove(string resource, string id)
        {
            var table = TableStorage.Table(resource);
            var operation = TableOperation.Retrieve(resource, id);
            TableResult retrievedResult = table.Execute(operation);
            var entity = (DynamicTableEntity)retrievedResult.Result;
            table.Execute(TableOperation.Delete(entity));
            return Ok();
        }

        [Route("{resource}"), HttpPost]
        public IHttpActionResult Create(string resource)
        {
            string result = Request.Content.ReadAsStringAsync().Result;
            dynamic originalRequestData = JsonConvert.DeserializeObject<dynamic>(result);
            var obj = JsonConvert.DeserializeObject<ElasticTableEntity>(result);
            var table = TableStorage.Table(resource);
            obj.PartitionKey = resource;
            obj.RowKey = originalRequestData.id.ToString();
            //dynamic entity = new ElasticTableEntity();
            //entity.PartitionKey = "Partition123";
            //entity.RowKey = (DateTime.MaxValue.Ticks - DateTime.Now.Ticks).ToString();
            //entity.Name = "Pascal";
            //entity.Number = 34;
            //entity.Bool = false;
            //entity.Date = new DateTime(1912, 3, 4);
            //entity.TokenId = Guid.NewGuid();
            //entity.NewColumn = "HElo world!"
            //entity["LastName"] = "Laurin";

            // Insert the entity we created dynamically
            table.Execute(TableOperation.Insert(obj));


            return Ok(obj);

        }
    }
}

