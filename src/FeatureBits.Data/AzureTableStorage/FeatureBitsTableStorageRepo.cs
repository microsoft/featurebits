using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace FeatureBits.Data.AzureTableStorage
{
    public class FeatureBitsTableStorageRepo : IFeatureBitsRepo
    {
        private CloudStorageAccount storageAccount;
        private CloudTableClient tableClient;
        private CloudTable table;
        public FeatureBitsTableStorageRepo(string connectionString)
        {
            storageAccount = CloudStorageAccount.Parse(connectionString);
            tableClient = storageAccount.CreateCloudTableClient();
            table = tableClient.GetTableReference("FeatureBits");
        }
        public IEnumerable<FeatureBitDefinition> GetAll()
        {
            TableContinuationToken token = null;
            var results = new List<FeatureBitDefinition>();
            TableQuery<FeatureBitDefinition> query = new TableQuery<FeatureBitDefinition>();
            do
            {
                var resultSegment = table.ExecuteQuerySegmentedAsync(query, token).Result;
                results.AddRange(resultSegment.Results);
                token = resultSegment.ContinuationToken;

            } while (token != null);
            return results;
        }

        public FeatureBitDefinition Add(FeatureBitDefinition definition)
        {
            var insertOp = TableOperation.Insert(definition);
            var result = table.ExecuteAsync(insertOp).Result;
            return definition;
        }

        public void Update(FeatureBitDefinition definition)
        {
            throw new NotImplementedException();
        }

        public void Remove(FeatureBitDefinition definition)
        {
            throw new NotImplementedException();
        }

        public async Task DoThings()
        {
            await table.CreateIfNotExistsAsync();
        }
    }
}
