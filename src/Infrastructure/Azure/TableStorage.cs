using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace blogapi.yngvenilsen.com.Infrastructure.Azure
{
    public class TableStorage
    {
        public static CloudTable Table(string name)
        {
            CloudStorageAccount storageAccount =
                CloudStorageAccount.Parse(
                    ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);

            var client = storageAccount.CreateCloudTableClient();
            var table = client.GetTableReference(name);
            table.CreateIfNotExists();
            return table;
        }

        public static ElasticTableEntity Get(string resource, PartitionKey partition, RowKey rowkey)
        {
            var query = new TableQuery<ElasticTableEntity>();
            dynamic result = Table(resource).ExecuteQuery(query).SingleOrDefault(e => e.RowKey.Equals(rowkey.ToString()) && e.PartitionKey.Equals(partition.ToString()));
            return result;
        }

        public static ElasticTableEntity Get(string resource, RowKey rowKey)
        {
            var query = new TableQuery<ElasticTableEntity>();
            dynamic result = Table(resource).ExecuteQuery(query).SingleOrDefault(e => e.RowKey.Equals(rowKey.ToString()));
            return result;
        }

        public static IEnumerable<ElasticTableEntity> Get(string resource, PartitionKey partition)
        {
            var query = new TableQuery<ElasticTableEntity>();
            dynamic result = Table(resource).ExecuteQuery(query).Where(e => e.PartitionKey.Equals(partition.ToString()));
            return result;
        }

        public static IEnumerable<ElasticTableEntity> Get(string resource, IEnumerable<RowKey> rowKeys)
        {
            var query = new TableQuery<ElasticTableEntity>();
            dynamic result = Table(resource).ExecuteQuery(query).Where(e => rowKeys.Select(p => p.ToString()).Contains(e.RowKey));
            return result;
        }

        public static IEnumerable<ElasticTableEntity> Get(string resource)
        {
            var query = new TableQuery<ElasticTableEntity>();
            dynamic result = Table(resource).ExecuteQuery(query);
            return result;
        }

        public static void Create(string name, dynamic obj)
        {
            obj.RowKey = ResolveID(name, obj);
            Table(name).Execute(TableOperation.Insert(obj));
        }

        public static void Create(string relationTable, string parentRowKey, string name, dynamic obj)
        {
            obj.RowKey = ResolveID(name, obj);
            
            ElasticTableEntity tableentity = new ElasticTableEntity()
            {
                PartitionKey = parentRowKey,
                RowKey = obj.RowKey
            };

            if (Get(name, new PartitionKey(obj.PartitionKey), new RowKey(obj.RowKey)) == null)
            {
                Table(name).Execute(TableOperation.Insert(obj));
            }

            if (Get(relationTable, new PartitionKey(tableentity.PartitionKey), new RowKey(tableentity.RowKey)) != null)
                return;

            Table(relationTable).Execute(TableOperation.Insert(tableentity));

        }

        private static string ResolveID(string name, dynamic o)
        {
            if (name.Equals("tags", StringComparison.InvariantCultureIgnoreCase)) return o.name;

            return o.id.ToString();
        }

        private static ElasticTableEntity Merge(ElasticTableEntity item1, ElasticTableEntity item2)
        {
            foreach (var prop in item2.Properties)
            {
                item1[prop.Key] = item2.Properties[prop.Key];
            }


            return item1;
        }

        public static ElasticTableEntity Update(string name, string rowKey, ElasticTableEntity updatedObject)
        {
            var table = Table(name);


            var entity = Get(name, new RowKey(rowKey));

            if (entity == null) throw new EntityNotFoundException();

            dynamic res = Merge(entity, updatedObject);
            
            table.Execute(TableOperation.Delete(entity));
            table.Execute(TableOperation.Insert(res));
            return res;
        }

        public static void Delete(string name, string rowKey)
        {
            var table = Table(name);
            var entity = Get(name, new RowKey(rowKey));

            if (entity == null) throw new EntityNotFoundException();

            table.Execute(TableOperation.Delete(entity));
        }
    }

    public class EntityNotFoundException : Exception
    {
    }

    public class RelationAlreadyExists : Exception
    {
    }

    public class RowKey
    {
        private readonly string _rowKey;

        public RowKey(string rowKey)
        {
            _rowKey = rowKey;
        }

        public override string ToString()
        {
            return _rowKey;
        }
    }

    public class PartitionKey
    {
        private readonly string _rowKey;

        public PartitionKey(string rowKey)
        {
            _rowKey = rowKey;
        }

        public override string ToString()
        {
            return _rowKey;
        }
    }
}