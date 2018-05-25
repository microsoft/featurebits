using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using FeatureBits.Data;

namespace Dotnet.FBit.Command
{
    public class CommandFeatureBitDefintion : IFeatureBitDefinition
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
        public int ExactAllowedPermissionLevel { get; set; }

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
        /// <see cref="IFeatureBitDefinition.Update"/>
        /// </summary>
        public void Update(IFeatureBitDefinition newEntity)
        {
            AllowedUsers = newEntity.AllowedUsers;
            LastModifiedByUser = newEntity.LastModifiedByUser;
            ExcludedEnvironments = newEntity.ExcludedEnvironments;
            LastModifiedDateTime = newEntity.LastModifiedDateTime;
            MinimumAllowedPermissionLevel = newEntity.MinimumAllowedPermissionLevel;
            OnOff = newEntity.OnOff;
        }
    }
}
