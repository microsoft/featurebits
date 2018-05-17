using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
        public IEnumerable<FeatureBitDefinition> GetAll()
        {
            TableContinuationToken token = null;
            var results = new List<FeatureBitDefinition>();
            TableQuery<FeatureBitDefinition> query = new TableQuery<FeatureBitDefinition>();
            do
            {
                var resultSegment = _table.ExecuteQuerySegmentedAsync(query, token).Result;
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
            featureBit.Id = GetNewId();
            var insertOp = TableOperation.Insert(featureBit);
            return _table.ExecuteAsync(insertOp).GetAwaiter().GetResult().Result as FeatureBitDefinition;
        }

        public void Update(FeatureBitDefinition featureBit)
        {
            var existing = GetExistingFeatureBit(featureBit);
            if (existing == null)
            {
                throw new DataException($"Could not update.  Feature bit with name '{featureBit.Name}' does not exist");
            }
            var replaceOp = TableOperation.Replace(UpdateEntity(existing, featureBit));
            _table.ExecuteAsync(replaceOp).GetAwaiter().GetResult();
        }

        public void Remove(FeatureBitDefinition definition)
        {
            var existing = GetExistingFeatureBit(definition);
            if (existing != null) {
                _table.ExecuteAsync(TableOperation.Delete(existing)).GetAwaiter().GetResult();
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

        private int GetNewId() {
            TableContinuationToken token = null;
            List<int> ints = new List<int>();
            TableQuery<DynamicTableEntity> projectionQuery = new TableQuery<DynamicTableEntity>().Select(new string[] { "Id" });
            EntityResolver<int> resolver = (pk, rk, ts, props, etag) => props.ContainsKey("Id") ? props["Id"].Int32Value.Value : 0;

            do
            {
                var resultSegment = _table.ExecuteQuerySegmentedAsync(projectionQuery, resolver, token, null, null).Result;
                ints.AddRange(resultSegment.Results);
                token = resultSegment.ContinuationToken;
            }
            while (token != null);
            return ints.OrderByDescending(x => x).First() + 1;
        }
    }
}
