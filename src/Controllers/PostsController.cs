﻿using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Http;
using System.Web.Routing;
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
            var table = TableStorage.Table(resource);

            var query = new TableQuery<ElasticTableEntity>();
            dynamic result = table.ExecuteQuery(query).ToList();
            return Ok(result);
        }

        [Route("{resource}/{id}"), HttpGet]
        public IHttpActionResult Get(string resource, string id)
        {
            var table = TableStorage.Table(resource);

            var query = new TableQuery<ElasticTableEntity>();
            dynamic result = table.ExecuteQuery(query).First(e => e.RowKey.Equals(id) && e.PartitionKey.Equals(resource));
            return Ok(result);
        }

        [Route("{resource}/{id}"), HttpDelete]
        public IHttpActionResult Remove(string resource, string id)
        {
            var table = TableStorage.Table(resource);
            var operation = TableOperation.Retrieve(resource, id);
            TableResult retrievedResult = table.Execute(operation);
            var entity = (DynamicTableEntity)retrievedResult.Result;
            
            if (entity == null) return NotFound();

            table.Execute(TableOperation.Delete(entity));
            return Ok();
        }

        [Route("{resource}/{id}"), HttpPut]
        public IHttpActionResult Update(string resource, string id)
        {
            string result = Request.Content.ReadAsStringAsync().Result;
            dynamic originalRequestData = JsonConvert.DeserializeObject<ElasticTableEntity>(result);
            originalRequestData.PartitionKey = resource;
            originalRequestData.RowKey = id;

            var table = TableStorage.Table(resource);
            var operation = TableOperation.Retrieve(resource, id);
            
            TableResult retrievedResult = table.Execute(operation);
            var entity = (DynamicTableEntity)retrievedResult.Result;
            if (entity == null) return NotFound();
                table.Execute(TableOperation.Delete(entity));
                
            table.Execute(TableOperation.Insert(originalRequestData));
            return Ok(originalRequestData);
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
            table.Execute(TableOperation.Insert(obj));


            return Created(new Uri(string.Format("/api/v1/{0}/{1}", resource, obj.RowKey), UriKind.Relative), obj);

        }
    }
}

