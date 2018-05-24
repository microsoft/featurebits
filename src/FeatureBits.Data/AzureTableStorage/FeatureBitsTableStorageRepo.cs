using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace FeatureBits.Data.AzureTableStorage
{
    public class FeatureBitsTableStorageRepo : IFeatureBitsRepo
    {
        private readonly CloudTable _table;
        public FeatureBitsTableStorageRepo(string connectionString, string tableName)
        {
            _table = CloudStorageAccount.Parse(connectionString)
                .CreateCloudTableClient()
                .GetTableReference(tableName);
            _table.CreateIfNotExistsAsync().GetAwaiter().GetResult();
        }
        public async Task<IEnumerable<FeatureBitDefinition>> GetAllAsync()
        {
            TableContinuationToken token = null;
            var results = new List<FeatureBitDefinition>();
            TableQuery<FeatureBitDefinition> query = new TableQuery<FeatureBitDefinition>();
            do
            {
                var resultSegment = await _table.ExecuteQuerySegmentedAsync(query, token);
                results.AddRange(resultSegment.Results);
                token = resultSegment.ContinuationToken;

            } while (token != null);
            return results;
        }

        public async Task<FeatureBitDefinition> AddAsync(FeatureBitDefinition definition)
        {
            if (await GetExistingFeatureBit(definition) != null)
            {
                throw new DataException($"Cannot add. Feature bit with name '{definition.Name}' already exists.");
            }
            definition.Id = await GetNextId();
            var insertOp = TableOperation.Insert(definition);
            var result = await _table.ExecuteAsync(insertOp);
            return result.Result as FeatureBitDefinition;
        }

        public async Task UpdateAsync(FeatureBitDefinition definition)
        {
            var existing = await GetExistingFeatureBit(definition);
            if (existing == null)
            {
                throw new DataException($"Could not update.  Feature bit with name '{definition.Name}' does not exist");
            }
            existing.Update(definition);
            var replaceOp = TableOperation.Replace(existing);
            await _table.ExecuteAsync(replaceOp);
        }

        public async Task RemoveAsync(FeatureBitDefinition definition)
        {
            var existing = await GetExistingFeatureBit(definition);
            if (existing != null)
            {
                await _table.ExecuteAsync(TableOperation.Delete(existing));
            }
        }

        private async Task<FeatureBitDefinition> GetExistingFeatureBit(FeatureBitDefinition definition)
        {
            var tableResult = await _table.ExecuteAsync(TableOperation.Retrieve<FeatureBitDefinition>(definition.PartitionKey, definition.RowKey));
            return tableResult.Result as FeatureBitDefinition;
        }

        private async Task<int> GetNextId()
        {
            TableContinuationToken token = null;
            List<int> ints = new List<int>();
            TableQuery<DynamicTableEntity> projectionQuery = new TableQuery<DynamicTableEntity>().Select(new string[] { "Id" });
            EntityResolver<int> resolver = (pk, rk, ts, props, etag) => props.ContainsKey("Id") ? props["Id"].Int32Value.GetValueOrDefault() : -1;

            var maxInt = -1;
            do
            {
                var resultSegment = await _table.ExecuteQuerySegmentedAsync(projectionQuery, resolver, token, null, null);
                var currentMax = resultSegment.Results.Max();
                maxInt = Math.Max(currentMax, maxInt);
                token = resultSegment.ContinuationToken;
            }
            while (token != null);
            return maxInt + 1;
        }

        public async Task<FeatureBitDefinition> GetByNameAsync(string featureBitName)
        {
            var tableResult = await _table.ExecuteAsync(TableOperation.Retrieve<FeatureBitDefinition>(_table.Name, featureBitName));
            return tableResult.Result as FeatureBitDefinition;
        }
    }
}
