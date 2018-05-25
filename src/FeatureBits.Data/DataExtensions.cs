using System;
using FeatureBits.Data.AzureTableStorage;

namespace FeatureBits.Data
{
    public static class DataExtensions
    {
        public static FeatureBitTableDefinition ToTableDefinition (this IFeatureBitDefinition definition, string tableName) {
            return new FeatureBitTableDefinition
            {
                Name = definition.Name,
                PartitionKey = tableName,
                RowKey = definition.Name,
                ExcludedEnvironments = definition.ExcludedEnvironments,
                ExactAllowedPermissionLevel = definition.ExactAllowedPermissionLevel,
                AllowedUsers = definition.AllowedUsers,
                CreatedByUser = definition.CreatedByUser,
                CreatedDateTime = definition.CreatedDateTime,
                OnOff = definition.OnOff,
                Id = definition.Id,
                LastModifiedByUser = definition.LastModifiedByUser,
                LastModifiedDateTime = definition.LastModifiedDateTime,
                MinimumAllowedPermissionLevel = definition.MinimumAllowedPermissionLevel
            };
        }
    }
}
