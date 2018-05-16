using System;
using System.Collections.Generic;
using System.Data;
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
            table.CreateIfNotExistsAsync().GetAwaiter().GetResult();
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

        public FeatureBitDefinition Add(FeatureBitDefinition featureBit)
        {
            if (GetExistingFeatureBit(featureBit) != null) {
                throw new DataException($"Cannot add. Feature bit with name '{featureBit.Name}' already exists.");
            }
            var insertOp = TableOperation.Insert(featureBit);
            return table.ExecuteAsync(insertOp).Result.Result as FeatureBitDefinition;
        }

        public void Update(FeatureBitDefinition definition)
        {
            throw new NotImplementedException();
        }

        public void Remove(FeatureBitDefinition definition)
        {
            var existing = GetExistingFeatureBit(definition);
            if (existing != null) {
                // TODO: Remove object from table
            }
        }

        private FeatureBitDefinition GetExistingFeatureBit(FeatureBitDefinition featureBit)
        {
            return table.ExecuteAsync(
                TableOperation.Retrieve<FeatureBitDefinition>(featureBit.PartitionKey, featureBit.RowKey)
            ).GetAwaiter().GetResult().Result as FeatureBitDefinition;
        }
    }
}
