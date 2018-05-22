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
        public FeatureBitsTableStorageRepo(string connectionString)
        {
            _table = CloudStorageAccount.Parse(connectionString)
                .CreateCloudTableClient()
                .GetTableReference("FeatureBits");
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

        public async Task<FeatureBitDefinition> AddAsync(FeatureBitDefinition featureBit)
        {
            if (GetExistingFeatureBit(featureBit) != null) {
                throw new DataException($"Cannot add. Feature bit with name '{featureBit.Name}' already exists.");
            }
            featureBit.Id = await GetNewId();
            var insertOp = TableOperation.Insert(featureBit);
            var result = await _table.ExecuteAsync(insertOp);
            return result.Result as FeatureBitDefinition;
        }

        public async Task UpdateAsync(FeatureBitDefinition featureBit)
        {
            var existing = GetExistingFeatureBit(featureBit);
            if (existing == null)
            {
                throw new DataException($"Could not update.  Feature bit with name '{featureBit.Name}' does not exist");
            }
            var replaceOp = TableOperation.Replace(UpdateEntity(existing, featureBit));
            await _table.ExecuteAsync(replaceOp);
        }

        public async Task RemoveAsync(FeatureBitDefinition definition)
        {
            var existing = GetExistingFeatureBit(definition);
            if (existing != null) {
                await _table.ExecuteAsync(TableOperation.Delete(existing));
            }
        }

        private FeatureBitDefinition GetExistingFeatureBit(FeatureBitDefinition featureBit)
        {
            return _table.ExecuteAsync(
                TableOperation.Retrieve<FeatureBitDefinition>(featureBit.PartitionKey, featureBit.RowKey)
            ).GetAwaiter().GetResult().Result as FeatureBitDefinition;
        }

        private FeatureBitDefinition UpdateEntity(FeatureBitDefinition existingEntity, FeatureBitDefinition newEntity)
        {
            existingEntity.AllowedUsers = newEntity.AllowedUsers;
            existingEntity.LastModifiedByUser = newEntity.LastModifiedByUser;
            existingEntity.ExcludedEnvironments = newEntity.ExcludedEnvironments;
            existingEntity.LastModifiedDateTime = newEntity.LastModifiedDateTime;
            existingEntity.MinimumAllowedPermissionLevel = newEntity.MinimumAllowedPermissionLevel;
            existingEntity.OnOff = newEntity.OnOff;
            return existingEntity;
        }

        private async Task<int> GetNewId() {
            TableContinuationToken token = null;
            List<int> ints = new List<int>();
            TableQuery<DynamicTableEntity> projectionQuery = new TableQuery<DynamicTableEntity>().Select(new string[] { "Id" });
            EntityResolver<int> resolver = (pk, rk, ts, props, etag) => props.ContainsKey("Id") ? props["Id"].Int32Value.Value : 0;

            do
            {
                var resultSegment = await _table.ExecuteQuerySegmentedAsync(projectionQuery, resolver, token, null, null);
                ints.AddRange(resultSegment.Results);
                token = resultSegment.ContinuationToken;
            }
            while (token != null);
            return ints.OrderByDescending(x => x).First() + 1;
        }
    }
}
