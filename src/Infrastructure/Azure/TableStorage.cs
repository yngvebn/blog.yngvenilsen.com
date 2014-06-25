using System.Configuration;
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
    }
}