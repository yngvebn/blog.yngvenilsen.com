using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
            dynamic result = Table(resource).ExecuteQuery(query).First(e => e.RowKey.Equals(rowkey.ToString()) && e.PartitionKey.Equals(partition.ToString()));
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


        public static IEnumerable<ElasticTableEntity> Get(string resource)
        {
            var query = new TableQuery<ElasticTableEntity>();
            dynamic result = Table(resource).ExecuteQuery(query);
            return result;
        }
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