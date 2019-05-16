// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.ComponentModel.DataAnnotations;

namespace FeatureBits.Data.AzureTableStorage
{
    /// <summary>
    /// This represents the data structure used to define Feature BitsData
    /// </summary>
    public class FeatureBitTableDefinition : TableEntity, IFeatureBitDefinition
    {
        /// <summary>
        /// <see cref="IFeatureBitDefinition.Id"/>
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// <see cref="IFeatureBitDefinition.Name"/>
        /// </summary>
        [Required, MaxLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// <see cref="IFeatureBitDefinition.OnOff"/>
        /// </summary>
        public bool OnOff { get; set; }

        /// <summary>
        /// <see cref="IFeatureBitDefinition.ExcludedEnvironments"/>
        /// </summary>
        [MaxLength(300)]
        public string ExcludedEnvironments { get; set; }

        /// <summary>
        /// <see cref="IFeatureBitDefinition.MinimumAllowedPermissionLevel"/>
        /// </summary>
        public int MinimumAllowedPermissionLevel { get; set; }

        /// <summary>
        /// <see cref="IFeatureBitDefinition.ExactAllowedPermissionLevel"/>
        /// </summary>
        public int? ExactAllowedPermissionLevel { get; set; }

        /// <summary>
        /// <see cref="IFeatureBitDefinition.AllowedUsers"/>
        /// </summary>
        [MaxLength(2048)]
        public string AllowedUsers { get; set; }

        /// <summary>
        /// <see cref="IFeatureBitDefinition.CreatedDateTime"/>
        /// </summary>
        [Required]
        public DateTime CreatedDateTime { get; set; }

        /// <summary>
        /// <see cref="IFeatureBitDefinition.CreatedByUser"/>
        /// </summary>
        [Required, MaxLength(100)]
        public string CreatedByUser { get; set; }

        /// <summary>
        /// <see cref="IFeatureBitDefinition.LastModifiedDateTime"/>
        /// </summary>
        [Required]
        public DateTime LastModifiedDateTime { get; set; }

        /// <summary>
        /// <see cref="IFeatureBitDefinition.LastModifiedByUser"/>
        /// </summary>
        [Required, MaxLength(100)]
        public string LastModifiedByUser { get; set; }

        /// <summary>
        /// <see cref="IFeatureBitDefinition.DependentIds"/>
        /// </summary>
        [MaxLength(255)]
        public string DependentIds { get; set; }

        /// <summary>
        /// <see cref="IFeatureBitDefinition.Update"/>
        /// </summary>
        public void Update(IFeatureBitDefinition newEntity)
        {
            AllowedUsers = newEntity.AllowedUsers;
            LastModifiedByUser = newEntity.LastModifiedByUser;
            ExcludedEnvironments = newEntity.ExcludedEnvironments;
            LastModifiedDateTime = newEntity.LastModifiedDateTime;
            MinimumAllowedPermissionLevel = newEntity.MinimumAllowedPermissionLevel;
            ExactAllowedPermissionLevel = newEntity.ExactAllowedPermissionLevel;
            OnOff = newEntity.OnOff;
            DependentIds = newEntity.DependentIds;
        }
    }
}
